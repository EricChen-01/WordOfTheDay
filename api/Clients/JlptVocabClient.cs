
using Newtonsoft.Json;
using WordOfTheDay.Models;

namespace WordOfTheDay.Clients;

public sealed class JlptVocabClient
{
    private readonly string apiUrl = "https://jlpt-vocab-api.vercel.app/api/words/random";
    private readonly HttpClient _httpClient;

    public JlptVocabClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JlptWord?> GetRandomWordAsync()
    {
        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<JlptWord>(content);
    }
}