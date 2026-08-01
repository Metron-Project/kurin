# Metron.Api

An C# client library for the [Metron](https://metron.cloud) comic book database API,
targeting .NET 10.

- Covers the full API surface: arcs, characters, creators, credits, imprints, issues,
  publishers, series, series types, teams, universes, variants, roles, plus the
  authenticated-user resources (collection, pull list, reading list, wish list).
- Implements the dual rate limits (burst + sustained) described in
  [Metron's `RATELIMIT.md`](https://github.com/Metron-Project/metron/blob/master/api/RATELIMIT.md):
  tracks the `X-RateLimit-*` response headers, throttles proactively before either counter would
  be exhausted, and honors `Retry-After` on a `429` with bounded retries.
- Authenticates with a bearer API token.

## Installation

Reference the project directly, or build and pack it:

```bash
dotnet pack src/Metron.Api/Metron.Api.csproj
```

## Usage

```csharp
using Metron.Api;
using Metron.Api.Filters;

using var client = new MetronClient(new MetronClientOptions
{
    ApiToken = "your-metron-api-token",
});

var arc = await client.Arc.GetAsync(42);

await foreach (var issue in client.Issue.ListAllAsync(new IssueFilter { SeriesId = 7 }))
{
    Console.WriteLine(issue.Number);
}

// Latest known rate-limit snapshot, updated after every request.
Console.WriteLine(client.RateLimitStatus?.BurstRemaining);
```

`MetronClientOptions` also accepts `BaseAddress`, `UserAgent`, `MaxRetryAttempts` (default 3), and
an optional `TransportHandler` for tests or custom `HttpMessageHandler` pipelines.

### Writes

All create/update/partial-update calls are sent as `multipart/form-data`, since several Metron
resources (arc, character, creator, imprint, publisher, team, universe) don't accept a JSON body:

```csharp
var created = await client.Arc.CreateAsync(new Arc { Name = "Infinity Gauntlet" });
await client.Arc.PartialUpdateAsync(created.Id!.Value, new PatchedArc { Desc = "..." });
```

### Errors

Non-success responses throw `MetronApiException` (status code + the API's `detail` message). A
request still throttled after `MaxRetryAttempts` retries throws `MetronRateLimitException`
(carries `RetryAfter` and the last known `RateLimitStatus`).

## Project layout

```bash
src/Metron.Api/
  MetronClient.cs, MetronClientOptions.cs   Facade + options
  Http/                                      Rate-limit handler/tracker, multipart & query builders
  Exceptions/                                MetronApiException, MetronRateLimitException
  Models/Generated/, Filters/                Generated from the OpenAPI schema (see below)
  Models/PagedResult.cs, CollectionStats.cs  Hand-written models
  Resources/                                 One client per resource group (Arc, Issue, Series, ...)
tools/codegen/generate_models.py             Regenerates Models/Generated and Filters
tests/Metron.Api.Tests/                      xUnit tests
```

## Regenerating models

`Models/Generated/*.cs` and `Filters/*.cs` are generated from `Metron Comicbook Database.yaml`
(the OpenAPI schema at the repo root) and committed to the repo. Re-run the generator whenever
the schema changes:

```python
python3 tools/codegen/generate_models.py
```

Requires Python 3 with PyYAML (`pip install pyyaml`).

## Running tests

```bash
dotnet test
```

## Known limitations

- `Issue.Price` is typed as `string?`. The schema allows either a plain decimal string (USD) or
  an `{amount, currency}` object for non-USD prices; only the plain-string form is supported
  directly through the typed model.
- Every generated model property is nullable, regardless of the schema's `required` list, so
  read and write DTOs stay interchangeable without constructor boilerplate.
- Retrying a request that carries a non-seekable file `Stream` (e.g. a manually attached image
  upload) after a `429` isn't supported; retries re-read buffered content, which requires
  seekable/re-readable request bodies.
