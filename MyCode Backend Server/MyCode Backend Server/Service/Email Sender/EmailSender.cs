using System.Net.Mail;
using System.Net;

namespace MyCode_Backend_Server.Service.Email_Sender
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("f427af4553ad24", "55f79279d450ee"),
            };

            return client.SendMailAsync(
                new MailMessage(from: "MyCodeApp@mycode.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
