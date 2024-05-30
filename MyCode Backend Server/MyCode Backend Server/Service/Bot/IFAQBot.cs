using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Bot
{
    public interface IFAQBot
    {
        Task<string> OnMessageActivityAsync(BotMessage message);
    }
}
