using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WordOfTheDay.Clients;
using WordOfTheDay.Models;

namespace WordOfTheDay.Demo;

public class WordOfTheDay
{
    private readonly ILogger<WordOfTheDay> _logger;
    private readonly JlptVocabClient _jlptVocabClient;
    private readonly JishoClient _jishoClient;

    public WordOfTheDay(ILoggerFactory loggerFactory, JlptVocabClient jlptVocabClient, JishoClient jishoClient)
    {
        _logger = loggerFactory.CreateLogger<WordOfTheDay>();
        _jlptVocabClient = jlptVocabClient;
        _jishoClient = jishoClient;
    }

    [Function("WordOfTheDay")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var randomWord = await _jlptVocabClient.GetRandomWordAsync();
        if (randomWord is null)
        {
            _logger.LogError("Failed to retrieve a random word from the JLPT Vocab API.");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        var jishoResponse = await _jishoClient.GetWordAsync(randomWord.Word);
        if (jishoResponse is null || jishoResponse.Data.Count == 0 || jishoResponse.Meta.Status is not 200)
        {
            _logger.LogError("Failed to retrieve word details from the Jisho API for word: {Word}", randomWord.Word);
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        var jishoData = jishoResponse.Data.FirstOrDefault();
        if (jishoData is null)
        {
            _logger.LogError("No data found for word: {Word} in Jisho API response.", randomWord.Word);
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        var firstSense = jishoData.Senses.FirstOrDefault();
        if (firstSense is null)
        {
            _logger.LogError("No senses found for word: {Word} in Jisho API response.", randomWord.Word);
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        var response = new WordOfTheDayResponse
        {
            Word = randomWord.Word,
            Meaning = randomWord.Meaning,
            Furigana = string.IsNullOrWhiteSpace(randomWord.Furigana) ? randomWord.Word : randomWord.Furigana,
            Romaji = randomWord.Romaji,
            Level = randomWord.Level,
            PartsOfSpeech = firstSense.PartsOfSpeech 
        };

        return new OkObjectResult(response);
    }
}