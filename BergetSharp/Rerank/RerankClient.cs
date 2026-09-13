using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json.Serialization;
using BergetSharp.Internal;
using BergetSharp.Model.Rerank;
using OpenAI;

namespace BergetSharp.Rerank;

/// <summary>
/// Client for the Berget rerank endpoint (<c>/v1/rerank</c>), which sorts documents
/// by relevance to a query. The endpoint is compatible with the Cohere rerank API format.
/// </summary>
public class RerankClient
{
    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });

    private readonly string? _model;
    private readonly Uri _endpoint;

    /// <summary>
    /// The pipeline used by this client instance to send and receive requests.
    /// </summary>
    public ClientPipeline Pipeline { get; }

    /// <summary>
    /// The base service endpoint the client sends requests to.
    /// </summary>
    public Uri Endpoint => _endpoint;

    /// <summary>
    /// The rerank model used by this client, or null when no model is set.
    /// </summary>
    public string? Model => _model;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RerankClient"/> class.
    /// </summary>
    /// <param name="model"> The default model to use for rerank requests. Optional; when null or empty, requests without a model fall back to the service default. </param>
    /// <param name="credential"> The API credential used to authenticate with the Berget service. </param>
    /// <param name="options"> Additional client options applied to the underlying pipeline. </param>
    public RerankClient(string? model, ApiKeyCredential credential, OpenAIClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(credential);
        
        _model = string.IsNullOrWhiteSpace(model) ? null : model;
        _endpoint = options.Endpoint;
        Pipeline = ClientPipeline.Create(
            options,
            perCallPolicies: [],
            perTryPolicies: [ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(credential, "Authorization", "Bearer")],
            beforeTransportPolicies: []);
    }

    internal RerankClient(ClientPipeline pipeline, string? model, OpenAIClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        _model = string.IsNullOrWhiteSpace(model) ? null : model;
        _endpoint = options.Endpoint;
        Pipeline = pipeline;
    }

    /// <summary>
    /// Reranks a list of documents by relevance to the query.
    /// </summary>
    /// <param name="query"> The search query. </param>
    /// <param name="documents"> The documents to rerank. </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual ClientResult<RerankResponse> Rerank(string query, IEnumerable<string> documents, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        ArgumentNullException.ThrowIfNull(documents);

        RerankRequest request = new()
        {
            Query = query,
            Documents = documents.ToList(),
            
        };

        return Rerank(request, cancellationToken);
    }

    /// <summary>
    /// Reranks a list of documents by relevance to the query.
    /// </summary>
    /// <param name="query"> The search query. </param>
    /// <param name="documents"> The documents to rerank. </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual async Task<ClientResult<RerankResponse>> RerankAsync(string query, IEnumerable<string> documents, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        ArgumentNullException.ThrowIfNull(documents);

        RerankRequest request = new()
        {
            Query = query,
            Documents = documents.ToList(),
            Model = _model
        };

        return await RerankAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reranks documents using an explicitly created request.
    /// </summary>
    /// <param name="request"> The rerank request to send. </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual ClientResult<RerankResponse> Rerank(RerankRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        RequestOptions? options = cancellationToken.ToRequestOptions();
        using BinaryContent content = CreateBinaryContent(request);
        using PipelineMessage message = CreateRerankRequest(content, options);
        PipelineResponse response = Pipeline.ProcessMessage(message, options);

        return ClientResult.FromValue(JsonUtilities.FromResponse<RerankResponse>(response), response);
    }

    /// <summary>
    /// Reranks documents using an explicitly created request.
    /// </summary>
    /// <param name="request"> The rerank request to send. </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual async Task<ClientResult<RerankResponse>> RerankAsync(RerankRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        RequestOptions? options = cancellationToken.ToRequestOptions();
        using BinaryContent content = CreateBinaryContent(request);
        using PipelineMessage message = CreateRerankRequest(content, options);
        PipelineResponse response = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);

        return ClientResult.FromValue(JsonUtilities.FromResponse<RerankResponse>(response), response);
    }

    /// <summary>
    /// [Protocol Method] Reranks documents using raw request content.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options for configuring the response behavior. </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual ClientResult Rerank(BinaryContent content, RequestOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        using PipelineMessage message = CreateRerankRequest(content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Reranks documents using raw request content.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options for configuring the response behavior. </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual async Task<ClientResult> RerankAsync(BinaryContent content, RequestOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        using PipelineMessage message = CreateRerankRequest(content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    internal virtual PipelineMessage CreateRerankRequest(BinaryContent content, RequestOptions? options)
    {
        PipelineMessage message = Pipeline.CreateMessage(ClientPipelineExtensions.BuildUri(_endpoint, "/rerank"), "POST", PipelineMessageClassifier200);
        PipelineRequest request = message.Request;
        request.Headers.Set("Accept", "application/json");
        request.Headers.Set("Content-Type", "application/json");
        request.Content = content;
        message.Apply(options);
        return message;
    }

    private BinaryContent CreateBinaryContent(RerankRequest request)
    {
        if (string.IsNullOrEmpty(request.Model) && _model is { Length: > 0 })
        {
            request.Model = _model;
        }

        return BinaryContent.CreateJson(request, JsonUtilities.s_options);
    }
}
