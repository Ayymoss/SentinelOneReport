using CsvHelper.Configuration.Attributes;

namespace SentinelOneReport;

public record ComputerInfo
{
    public string? ComputerName { get; set; }
    public string? LastLoggedInUserName { get; set; }
    public string? LastIpToMgmt { get; set; }

    [TypeConverter(typeof(StringListConverter))]
    public List<string>? UserActionsNeeded { get; set; }

    public string? AgentVersion { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? GroupName { get; set; }
    public string? Uuid { get; set; }
}

public record Pagination
{
    public int TotalItems { get; set; }
}

public record SentinelOneDto
{
    public List<ComputerInfo>? Data { get; set; }
    public Pagination? Pagination { get; set; }
}
