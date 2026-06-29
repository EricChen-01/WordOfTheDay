using Newtonsoft.Json;

namespace WordOfTheDay.Models;

public record JishoJapanese
{
    [JsonProperty("word")]
    public string? Word { get; init; }

    [JsonProperty("reading")]
    public string? Reading { get; init; }
}
