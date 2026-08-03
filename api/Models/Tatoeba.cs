using Newtonsoft.Json;

namespace WordOfTheDay.Models;

public sealed class TatoebaSentenceResponse
{
    [JsonProperty("data")]
    public List<TatoebaSentence> Data { get; set; } = [];

    [JsonProperty("paging")]
    public TatoebaPaging? Paging { get; set; }
}

public sealed class TatoebaSentence
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("lang")]
    public string Lang { get; set; } = string.Empty;

    [JsonProperty("translations")]
    public List<TatoebaTranslation> Translations { get; set; } = [];
}

public sealed class TatoebaTranslation
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("lang")]
    public string Lang { get; set; } = string.Empty;

    [JsonProperty("is_direct")]
    public bool IsDirect { get; set; }
}

public sealed class TatoebaPaging
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("has_next")]
    public bool HasNext { get; set; }
}