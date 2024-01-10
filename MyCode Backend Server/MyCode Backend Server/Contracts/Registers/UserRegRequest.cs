using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Registers
{
    public record UserRegRequest
    (
        [Required] string Username,
        [Required] string Password,
        [Required] string Email,
        [Required] string PhoneNumber
    );
}
