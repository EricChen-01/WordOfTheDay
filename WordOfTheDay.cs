using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WordOfTheDay.Demo;

public class WordOfTheDay
{
    private readonly ILogger<WordOfTheDay> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public string jlptVocabApi = "https://jlpt-vocab-api.vercel.app/api/words/random";

    public WordOfTheDay(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
    {
        _logger = loggerFactory.CreateLogger<WordOfTheDay>();
        _httpClientFactory = httpClientFactory;
    }

    [Function("WordOfTheDay")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("Http Example function processing a request.");
        var client = _httpClientFactory.CreateClient();
        var json = await client.GetStringAsync(jlptVocabApi);

        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }
}