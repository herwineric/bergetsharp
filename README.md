# berget-ai-csharp
[![test](https://github.com/herwineric/berget-ai-csharp/actions/workflows/test.yml/badge.svg)](https://github.com/herwineric/berget-ai-csharp/actions/workflows/test.yml)

An innofficial client for the Berget.AI api used for .NET

This clients only dependency is the [OpenAI](https://www.nuget.org/packages/OpenAI/#using-the-client-library) package and will work out of the box with existing OpenAI SDK implementations but with the extending endpoints from [BergetAI](https://berget.ai/)


## Installation

```bash
dotnet add package BergetSharp
```

## Usage

`BergetClient` extends the official `OpenAIClient`, so it defaults to `https://api.berget.ai/v1` and gives you chat, embeddings and audio out of the box.

```csharp
using System.ClientModel;
using BergetSharp;

var apiKey = Environment.GetEnvironmentVariable("BERGET_API_KEY")!;
var client = new BergetClient(new ApiKeyCredential(apiKey));
```

### Chat

```csharp
using OpenAI.Chat;

ChatClient chat = client.GetChatClient("your-chat-model");

ChatCompletion completion = await chat.CompleteChatAsync("Vad är huvudstaden i Sverige?");
Console.WriteLine(completion.Content[0].Text);

// Streaming
await foreach (ChatCompletionUpdate update in chat.CompleteChatStreamingAsync("Skriv en haiku om Stockholm."))
{
    Console.Write(update.ContentUpdate);
}
```

### Embeddings

```csharp
using OpenAI.Embeddings;

EmbeddingClient embeddings = client.GetEmbeddingClient("your-embedding-model");

OpenAIEmbedding embedding = await embeddings.GenerateEmbeddingAsync("Stockholm är huvudstaden i Sverige");
Console.WriteLine(embedding.Vector.Length);
```

### Audio transcription

```csharp
using OpenAI.Audio;

AudioClient audio = client.GetAudioClient("your-transcription-model");

await using FileStream stream = File.OpenRead("audio.mp3");
AudioTranscription transcription = await audio.TranscribeAudioAsync(stream, "audio.mp3");
Console.WriteLine(transcription.Text);
```

### Rerank

The rerank endpoint is Cohere-compatible (`/v1/rerank`) and exposed as a sub-client.

```csharp
using System.ClientModel;
using BergetSharp.Model.Rerank;
using BergetSharp.Rerank;

RerankClient rerank = client.GetRerankClient("BAAI/bge-reranker-v2-m3");

ClientResult<RerankResponse> result = await rerank.RerankAsync(
    "what is the capital of sweden?",
    new List<string>
    {
        "capital of sweden is Stockholm",
        "capital of Norway is Oslo",
        "An apple is a fruit",
    });

foreach (RerankResult item in result.Value.Results)
{
    Console.WriteLine($"{item.Index}: {item.RelevanceScore:F4}");
}
```

### System One

The system one endpoint (`/v1/systemone`, TypeSafe Jev-compatible) answers typed questions
(`noul`, `choice`, `score`) about an application state and returns a structured judgement per
question. Requests are validated client-side (required state, 1-64 questions, per-type criteria rules).

```csharp
using System.ClientModel;
using BergetSharp.Model;
using BergetSharp.Model.SystemOne;
using BergetSharp.SystemOne;

SystemOneClient systemOne = client.GetSystemOneClient(Models.SystemOne.Qwen3_5_2B);

var questions = new Dictionary<string, SystemOneQuestion>
{
    ["route"] = SystemOneQuestion.Choice("Which team should handle this?", new Dictionary<string, string?>
    {
        ["billing"] = "Payments",
        ["technical"] = "Bugs",
    }),
    ["refund"] = SystemOneQuestion.Noul("Is the customer asking for money back?"),
    ["urgency"] = SystemOneQuestion.Score("How urgent is this?", new[] { "Low", "Rising", "Critical" }),
};

ClientResult<SystemOneResponse> result = await systemOne.SystemOneAsync(
    "I was charged twice for my subscription.", questions);

foreach ((string id, SystemOneAnswer answer) in result.Value.Answers)
{
    Console.WriteLine($"{id} ({answer.Type})");
}
```

## Maintenance

This package is maintained by a single contributor with help from automation:

- **CI**: builds the solution and runs the integration suite against the live Berget API on every push/PR to `main` (`.github/workflows/test.yml`).
- **Model drift check**: a weekly scheduled workflow (`.github/workflows/model-drift-check.yml`) compares the model IDs in `BergetSharp/Model/Models.cs` against the live `/v1/models` endpoint and opens a PR with synced IDs when they drift.
- **Dependencies**: Dependabot keeps NuGet packages and GitHub Actions up to date weekly.
- **Releases**: manual. The `publish` workflow (`.github/workflows/publish.yml`) reads the version from `<VersionPrefix>` in `BergetSharp.csproj`, tags the commit `v<version>`, and pushes the package to nuget.org (Trusted Publishing) together with a GitHub release.


