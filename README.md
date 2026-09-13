# berget-ai-csharp
[![test](https://github.com/herwineric/berget-ai-csharp/actions/workflows/test.yml/badge.svg)](https://github.com/herwineric/berget-ai-csharp/actions/workflows/test.yml)
An innofficial client for the Berget.AI api used for .NET

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
