namespace MyCode_Backend_Server.Contracts.Registers
{
    public record UserRegResponse(string Id, string Email, string UserName, string DisplayName, string PhoneNumber, string Role);
}
