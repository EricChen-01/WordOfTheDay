using Newtonsoft.Json;

namespace WordOfTheDay.Models;

public record JishoData
{
    [JsonProperty("slug")]
    public string Slug { get; init; }

    [JsonProperty("is_common")]
    public bool IsCommon { get; init; }

    [JsonProperty("jlpt")]
    public List<string> Jlpt { get; init; }

    [JsonProperty("japanese")]
    public List<JishoJapanese> Japanese { get; init; }

    [JsonProperty("senses")]
    public List<JishoSense> Senses { get; init; }
}
