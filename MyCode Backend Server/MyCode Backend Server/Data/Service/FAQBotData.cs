using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data.Service
{
    public class FAQBotData
    {
        public async Task InitializeFAQBBotDataAsync(DataContext context)
        {
            var faqDatabase = new Dictionary<string, string>
            {
                { "hi hello greetings good morning afternoon evening", "Howdy! How may I'll be at your service?!" },
                { "bye see you later good night", "Cheers, behave yourself, I'll be watching You!" },
                { "can't cannot unable add codes", "Before at all You have to verify Yourself! Please go to My Account and press on VERIFICATION." },             
                { "verify verification auth authentication", "The app is using multi-factor authentication, if You wish to verify it, You'll receive a verification code via E-mail to Your externally attached e-mail address. Just prove Yourself in My Account/Verification" },
                { "set setup change password", "Through the My Account look for the PASSWORD CHANGE button." },   
                { "set setup change username user name", "Through the My Account simply type in the name You desire (can't contain spaces), than press UPDATE USER." },
                { "set setup change email e-mail", "Through the My Account simply type in the address You desire (must be a proper e-mail address), than press UPDATE USER." },
                { "set setup change displayname display name", "Through the My Account simply type in the name You desire, than press UPDATE USER." },
                { "set setup add mfa multifactor multi-factor multi factor auth authentication", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "update change code", "Through the My Codes button look for Your code which one You would like to change, and press EDIT on the right." },
                { "delete code", "Through the My Codes button look for Your code which one You would like to change, and press DELETE on the right." },
                { "delete account acc user", "Through the My Account button look for DELETE ACCOUNT button." },
                { "privacy policy", "Look around for the PRIVACY POLICY button, trust me, I won't bombing You with Ads, or selling Your data!" },
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
