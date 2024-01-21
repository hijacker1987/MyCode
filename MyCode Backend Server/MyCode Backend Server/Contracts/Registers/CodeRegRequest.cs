using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Registers
{
    public record CodeRegRequest
    {
        public static readonly int MaxCodeTitleLength = 255;

        [Required]
        [MaxLength(255)]
        public string CodeTitle { get; init; }

        [Required]
        public string MyCode { get; init; }

        [Required]
        public string WhatKindOfCode { get; init; }

        [Required]
        public bool IsBackend { get; init; }

        [Required]
        public bool IsVisible { get; init; }

        public CodeRegRequest(string codeTitle, string myCode, string whatKindOfCode, bool isBackend, bool isVisible)
        {
            CodeTitle = codeTitle;
            MyCode = myCode;
            WhatKindOfCode = whatKindOfCode;
            IsBackend = isBackend;
            IsVisible = isVisible;
        }
    }
}
