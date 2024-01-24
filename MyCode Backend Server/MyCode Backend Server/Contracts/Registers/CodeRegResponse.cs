namespace MyCode_Backend_Server.Contracts.Registers
{
    public record CodeRegResponse
        (
            string Id,
            string CodeTitle,
            string MyCode,
            string WhatKindOfCode,
            bool IsBackend,
            bool IsVisible
        );
}
