using System.ClientModel;
using OpenAI;

namespace BergetSharp.Test;

public class RerankTest
{
    [Fact]
    public void Rerank_Top3_Test()
    {
        var credentials = new ApiKeyCredential("");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };

        var client = new BergetClient(credentials, options);
        var rerankClient = client.GetRerankClient("BAAI/bge-reranker-v2-m3");

        var query = "what is the capital of sweden?";
        List<string> documents = ["capital of sweden is Stockholm", "capital of Norway is Oslo", "An apple is a fruit"];
        
        var response = rerankClient.Rerank(query, documents);
        
        Assert.True(response.Value.Results.Count == documents.Count);
    }
}