
using Newtonsoft.Json;
using WordOfTheDay.Models;

namespace WordOfTheDay.Clients;

public sealed class JishoClient
{
    private readonly HttpClient _httpClient;
    private readonly string apiUrl = "https://jisho.org/api/v1/search/words";

    public JishoClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JishoKeywordResponse?> GetWordAsync(string word)
    {
        var response = await _httpClient.GetAsync($"{apiUrl}?keyword={word}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<JishoKeywordResponse>(content);
    }
}