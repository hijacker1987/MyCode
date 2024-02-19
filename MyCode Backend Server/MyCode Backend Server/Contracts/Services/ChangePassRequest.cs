using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Services
{
    public record ChangePassRequest
    {
        public readonly static int MaxPasswordLength = 20;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Current password is required.")]
        [MinLength(6, ErrorMessage = "Current password must be at least 6 characters.")]
        [MaxLength(20, ErrorMessage = "Current password cannot be longer than 20 characters.")]
        public string CurrentPassword { get; init; }

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
        [MaxLength(20, ErrorMessage = "New password cannot be longer than 20 characters.")]
        public string NewPassword { get; init; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; init; }

        public ChangePassRequest(string email, string currentPassword, string newPassword, string confirmPassword)
        {
            Email = email;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
        }
    }
}
