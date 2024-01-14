namespace MyCode_Backend_Server.Contracts.Registers
{
    public record CodeRegResponse
        (
            Guid Id,
            string CodeTitle,
            string MyCode,
            string WhatKindOfCode,
            bool IsBackend,
            bool IsVisible
        );
}
