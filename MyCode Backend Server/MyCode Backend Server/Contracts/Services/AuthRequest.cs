using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Services
{
    public record AuthRequest
    {
        public static readonly int MaxPasswordLength = 20;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6), MaxLength(20)]
        public string Password { get; init; }

        public AuthRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
