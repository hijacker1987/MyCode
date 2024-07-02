using MyCode_Backend_Server.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Models
{
    public class RefreshTM_And_Verification_Tests
    {
        /*private static void ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, context, results, true);
            if (!isValid)
            {
                throw new ValidationException(results[0], null, null);
            }
        }*/

        [Fact]
        public void RefreshTM_Validation_Success()
        {
            // Arrange
            var refreshTM = new RefreshTM
            {
                AccessToken = "testAccessToken",
                RefreshToken = "testRefreshToken"
            };

            // Act & Assert
            Assert.Equal("testAccessToken", refreshTM.AccessToken);
            Assert.Equal("testRefreshToken", refreshTM.RefreshToken);
        }

        [Fact]
        public void VerifyModel_CanBeInitialized()
        {
            // Arrange
            var attachment = "attachment.pdf";
            var external = true;

            // Act
            var verifyModel = new VerifyModel
            (
                attachment,
                external
            );

            // Assert
            Assert.Equal(attachment, verifyModel.Attachment);
            Assert.True(verifyModel.External);
        }

        [Fact]
        public void VerifyModel_DefaultExternalValueIsFalse()
        {
            // Arrange
            var attachment = "attachment.pdf";

            // Act
            var verifyModel = new VerifyModel(attachment);

            // Assert
            Assert.Equal(attachment, verifyModel.Attachment);
            Assert.False(verifyModel.External);
        }
    }
}
