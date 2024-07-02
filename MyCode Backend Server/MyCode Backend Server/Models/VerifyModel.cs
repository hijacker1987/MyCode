namespace MyCode_Backend_Server.Models
{
    public class VerifyModel(string attachment, bool external = false)
    {
        public string Attachment { get; set; } = attachment ?? throw new ArgumentNullException(nameof(attachment));
        public bool External { get; set; } = external;
    }
}
