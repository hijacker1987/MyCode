using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Bot;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Chat
{
    public class FAQBotTests
    {
        private readonly DataContext _dataContext;
        private readonly FAQBot _faqBot;

        public FAQBotTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("TestConnectionString");
            Assert.False(string.IsNullOrEmpty(connectionString), "Connection string should not be null or empty.");

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(connectionString)
                .Options;

            _dataContext = new DataContext(options);
            _faqBot = new FAQBot(_dataContext);
        }

        [Fact]
        public async Task OnMessageActivityAsync_NullText_ReturnsDefaultMessage()
        {
            // Arrange
            var message = new BotMessage { Text = null };

            // Act
            var result = await _faqBot.OnMessageActivityAsync(message);

            // Assert
            Assert.Equal("Sorry, I couldn't understand your message.", result);
        }

        [Fact]
        public async Task OnMessageActivityAsync_NoKeywords_ReturnsMoreSpecificMessage()
        {
            // Arrange
            var message = new BotMessage { Text = "   " };

            // Act
            var result = await _faqBot.OnMessageActivityAsync(message);

            // Assert
            Assert.Equal("Please be more specific!", result);
        }

        [Fact]
        public async Task OnMessageActivityAsync_NoMatchingFAQ_ReturnsDefaultAnswer()
        {
            // Arrange
            var message = new BotMessage { Text = "unknown keywords" };

            // Act
            var result = await _faqBot.OnMessageActivityAsync(message);

            // Assert
            Assert.Equal("Sorry, I couldn't find an answer related to your question.", result);
        }

        [Fact]
        public async Task OnMessageActivityAsync_MatchingFAQ_ReturnsAnswer()
        {
            // Arrange
            var message = new BotMessage { Text = "hi" };

            // Act
            var result = await _faqBot.OnMessageActivityAsync(message);

            // Assert
            Assert.Equal("Howdy! How may I'll be at your service?!", result);
        }
    }
}
