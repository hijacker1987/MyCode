using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Services
{
    public record ChangePassRequest
        (
            [Required] string Email,
            [Required] string CurrentPassword,
            [Required][MinLength(6)] string NewPassword
        );
}
