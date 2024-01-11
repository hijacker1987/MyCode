using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Contracts.Registers
{
    public record CodeRegRequest
    (
        [Required] string CodeTitle,
        [Required] string MyCode,
        [Required] string WhatKindOfCode,
        [Required] bool IsBackend,
        [Required] bool IsVisible
    );
}
