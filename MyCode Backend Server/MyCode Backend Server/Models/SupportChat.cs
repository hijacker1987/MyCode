using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class SupportChat
    {
        [Key]
        public Guid SupportId { get; set; }
        public string? Text { get; set; }
        public DateTime When { get; set; }
        public Guid With { get; set; }

        public SupportChat(string text)
        {
            Text = text;
        }

        public SupportChat()
        {
            SupportId = Guid.NewGuid();
            When = DateTime.UtcNow;
        }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
