using System.ClientModel;
using BergetSharp.Rerank;
using OpenAI;

namespace BergetSharp;

/// <summary>
/// OpenAI compatible Client for the Berget Inference API. There are subclients for each endpoint:
/// chat completions, embeddings, transcriptions, rerank, and seat budgets.
/// </summary>
public class BergetClient : OpenAIClient
{
    private readonly OpenAIClientOptions _options;
    
    private static Uri DefaultEndpoint => new Uri("https://api.berget.ai/v1");

    /// <summary>
    /// Initializes a new instance of the <see cref="BergetClient"/> class.
    /// </summary>
    /// <param name="credential"> The API credential used to authenticate with the Berget service. </param>
    /// <param name="options"> Additional client options applied to the underlying pipeline. </param>
    public BergetClient(ApiKeyCredential credential, OpenAIClientOptions? options = null)
        : base(credential, options ?? new OpenAIClientOptions { Endpoint = DefaultEndpoint})
    {
        _options = options ?? new OpenAIClientOptions { Endpoint = DefaultEndpoint};
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BergetClient"/> class.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="credential"></param>
    /// <param name="options"></param>
    public BergetClient(Uri endpoint, ApiKeyCredential credential, OpenAIClientOptions? options = null)
        : this(credential, options ?? new OpenAIClientOptions { Endpoint = endpoint })
    {
        
    }

    /// <summary>
    /// Gets a new instance of the <see cref="RerankClient"/> class for the /v1/rerank endpoint,
    /// sharing this client's pipeline and endpoint.
    /// </summary>
    /// <param name="model"> The default model to use for rerank requests. </param>
    public virtual RerankClient GetRerankClient(string model)
        => new(Pipeline, model, _options);
    
}
