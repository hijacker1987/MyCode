using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Controllers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Chat;
using System.Linq.Expressions;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Chat
{
    public class MessageHubTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IHubCallerClients> _clientsMock;
        private readonly Mock<ISingleClientProxy> _singleClientProxyMock;
        private readonly Mock<HubCallerContext> _contextMock;
        private readonly Mock<IGroupManager> _groupManagerMock;
        private readonly Dictionary<string, ChatRooms> _connections;
        private readonly MessageHub _messageHub;
        private readonly Mock<DbSet<User>> _usersDbSetMock;
        private readonly Mock<DbSet<SupportChat>> _supportChatDbSetMock;
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IChatService> _chatService;
        private readonly Mock<IAuthService> _authService;
        private readonly Mock<ILogger<MessageHub>> _loggerMock;

        public MessageHubTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _clientsMock = new Mock<IHubCallerClients>();
            _singleClientProxyMock = new Mock<ISingleClientProxy>();
            _contextMock = new Mock<HubCallerContext>();
            _groupManagerMock = new Mock<IGroupManager>();
            _usersDbSetMock = new Mock<DbSet<User>>();
            _supportChatDbSetMock = new Mock<DbSet<SupportChat>>();
            _dataContextMock = new Mock<DataContext>();
            _chatService = new Mock<IChatService>();
            _authService = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<MessageHub>>();

            _connections = [];
            _messageHub = new MessageHub(_connections, _userManagerMock.Object, _dataContextMock.Object, _chatService.Object, _authService.Object, _loggerMock.Object)
            {
                Clients = _clientsMock.Object,
                Context = _contextMock.Object,
                Groups = _groupManagerMock.Object,
            };
        }

        [Fact]
        public async Task LeaveRoom_RemovesUserFromGroupAndMarksMessagesAsInactive()
        {
            // Arrange
            var room = new ChatRooms { ChatRoom = "testRoom" };
            var messages = new List<SupportChat>
            {
                new(Guid.NewGuid().ToString())
            };

            _contextMock.Setup(c => c.ConnectionId).Returns("testConnectionId");
            var supportChats = messages.AsQueryable();
            _supportChatDbSetMock.SetupDataAsync(supportChats);
            _dataContextMock.Setup(d => d.SupportDb).Returns(_supportChatDbSetMock.Object);

            _clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_singleClientProxyMock.Object);

            // Act
            var result = await _messageHub.LeaveRoom(room);

            // Assert
            Assert.Equal(200, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task OnDisconnectedAsync_RemovesUserFromRoomAndNotifiesGroup()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), DisplayName = "Test User" };
            var chatRoom = new ChatRooms { ChatRoom = "testRoom", User = user.Id.ToString() };

            _connections["testConnectionId"] = chatRoom;
            _contextMock.Setup(c => c.ConnectionId).Returns("testConnectionId");
            _authService.Setup(a => a.TryGetUserById(It.IsAny<string>())).ReturnsAsync(user);
            _clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_singleClientProxyMock.Object);

            // Act
            await _messageHub.OnDisconnectedAsync(null);

            // Assert
            _groupManagerMock.Verify(g => g.RemoveFromGroupAsync("testConnectionId", "testRoom", default), Times.Once);
            _singleClientProxyMock.Verify(c => c.SendCoreAsync("UserDisconnected", It.IsAny<object[]>(), default), Times.Once);
            Assert.False(_connections.ContainsKey("testConnectionId"));
        }
    }

    public static class MockDbSetExtensions
    {
        public static void SetupDataAsync<T>(this Mock<DbSet<T>> dbSetMock, IQueryable<T> data) where T : class
        {
            dbSetMock.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(data.Provider));
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private class TestAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator = enumerator;

            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                return default;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }

            public T Current => _enumerator.Current;
        }

        private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _queryProvider;

            internal TestAsyncQueryProvider(IQueryProvider queryProvider)
            {
                _queryProvider = queryProvider;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _queryProvider.Execute(expression)!;
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _queryProvider.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return (IAsyncEnumerable<TResult>)ExecuteAsync<TResult>(expression, CancellationToken.None)!;
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = (IQueryable)_queryProvider.Execute(expression)!;

                return (TResult)typeof(EntityFrameworkQueryableExtensions)
                    .GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync))!
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, [executionResult!, cancellationToken])!;
            }
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }
    }
}

