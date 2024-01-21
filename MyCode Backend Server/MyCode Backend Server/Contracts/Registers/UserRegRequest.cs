using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Registers
{
    public record UserRegRequest
    {
        public readonly static int MaxPasswordLength = 20;

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        public string Username { get; init; }

        [Required]
        [MinLength(6), MaxLength(20)]
        public string Password { get; init; }

        [Required]
        public string DisplayName { get; init; }

        [Required]
        public string PhoneNumber { get; init; }

        public UserRegRequest(string email, string username, string password, string displayName, string phoneNumber)
        {
            Email = email;
            Username = username;
            Password = password;
            DisplayName = displayName;
            PhoneNumber = phoneNumber;
        }
    }
}
