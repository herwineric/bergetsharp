using System.ClientModel;
using OpenAI;

namespace BergetSharp;

public class BergetClient : OpenAIClient
{
    public BergetClient(string apiKey)
        : base(apiKey)
    {
    }

    public BergetClient(ApiKeyCredential credential)
        : base(credential)
    {
    }

    public BergetClient(ApiKeyCredential credential, OpenAIClientOptions options)
        : base(credential, options)
    {
    }
}