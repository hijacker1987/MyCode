using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Services
{
    public record AuthRequest
    {
        public static readonly int MaxPasswordLength = 20;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format."), MinLength(4), MaxLength(30)]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6), MaxLength(20)]
        public string Password { get; init; }

        [Required(ErrorMessage = "Confirmation is required.")]
        [MinLength(6), MaxLength(20)]
        public string ConfirmPassword { get; init; }

        public AuthRequest(string email, string password, string confirmPassword)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
}
