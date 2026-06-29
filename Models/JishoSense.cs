using Newtonsoft.Json;

namespace WordOfTheDay.Models;

public record JishoSense
{
    [JsonProperty("english_definitions")]
    public List<string> EnglishDefinitions { get; init; }

    [JsonProperty("parts_of_speech")]
    public List<string> PartsOfSpeech { get; init; }
}