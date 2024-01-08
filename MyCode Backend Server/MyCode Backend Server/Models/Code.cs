using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class Code
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string MyCode { get; set; }
        public bool IsVisible { get; set; }

        public Code(string myCode, bool isVisible = false)
        {
            MyCode = myCode;
            IsVisible = isVisible;
        }

        public Code()
        {

        }
    }
}
