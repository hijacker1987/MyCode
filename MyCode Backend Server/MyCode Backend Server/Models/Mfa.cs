using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class Mfa
    {

        [Key]
        public Guid Id { get; set; }
        [MaxLength(255)]
        public string? ReliableEmail { get; set; }
        public bool SecondaryLoginMethod { get; set; }

        public Mfa(string? reliableEmail, bool secondaryLoginMethod = false)
        {
            ReliableEmail = reliableEmail;
            SecondaryLoginMethod = secondaryLoginMethod;
        }

        public Mfa()
        {
        }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
