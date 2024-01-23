﻿namespace MyCode_Backend_Server.Service.Authentication
{
    public record AuthResult(
        string? Id,
        bool Success,
        string? Email,
        string? UserName,
        string? DisplayName,
        string? PhoneNumber,
        string? Token
    )
    {
        public readonly Dictionary<string, string> ErrorMessages = [];
    }
}
