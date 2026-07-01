
using Newtonsoft.Json;

namespace WordOfTheDay.Models;

public record JlptWord
{
    [JsonProperty("word")]
    public required string Word { get; init; }

    [JsonProperty("meaning")]
    public required string Meaning { get; init; }

    [JsonProperty("furigana")]
    public required string Furigana { get; init; }

    [JsonProperty("romaji")]
    public required string Romaji { get; init; }

    [JsonProperty("level")]
    public required int Level { get; init; }
}