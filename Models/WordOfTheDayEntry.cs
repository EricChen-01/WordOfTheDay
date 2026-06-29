using Azure;
using Azure.Data.Tables;

namespace WordOfTheDay.Models;

public class WordOfTheDayEntity : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string Word { get; set; } = default!;
}