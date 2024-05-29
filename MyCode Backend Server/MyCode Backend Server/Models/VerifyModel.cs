namespace MyCode_Backend_Server.Models
{
    public class VerifyModel(string userId, string attachment, bool external = false)
    {
        public string UserId { get; set; } = userId ?? throw new ArgumentNullException(nameof(userId));
        public string Attachment { get; set; } = attachment ?? throw new ArgumentNullException(nameof(attachment));
        public bool External { get; set; } = external;
    }
}
