using System.ClientModel;
using BergetSharp.Model;
using OpenAI;

namespace BergetSharp.Test;

public class RerankTest
{
    [Test]
    public async Task Rerank_Top3_Test()
    {
        var credentials = new ApiKeyCredential(Environment.GetEnvironmentVariable("BERGET_API_KEY") ?? "");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };

        var client = new BergetClient(credentials, options);
        var rerankClient = client.GetRerankClient(Models.Rerank.BgeRerankerV2M3);

        var query = "what is the capital of sweden?";
        List<string> documents = ["capital of sweden is Stockholm", "capital of Norway is Oslo", "An apple is a fruit"];
        
        var response = rerankClient.Rerank(query, documents);
        
        await Assert.That(response.Value.Results.Count == documents.Count).IsTrue();
    }
}