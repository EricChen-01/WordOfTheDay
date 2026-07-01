# Word of the Day — API

Azure Functions (.NET, isolated worker) backend that picks a random JLPT
vocab word once a day, enriches it with a definition/reading from Jisho,
and serves it over HTTP.

## How it works

Two functions, one table:

```
23:55 UTC daily            on-demand
       │                        │
       ▼                        ▼
WordOfTheDayDailyTrigger*   WordOfTheDay (HTTP GET/POST)
  - calls JLPT Vocab API     - reads today's word from Table Storage
  - picks a random word      - calls Jisho for more info
  - stores it in Table       - returns 
```

*The timer fires at 23:55 UTC and writes the entity under
`RowKey = UtcNow.AddDays(1)` — i.e. it pre-generates the *next* day's word
the night before, so it's already sitting in the table by the time anyone
requests it. `WordOfTheDay.cs` currently looks up `RowKey = UtcNow` (today,
server time).

Table Storage is just a lookup cache — the "word of the day" work (picking +
enriching the word) all happens in `WordOfTheDay.cs` at request time, not
in the timer. The timer only picks the word.

## Project structure
 
| File / Folder | Description |
|---|---|
| `WordOfTheDay.cs` | The HTTP-triggered endpoint |
| `WordOfTheDayDailyTrigger.cs` | The timer-triggered word picker |
| `Clients/JlptVocabClient.cs` | GET random word from jlpt-vocab-api.vercel.app |
| `Clients/JishoClient.cs` | GET word details from jisho.org |
| `Models/JlptVocabWord.cs` | JLPT Vocab API response shape |
| `Models/JishoKeywordResponse.cs` | Jisho API response shape |
| `Models/JishoData.cs`, `Models/JishoJapanese.cs`, `Models/JishoSense.cs`, `Models/JishoMeta.cs` | Jisho API nested response shapes |
| `Models/WordOfTheDayEntity.cs` | Table Storage entity (PartitionKey `"wordoftheday"`, RowKey = date, `Word` = word) |
| `Models/WordOfTheDayResponse.cs` | What the HTTP endpoint actually returns |
| `Program.cs` | DI setup: HttpClients, TableClient, OpenTelemetry |
| `host.json` | Just enables OpenTelemetry. I don' think this is being used. |

## Configuration

Set these as Function App settings in Azure, or in `local.settings.json` for local dev:

| Setting | Required | Notes |
|---|---|---|
| `TABLESTORE_CONNECTIONURL` | Yes | Full URL to the storage account table endpoint. Auth is via `DefaultAzureCredential` (managed identity in Azure, your `az login` / Visual Studio credential locally) — **not** a connection string. |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | No | If set, OpenTelemetry exports to Azure Monitor. If unset, telemetry is skipped entirely (see the `if` in `Program.cs`). |

`local.settings.json` template:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "TABLESTORE_CONNECTIONURL": "https://<your-account>.table.core.windows.net"
  }
}
```

Table name is hardcoded in `Program.cs`: **`WordOfTheDayTable`**.

## Running locally

1. Start Azurite (or point `TABLESTORE_CONNECTIONURL` at a real dev storage account — `DefaultAzureCredential` won't authenticate against Azurite's table endpoint the same way a connection string would, so a real storage account is easier for local testing). (F1 and type in Azurite start)
2. `func start` from the `api/` folder. (F5)
3. Manually trigger the timer once (or insert a row by hand) so there's a word for today — the HTTP function will 404 otherwise.
4. `GET http://localhost:7071/api/WordOfTheDay`

## API

**`GET /api/WordOfTheDay`** — Anonymous auth.

Success (200):
```json
{
  "word": "食べる",
  "meaning": "to eat",
  "furigana": "たべる",
  "level": "N5",
  "partsOfSpeech": ["Ichidan verb", "transitive verb"]
}
```

| Status | Meaning |
|---|---|
| 404 | No word stored for today's date (timer hasn't run, or table's empty) |
| 500 or somiliar | Unhandled exception (see logs); probably an auth issue. |

## Known gaps / things to revisit

- **Timezone:** `WordOfTheDay.cs` uses `DateTime.UtcNow` server-side. Future improvement is to accept a date query.
- **Response serialization:** `WordOfTheDayResponse` is annotated with
  Newtonsoft `[JsonProperty]` attributes, but `OkObjectResult` is
  serialized by ASP.NET Core's default output formatter, which is
  System.Text.Json unless configured otherwise — Newtonsoft attributes
  won't apply there. **Worth double-checking what the actual response
  casing looks like** (`Word` vs `word`) before assuming the frontend's
  field names line up.
- **No retry/backoff** on the Jisho or JLPT Vocab calls — a transient
  failure on either just surfaces as a 502/failed timer run.
- **No dedupe** — the timer can pick the same word on two different days
  since `GetRandomWordAsync()` has no memory of past words.
- Both external APIs (Jisho, JLPT Vocab) are free/unofficial with no SLA —
  worth keeping an eye on if either goes down or changes shape.