namespace AttachmentSaver.Configuration;

public class MailOptions
{
    public const string Mail = "Mail";

    public string Host { get; set; } = String.Empty;
    public int Port { get; set; } = 993;
    public string Username { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public string TargetFolder { get; set; } = String.Empty;
}