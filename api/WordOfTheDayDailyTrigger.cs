using System;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WordOfTheDay.Clients;
using WordOfTheDay.Models;

namespace WordOfTheDay.Functions;

public class WordOfTheDayDailyTrigger
{
    private readonly ILogger _logger;
    private readonly JlptVocabClient _jlptVocabClient;
    private readonly TableClient _tableClient;

    public WordOfTheDayDailyTrigger(ILoggerFactory loggerFactory, JlptVocabClient jlptVocabClient, TableClient tableClient)
    {
        _logger = loggerFactory.CreateLogger<WordOfTheDayDailyTrigger>();
        _jlptVocabClient = jlptVocabClient;
        _tableClient = tableClient;
    }

    /// <summary>
    /// This function is triggered by a timer and will generate a new word of the day at 11:55 PM UTC everday. It wil store result into a table for later retrieval.
    /// </summary>
    [Function("WordOfTheDayDailyTrigger")]
    public async Task Run([TimerTrigger("0 55 23 * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }

        // get random word from JLPT vocab api client
        var randomWord = await _jlptVocabClient.GetRandomWordAsync();
        if (randomWord is null)
        {
            _logger.LogError("Failed to retrieve a random word from the JLPT Vocab API.");
            return;
        }

        // store the word of the day in Azure Table Storage
        var entity = new WordOfTheDayEntity
        {
            PartitionKey = "wordoftheday",
            RowKey = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
            Word = randomWord.Word // adjust property name to match your JLPT client's response model
        };

        await _tableClient.UpsertEntityAsync(entity);

        _logger.LogInformation("Stored word of the day '{word}' for {date}", entity.Word, entity.RowKey);
    }
}