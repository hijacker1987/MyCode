using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Services
{
    public record ChangePassRequest
    {
        public readonly static int MaxPasswordLength = 20;


        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [MinLength(6), MaxLength(20)]
        public string CurrentPassword { get; init; }

        [Required]
        [MinLength(6), MaxLength(20)]
        public string NewPassword { get; init; }

        public ChangePassRequest(string email, string currentPassword, string newPassword)
        {
            Email = email;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }
}
