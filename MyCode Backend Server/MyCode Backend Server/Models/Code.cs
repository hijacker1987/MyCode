using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class Code
    {
        [Key]
        public Guid Id { get; set; }
        public string? CodeTitle { get; set; }
        public string? MyCode { get; set; }
        public string? WhatKindOfCode { get; set; }
        public bool IsBackend { get; set; }
        public bool IsVisible { get; set; }

        public Code(string codeTitle, string myCode, string whatKindOfCode, bool isBackend = false, bool isVisible = false)
        {
            CodeTitle = codeTitle;
            MyCode = myCode;
            WhatKindOfCode = whatKindOfCode;
            IsBackend = isBackend;
            IsVisible = isVisible;
        }

        public Code()
        {
        }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
