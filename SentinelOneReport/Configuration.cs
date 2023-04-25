namespace SentinelOneReport;

public class Configuration
{
    public required string EmailFrom { get; set; }
    public required string EmailFromName { get; set; }
    public required string EmailTo { get; set; }
    public required string EmailToName { get; set; }
    public required string SentinelApiKey { get; set; }
    public required string SentinelHost { get; set; }
    public required string SmtpAddress { get; set; }
}
