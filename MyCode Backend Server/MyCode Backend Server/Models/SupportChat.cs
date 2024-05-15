using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class SupportChat
    {
        [Key]
        public Guid SupportId { get; set; }
        public virtual string? Text { get; set; }
        public bool IsUser { get; set; } = true;
        public DateTime When { get; set; }
        public Guid With { get; set; }
        public bool IsActive { get; set; }
        public byte[] VersionForOptimisticLocking { get; set; } = [];

        public SupportChat(string text)
        {
            Text = text;
        }

        public SupportChat()
        {
            SupportId = Guid.NewGuid();
            When = DateTime.UtcNow;
            IsActive = true;
        }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
