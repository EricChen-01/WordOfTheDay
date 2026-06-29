using Azure.Data.Tables;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using WordOfTheDay.Clients;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<JlptVocabClient>();
builder.Services.AddHttpClient<JishoClient>();

builder.Services.AddSingleton(_ =>
{
    var tableUri = Environment.GetEnvironmentVariable("TABLESTORE_CONNECTIONURL")
        ?? throw new InvalidOperationException("TABLESTORE_CONNECTIONURL is not configured.");

    return new TableClient(new Uri(tableUri), "WordOfTheDayTable", new DefaultAzureCredential());
});

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
