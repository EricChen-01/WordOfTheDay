
namespace WordOfTheDay.Models;

using Newtonsoft.Json;

public record WordOfTheDayResponse
{
    [JsonProperty("word")]
    public required string Word { get; init; }

    [JsonProperty("meaning")]
    public required string Meaning { get; init; }

    [JsonProperty("furigana")]
    public required string Furigana { get; init; }

    [JsonProperty("level")]
    public required string Level { get; init; }

    [JsonProperty("partsOfSpeech")]
    public required List<string> PartsOfSpeech { get; init; }
}