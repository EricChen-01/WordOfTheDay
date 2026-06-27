using Newtonsoft.Json;

namespace WordOfTheDay.Models;

public record JishoMeta
{
    [JsonProperty("status")]
    public int Status { get; init; }
}
