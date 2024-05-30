using Xunit;
using MyCode_Backend_Server.Contracts.Registers;
using Assert = Xunit.Assert;
using MyCode_Backend_Server.Models;
namespace MyCode_Backend_Server_Tests.Models
{
    public class ChatModelTests
    {
        [Fact]
        public void ChatMessageRequest_CanBeCreated_WithValidData()
        {
            // Arrange
            var roomId = "room123";
            var userId = "user123";
            var message = "Hello, world!";

            // Act
            var request = new ChatMessageRequest(roomId, userId, message);

            // Assert
            Assert.Equal(roomId, request.RoomId);
            Assert.Equal(userId, request.UserId);
            Assert.Equal(message, request.Message);
        }

        [Fact]
        public void ChatMessageRequest_CanBeCreated_WithNullValues()
        {
            // Arrange
            string? roomId = null;
            string? userId = null;
            string? message = null;

            // Act
            var request = new ChatMessageRequest(roomId!, userId!, message!);

            // Assert
            Assert.Null(request.RoomId);
            Assert.Null(request.UserId);
            Assert.Null(request.Message);
        }

        [Fact]
        public void ChatMessageRequest_CanBeCreated_WithEmptyStrings()
        {
            // Arrange
            var roomId = string.Empty;
            var userId = string.Empty;
            var message = string.Empty;

            // Act
            var request = new ChatMessageRequest(roomId, userId, message);

            // Assert
            Assert.Equal(string.Empty, request.RoomId);
            Assert.Equal(string.Empty, request.UserId);
            Assert.Equal(string.Empty, request.Message);
        }

        [Fact]
        public void BotMessage_CanBeCreated_WithText()
        {
            // Arrange
            var text = "Hello, I am a bot!";

            // Act
            var botMessage = new BotMessage { Text = text };

            // Assert
            Assert.Equal(text, botMessage.Text);
        }

        [Fact]
        public void BotMessage_CanBeCreated_WithNullText()
        {
            // Act
            var botMessage = new BotMessage { Text = null };

            // Assert
            Assert.Null(botMessage.Text);
        }

        [Fact]
        public void BotMessage_CanUpdateText()
        {
            // Arrange
            var botMessage = new BotMessage { Text = "Initial text" };

            // Act
            botMessage.Text = "Updated text";

            // Assert
            Assert.Equal("Updated text", botMessage.Text);
        }

        [Fact]
        public void BotMessage_CanBeCreated_WithEmptyText()
        {
            // Act
            var botMessage = new BotMessage { Text = string.Empty };

            // Assert
            Assert.Equal(string.Empty, botMessage.Text);
        }

        [Fact]
        public void ChatRooms_CanBeCreated_WithUserAndChatRoom()
        {
            // Arrange
            var user = "testuser";
            var chatRoom = "testroom";

            // Act
            var chatRooms = new ChatRooms { User = user, ChatRoom = chatRoom };

            // Assert
            Assert.Equal(user, chatRooms.User);
            Assert.Equal(chatRoom, chatRooms.ChatRoom);
        }

        [Fact]
        public void ChatRooms_CanBeCreated_WithNullValues()
        {
            // Act
            var chatRooms = new ChatRooms { User = null, ChatRoom = null };

            // Assert
            Assert.Null(chatRooms.User);
            Assert.Null(chatRooms.ChatRoom);
        }

        [Fact]
        public void ChatRooms_CanUpdateUserAndChatRoom()
        {
            // Arrange
            var chatRooms = new ChatRooms { User = "initialUser", ChatRoom = "initialRoom" };

            // Act
            chatRooms.User = "updatedUser";
            chatRooms.ChatRoom = "updatedRoom";

            // Assert
            Assert.Equal("updatedUser", chatRooms.User);
            Assert.Equal("updatedRoom", chatRooms.ChatRoom);
        }

        [Fact]
        public void ChatRooms_CanBeCreated_WithEmptyStrings()
        {
            // Act
            var chatRooms = new ChatRooms { User = string.Empty, ChatRoom = string.Empty };

            // Assert
            Assert.Equal(string.Empty, chatRooms.User);
            Assert.Equal(string.Empty, chatRooms.ChatRoom);
        }

        [Fact]
        public void StoredMessage_CanBeCreated_WithWhomTextAndWhen()
        {
            // Arrange
            var whom = "testuser";
            var text = "This is a test message.";
            var when = DateTime.Now;

            // Act
            var storedMessage = new StoredMessage { Whom = whom, Text = text, When = when };

            // Assert
            Assert.Equal(whom, storedMessage.Whom);
            Assert.Equal(text, storedMessage.Text);
            Assert.Equal(when, storedMessage.When);
        }

