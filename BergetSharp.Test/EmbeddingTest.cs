using System.ClientModel;
using BergetSharp.Model;
using OpenAI;
using OpenAI.Embeddings;

namespace BergetSharp.Test;

public class EmbeddingTest
{
    [Test]
    public async Task GenerateEmbeddings_MultipleInputs_Test()
    {
        var credentials = new ApiKeyCredential(Environment.GetEnvironmentVariable("BERGET_API_KEY") ?? "");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };

        var client = new BergetClient(credentials, options);
        var embeddingClient = client.GetEmbeddingClient(Models.Embedding.MultilingualE5Large);

        List<string> inputs = ["what is the capital of sweden?", "capital of Norway is Oslo", "An apple is a fruit"];

        var result = await embeddingClient.GenerateEmbeddingsAsync(inputs);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value.Count).IsEqualTo(inputs.Count);
        await Assert.That(result.Value[0].ToFloats().Length).IsGreaterThan(0);
    }
}
