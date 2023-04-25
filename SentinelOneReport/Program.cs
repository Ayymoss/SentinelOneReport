using System.Globalization;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Reflection;
using System.Text.Json;
using CsvHelper;
using RestEase;

namespace SentinelOneReport;

public static class SentinelOneReport
{
    private static Configuration? _configuration;
    private const string ExportFolder = "Exports";

    public static async Task Main()
    {
        _configuration = ReadFromConfiguration();
        if (_configuration is null) Environment.Exit(1);
        DisplayHeader();

        try
        {
            var result = await FetchDataAsync(_configuration.SentinelApiKey);
            var outputPath = GetFilePath();
            await WriteDataToCsvAsync(result, outputPath);

            Console.WriteLine("Emailing...");
            await SendEmailWithAttachmentAsync(outputPath);
            Console.WriteLine($"Emailed {result.Pagination?.TotalItems} items {outputPath}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(1);
        }
    }

    private static string GetFilePath()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fileName = $"S1-ActionRequired-{DateTimeOffset.UtcNow.ToFileTime()}.csv";
        if (!Directory.Exists(Path.Join(directory, ExportFolder))) Directory.CreateDirectory(Path.Join(directory, ExportFolder));
        return Path.Join(directory, ExportFolder, fileName);
    }

    private static Configuration? ReadFromConfiguration()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var file = File.ReadAllText(Path.Join(directory, "Configuration", "_Configuration.json"));
        return JsonSerializer.Deserialize<Configuration>(file);
    }

    private static void DisplayHeader()
    {
        Console.WriteLine();
        Console.WriteLine("============================");
        Console.WriteLine(" SentinelOne Data Extractor");
        Console.WriteLine(" By Ryan Amos");
        Console.WriteLine("============================");
        Console.WriteLine();
    }

    private static async Task<SentinelOneDto> FetchDataAsync(string apiToken)
    {
        var api = RestClient.For<ISentinelOneEndpoints>(_configuration.SentinelHost);
        api.Authorization = $"ApiToken {apiToken}";

        Console.WriteLine("Fetching data...");
        var response = await api.GetActionRequiredComputers();
        return await response.Content.ReadFromJsonAsync<SentinelOneDto>();
    }

    private static async Task WriteDataToCsvAsync(SentinelOneDto result, string outputPath)
    {
        await using var writer = new StreamWriter(outputPath);
        await using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteHeader<ComputerInfo>();
        await csvWriter.NextRecordAsync();
        await csvWriter.WriteRecordsAsync(result.Data);
    }

    private static async Task SendEmailWithAttachmentAsync(string attachmentPath)
    {
        var fromAddress = new MailAddress(_configuration.EmailFrom, _configuration.EmailFromName);
        var toAddress = new MailAddress(_configuration.EmailTo, _configuration.EmailToName);

        const string subject = "Sentinel One - Action Required Computers";
        const string body = "Please find the attached CSV file.";

        var smtp = new SmtpClient
        {
            Host = _configuration.SmtpAddress,
            Port = 25,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };

        message.Attachments.Add(new Attachment(attachmentPath));
        await smtp.SendMailAsync(message);
    }
}
