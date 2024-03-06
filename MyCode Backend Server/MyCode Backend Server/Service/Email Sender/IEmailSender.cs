namespace MyCode_Backend_Server.Service.Email_Sender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