        [Fact]
        public void StoredMessage_CanBeCreated_WithNullValues()
        {
            // Arrange
            var when = DateTime.Now;

            // Act
            var storedMessage = new StoredMessage { Whom = null, Text = null, When = when };

            // Assert
            Assert.Null(storedMessage.Whom);
            Assert.Null(storedMessage.Text);
            Assert.Equal(when, storedMessage.When);
        }

        [Fact]
        public void StoredMessage_CanUpdateWhomTextAndWhen()
        {
            // Arrange
            var storedMessage = new StoredMessage { Whom = "initialUser", Text = "Initial text", When = DateTime.Now };

            // Act
            var newWhen = DateTime.Now.AddDays(1);
            storedMessage.Whom = "updatedUser";
            storedMessage.Text = "Updated text";
            storedMessage.When = newWhen;

            // Assert
            Assert.Equal("updatedUser", storedMessage.Whom);
            Assert.Equal("Updated text", storedMessage.Text);
            Assert.Equal(newWhen, storedMessage.When);
        }

        [Fact]
        public void StoredMessage_CanBeCreated_WithEmptyStrings()
        {
            // Arrange
            var when = DateTime.Now;

            // Act
            var storedMessage = new StoredMessage { Whom = string.Empty, Text = string.Empty, When = when };

            // Assert
            Assert.Equal(string.Empty, storedMessage.Whom);
            Assert.Equal(string.Empty, storedMessage.Text);
            Assert.Equal(when, storedMessage.When);
        }

        [Fact]
        public void SupportChat_CanBeCreated_WithText()
        {
            // Arrange
            var text = "This is a test message.";

            // Act
            var supportChat = new SupportChat(text);

            // Assert
            Assert.Equal(text, supportChat.Text);
            Assert.True(supportChat.IsUser);
            Assert.Equal(default, supportChat.SupportId);
            Assert.Equal(default, supportChat.When, TimeSpan.FromSeconds(1));
            Assert.True(supportChat.IsActive);
            Assert.NotNull(supportChat.VersionForOptimisticLocking);
            Assert.Empty(supportChat.VersionForOptimisticLocking);
        }

        [Fact]
        public void SupportChat_CanBeCreated_WithDefaultConstructor()
        {
            // Act
            var supportChat = new SupportChat();

            // Assert
            Assert.Null(supportChat.Text);
            Assert.True(supportChat.IsUser);
            Assert.NotEqual(default, supportChat.SupportId);
            Assert.Equal(DateTime.UtcNow, supportChat.When, TimeSpan.FromSeconds(1));
            Assert.True(supportChat.IsActive);
            Assert.NotNull(supportChat.VersionForOptimisticLocking);
            Assert.Empty(supportChat.VersionForOptimisticLocking);
        }

        [Fact]
        public void SupportChat_CanUpdateProperties()
        {
            // Arrange
            var talkWith = Guid.NewGuid();
            var talkWhom = Guid.NewGuid();

            var supportChat = new SupportChat
            {
                Text = "Initial message",
                IsUser = false,
                When = DateTime.UtcNow.AddDays(-1),
                With = Guid.NewGuid(),
                IsActive = false,
                UserId = Guid.NewGuid(),
                VersionForOptimisticLocking = new byte[] { 1, 2, 3 }
            };

            // Act
            supportChat.Text = "Updated message";
            supportChat.IsUser = true;
            supportChat.When = DateTime.UtcNow;
            supportChat.With = talkWith;
            supportChat.IsActive = true;
            supportChat.UserId = talkWhom;
            supportChat.VersionForOptimisticLocking = new byte[] { 4, 5, 6 };

            // Assert
            Assert.Equal("Updated message", supportChat.Text);
            Assert.True(supportChat.IsUser);
            Assert.Equal(expected: DateTime.UtcNow, supportChat.When, TimeSpan.FromSeconds(1));
            Assert.Equal(talkWith, supportChat.With);
            Assert.True(supportChat.IsActive);
            Assert.Equal(talkWhom, supportChat.UserId);
            Assert.Equal(new byte[] { 4, 5, 6 }, supportChat.VersionForOptimisticLocking);
        }

        [Fact]
        public void SupportChat_CanBeCreated_WithNullText()
        {
            // Act
            var supportChat = new SupportChat(null!);

            // Assert
            Assert.Null(supportChat.Text);
            Assert.True(supportChat.IsUser);
            Assert.Equal(default, supportChat.SupportId);
            Assert.Equal(default, supportChat.When, TimeSpan.FromSeconds(1));
            Assert.True(supportChat.IsActive);
            Assert.NotNull(supportChat.VersionForOptimisticLocking);
            Assert.Empty(supportChat.VersionForOptimisticLocking);
        }
    }
}
