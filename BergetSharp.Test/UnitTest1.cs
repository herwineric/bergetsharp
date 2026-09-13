using System.ClientModel;
using OpenAI;

namespace BergetSharp.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var credentials = new ApiKeyCredential("");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };
        
        var client = new BergetClient(credentials, options);
        
        //client.GetAudioClient("").
    }
}