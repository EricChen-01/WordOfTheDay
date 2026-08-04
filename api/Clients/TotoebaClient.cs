using Newtonsoft.Json;
using WordOfTheDay.Models;

namespace WordOfTheDay.Clients;

public sealed class TatoebaClient
{
    private readonly HttpClient _httpClient;
    private readonly string apiUrl = "https://api.tatoeba.org/v1/sentences";

    public TatoebaClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TatoebaSentence>> GetExampleSentencesAsync(string word, int count = 2)
    {
        // Note: "trans:lang" contains a colon, so it must be percent-encoded (%3A) in the raw query string.
        var query = $"?lang=jpn" +
                    $"&q={Uri.EscapeDataString(word)}" +
                    $"&trans%3Alang=eng" +
                    $"&sort=relevance" + 
                    $"&limit={count}";

        var response = await _httpClient.GetAsync($"{apiUrl}{query}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<TatoebaSentenceResponse>(content);

        return result?.Data.Take(count).ToList() ?? [];
    }
}