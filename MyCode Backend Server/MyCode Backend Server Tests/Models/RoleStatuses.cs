using Xunit;
using MyCode_Backend_Server.Contracts.Registers;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Models
{
    public class RoleStatusRequestTests
    {
        [Fact]
        public void RoleStatusRequest_CanBeInitialized()
        {
            // Arrange
            var status = "Active";

            // Act
            var roleStatusRequest = new RoleStatusRequest(status);

            // Assert
            Assert.Equal(status, roleStatusRequest.Status);
        }

        [Fact]
        public void RoleStatusRequest_CanHandleNullStatus()
        {
            // Act
            var roleStatusRequest = new RoleStatusRequest(null!);

            // Assert
            Assert.Null(roleStatusRequest.Status);
        }

        [Fact]
        public void RoleStatusResponse_CanBeInitialized()
        {
            // Arrange
            var status = "Active";

            // Act
            var roleStatusRequest = new RoleStatusResponse(status);

            // Assert
            Assert.Equal(status, roleStatusRequest.Status);
        }

        [Fact]
        public void RoleStatusResponse_CanHandleNullStatus()
        {
            // Act
            var roleStatusRequest = new RoleStatusResponse(null!);

            // Assert
            Assert.Null(roleStatusRequest.Status);
        }
    }
}
