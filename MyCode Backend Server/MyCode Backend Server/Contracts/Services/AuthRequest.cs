using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Services
{
    public record AuthRequest([Required] string? Email, [Required] string? Password);
}
