using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WordOfTheDay.Clients;
using WordOfTheDay.Models;
using Azure.Data.Tables;

namespace WordOfTheDay.Functions;

public class WordOfTheDay
{
    private readonly ILogger<WordOfTheDay> _logger;
    private readonly JlptVocabClient _jlptVocabClient;
    private readonly JishoClient _jishoClient;
    private readonly TableClient _tableClient;

    public WordOfTheDay(ILoggerFactory loggerFactory, JlptVocabClient jlptVocabClient, JishoClient jishoClient, TableClient tableClient)
    {
        _logger = loggerFactory.CreateLogger<WordOfTheDay>();
        _jlptVocabClient = jlptVocabClient;
        _jishoClient = jishoClient;
        _tableClient = tableClient;
    }

    /// <summary>
    /// This function is triggered by an HTTP request and returns the word of the day.
    /// </summary>
    [Function("WordOfTheDay")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        _logger.LogInformation("Retrieving word of the day for date: {CurrentDate}", currentDate);
        var wordOfTheDayEntity = await _tableClient.GetEntityIfExistsAsync<WordOfTheDayEntity>("wordoftheday", currentDate);
        if (!wordOfTheDayEntity.HasValue)
        {
            _logger.LogError("No word of the day found for today.");
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }
        var wordOfTheDay = wordOfTheDayEntity.Value.Word;
        
        _logger.LogInformation("Retrieved word of the day: {Word}", wordOfTheDayEntity.Value.Word);
        
        var jishoResponse = await _jishoClient.GetWordAsync(wordOfTheDay);
        if (jishoResponse is null || jishoResponse.Data.Count == 0 || jishoResponse.Meta.Status is not 200)
        {
            _logger.LogError("Failed to retrieve word details from the Jisho API for word: {Word}", wordOfTheDay);
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        var jishoData = jishoResponse.Data.FirstOrDefault();
        if (jishoData is null)
        {
            _logger.LogError("No data found for word: {Word} in Jisho API response.", wordOfTheDay);
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        var firstSense = jishoData.Senses.FirstOrDefault();
        if (firstSense is null)
        {
            _logger.LogError("No senses found for word: {Word} in Jisho API response.", wordOfTheDay);
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        var response = new WordOfTheDayResponse
        {
            Word = wordOfTheDay,
            Meaning = string.Join(", ", firstSense.EnglishDefinitions),
            Furigana = jishoData.Japanese.FirstOrDefault()?.Reading ?? "Not Available. Please contact support.",
            Level = jishoData.Jlpt.FirstOrDefault() ?? "Not Available. Please contact support.",
            PartsOfSpeech = firstSense.PartsOfSpeech 
        };

        return new OkObjectResult(response);
    }
}