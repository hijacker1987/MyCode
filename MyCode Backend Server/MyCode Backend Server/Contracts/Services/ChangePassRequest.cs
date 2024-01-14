using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Services
{
    public record ChangePassRequest
    {
        [Required]
        public string Email { get; init; }

        [Required]
        public string CurrentPassword { get; init; }

        [Required]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; init; }

        public ChangePassRequest(string email, string currentPassword, string newPassword)
        {
            Email = email;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }
}
