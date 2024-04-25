using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data.Service
{
    public class FAQBotData
    {
        public async Task InitializeFAQBBotDataAsync(DataContext context)
        {
            var faqDatabase = new Dictionary<string, string>
            {
                { "can't add codes", "Before at all You have to verify Yourself! Please go to My Account and press on VERIFICATION." },
                { "cannot add codes", "Before at all You have to verify Yourself! Please go to My Account and press on VERIFICATION." },
                { "unable add codes", "Before at all You have to verify Yourself! Please go to My Account and press on VERIFICATION." },
                
                { "verify", "The app is using multi-factor authentication, if You wish to verify it, You'll receive a verification code via E-mail to Your externally attached e-mail address. Just prove Yourself in My Account/Verification" },
                { "verification", "The app is using multi-factor authentication, if You wish to verify it, You'll receive a verification code via E-mail to Your externally attached e-mail address. Just prove Yourself in My Account/Verification" },
                { "auth", "The app is using multi-factor authentication, if You wish to verify it, You'll receive a verification code via E-mail to Your externally attached e-mail address. Just prove Yourself in My Account/Verification" },
                { "authentication", "The app is using multi-factor authentication, if You wish to verify it, You'll receive a verification code via E-mail to Your externally attached e-mail address. Just prove Yourself in My Account/Verification" },
                
                { "set password", "Through the My Account look for the PASSWORD CHANGE button." },
                { "setup password", "Through the My Account look for the PASSWORD CHANGE button." },
                { "change password", "Through the My Account look for the PASSWORD CHANGE button." },
                
                { "set username", "Through the My Account simply type in the name You desire (can't contain spaces), than press UPDATE USER." },
                { "setup username", "Through the My Account simply type in the name You desire (can't contain spaces), than press UPDATE USER." },
                { "change username", "Through the My Account simply type in the name You desire (can't contain spaces), than press UPDATE USER." },
                { "set user name", "Through the My Account simply type in the name You desire (can't contain spaces), than press UPDATE USER." },
                { "setup user name", "Through the My Account simply type in the name You desire (can't contain spaces), than press UPDATE USER." },
                { "change user name", "Through the My Account simply type in the name You desire (can't contain spaces), than press UPDATE USER." },

                { "set email", "Through the My Account simply type in the address You desire (must be a proper e-mail address), than press UPDATE USER." },
                { "setup email", "Through the My Account simply type in the address You desire (must be a proper e-mail address), than press UPDATE USER." },
                { "change email", "Through the My Account simply type in the address You desire (must be a proper e-mail address), than press UPDATE USER." },
                { "set e-mail", "Through the My Account simply type in the address You desire (must be a proper e-mail address), than press UPDATE USER." },
                { "setup e-mail", "Through the My Account simply type in the address You desire (must be a proper e-mail address), than press UPDATE USER." },
                { "change e-mail", "Through the My Account simply type in the address You desire (must be a proper e-mail address), than press UPDATE USER." },
                
                { "set displayname", "Through the My Account simply type in the name You desire, than press UPDATE USER." },
                { "setup displayname", "Through the My Account simply type in the name You desire, than press UPDATE USER." },
                { "change displayname", "Through the My Account simply type in the name You desire, than press UPDATE USER." },
                { "set display name", "Through the My Account simply type in the name You desire, than press UPDATE USER." },
                { "setup display name", "Through the My Account simply type in the name You desire, than press UPDATE USER." },
                { "change display name", "Through the My Account simply type in the name You desire, than press UPDATE USER." },

                { "set mfa", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "setup mfa", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "add mfa", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "set multifactor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "setup multifactor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "add multifactor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "set multi-factor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "setup multi-factor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "add multi-factor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "set multi factor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "setup multi factor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "add multi factor", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "set auth", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "setup auth", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "add auth", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "set authentication", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "setup authentication", "Through the My Account button look for VERIFICATION button, and follow the hints." },
                { "add authentication", "Through the My Account button look for VERIFICATION button, and follow the hints." },

                { "change code", "Through the My Codes button look for Your code which one You would like to change, and press EDIT on the right." },
                { "update code", "Through the My Codes button look for Your code which one You would like to change, and press EDIT on the right." },

                { "delete code", "Through the My Codes button look for Your code which one You would like to change, and press DELETE on the right." },

                { "delete account", "Through the My Account button look for DELETE ACCOUNT button." },

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
