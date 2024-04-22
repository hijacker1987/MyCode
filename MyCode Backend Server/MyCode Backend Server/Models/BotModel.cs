using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class BotModel
    {
        [Key]
        public Guid BotId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }

        public BotModel(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }

        public BotModel()
        {
            BotId = Guid.NewGuid();
        }
    }
}
