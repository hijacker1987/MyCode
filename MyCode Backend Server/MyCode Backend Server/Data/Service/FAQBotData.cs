using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data.Service
{
    public class FAQBotData
    {
        public async Task InitializeFAQBBotDataAsync(DataContext context)
        {
            var faqDatabase = new Dictionary<string, string>
            {
                { "why i can't add codes?", "Before at all You have to verify Yourself! Please attach an external account, than if You wish to verify it, You'll receive a verification code via E-mail." },
                { "how can i set up a new password?", "Through the My Account button You'll see it." },
                { "how can i change my username?", "Through the My Account button You'll see it." },
            };

            foreach (var key in faqDatabase)
            {
                var createdBot = new BotModel { Question = key.Key, Answer = key.Value };

                await context.BotDb!.AddAsync(createdBot);
            }

            await context.SaveChangesAsync();
        }
    }
}
