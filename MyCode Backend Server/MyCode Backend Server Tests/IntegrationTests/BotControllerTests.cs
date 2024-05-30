using Microsoft.AspNetCore.Mvc;
using Moq;
using MyCode_Backend_Server.Controllers;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Bot;
using Newtonsoft.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Controllers
{
    public class BotControllerTests
    {
        [Fact]
        public async Task PostAsync_ValidMessage_ReturnsOkResult()
        {
            // Arrange
            var botMessage = new BotMessage { Text = "Hello" };
            var botResponse = "Howdy! How may I'll be at your service?!";
            var mockBot = new Mock<IFAQBot>();
            mockBot.Setup(bot => bot.OnMessageActivityAsync(It.IsAny<BotMessage>()))
                   .ReturnsAsync(botResponse);

            var controller = new BotController(mockBot.Object);

            // Act
            var result = await controller.PostAsync(botMessage) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(botResponse), result.Value);
        }

        [Fact]
        public async Task PostAsync_NullMessage_ReturnsOkResult()
        {
            // Arrange
            var mockBot = new Mock<IFAQBot>();
            var controller = new BotController(mockBot.Object);

            // Act
            var result = await controller.PostAsync(null!) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(string.Empty), result.Value);
        }

        [Fact]
        public async Task PostAsync_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var botMessage = new BotMessage { Text = "Hello" };
            var mockBot = new Mock<IFAQBot>();
            mockBot.Setup(bot => bot.OnMessageActivityAsync(It.IsAny<BotMessage>()))
                   .ThrowsAsync(new Exception("Test exception"));

            var controller = new BotController(mockBot.Object);

            // Act
            var result = await controller.PostAsync(botMessage) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("Internal Server Error: Test exception", result.Value);
        }
    }
}
