using Newtonsoft.Json;

namespace WordOfTheDay.Models;

public record JishoKeywordResponse
{
    [JsonProperty("meta")]
    public JishoMeta Meta { get; init; }

    [JsonProperty("data")]
    public List<JishoData> Data { get; init; }
}
