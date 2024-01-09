﻿using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class Code
    {
        [Key]
        public Guid Id { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public string? CodeTitle { get; set; }
        public string? MyCode { get; set; }
        public bool IsVisible { get; set; }

        public Code(string codeTitle, string myCode, bool isVisible = false)
        {
            CodeTitle = codeTitle;
            MyCode = myCode;
            IsVisible = isVisible;
        }

        public Code()
        {

        }
    }
}
