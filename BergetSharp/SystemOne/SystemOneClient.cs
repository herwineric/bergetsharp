using System.ClientModel;
using System.ClientModel.Primitives;
using BergetSharp.Internal;
using BergetSharp.Model.SystemOne;
using OpenAI;

namespace BergetSharp.SystemOne;

/// <summary>
/// Client for the Berget System One endpoint (<c>/v1/systemone</c>), which answers typed questions
/// (<c>noul</c>, <c>choice</c>, <c>score</c>) about an application state and returns a structured
/// judgement per question — probabilities and confidence, never prose. Compatible with the
/// TypeSafe Jev <c>/v1/systemone</c> API.
/// </summary>
public class SystemOneClient
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
    /// The system one model used by this client, or null when no model is set.
    /// </summary>
    public string? Model => _model;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemOneClient"/> class.
    /// </summary>
    /// <param name="model"> The default model to use for system one requests, e.g. <c>Qwen/Qwen3.5-2B</c> or <c>convaiinnovations/laya</c> (aliases such as <c>systemone</c> and <c>laya-latest</c> are also served). Optional; when null or empty, requests without a model fall back to the service default. </param>
    /// <param name="credential"> The API credential used to authenticate with the Berget service. </param>
    /// <param name="options"> Additional client options applied to the underlying pipeline. </param>
    public SystemOneClient(string? model, ApiKeyCredential credential, OpenAIClientOptions options)
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

    internal SystemOneClient(ClientPipeline pipeline, string? model, OpenAIClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        _model = string.IsNullOrWhiteSpace(model) ? null : model;
        _endpoint = options.Endpoint;
        Pipeline = pipeline;
    }

    /// <summary>
    /// Answers every question about the given application state in a single pass.
    /// </summary>
    /// <param name="state"> The application state the questions are asked about; a string, an object or an array. </param>
    /// <param name="questions"> The questions to answer, keyed by a caller-chosen id (1-64 questions). </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual ClientResult<SystemOneResponse> SystemOne(object state, IDictionary<string, SystemOneQuestion> questions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(questions);

        SystemOneRequest request = new()
        {
            State = state,
            Questions = new Dictionary<string, SystemOneQuestion>(questions),
        };

        return SystemOne(request, cancellationToken);
    }

    /// <summary>
    /// Answers every question about the given application state in a single pass.
    /// </summary>
    /// <param name="state"> The application state the questions are asked about; a string, an object or an array. </param>
    /// <param name="questions"> The questions to answer, keyed by a caller-chosen id (1-64 questions). </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual async Task<ClientResult<SystemOneResponse>> SystemOneAsync(object state, IDictionary<string, SystemOneQuestion> questions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(questions);

        SystemOneRequest request = new()
        {
            State = state,
            Questions = new Dictionary<string, SystemOneQuestion>(questions),
        };

        return await SystemOneAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Answers questions using an explicitly created request. The request is validated against the
    /// endpoint rules (required state, 1-64 questions, per-question type and criteria rules) before
    /// it is sent.
    /// </summary>
    /// <param name="request"> The system one request to send. </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual ClientResult<SystemOneResponse> SystemOne(SystemOneRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Validate();

        RequestOptions? options = cancellationToken.ToRequestOptions();
        using BinaryContent content = CreateBinaryContent(request);
        using PipelineMessage message = CreateSystemOneRequest(content, options);
        PipelineResponse response = Pipeline.ProcessMessage(message, options);

        return ClientResult.FromValue(JsonUtilities.FromResponse<SystemOneResponse>(response), response);
    }

    /// <summary>
    /// Answers questions using an explicitly created request. The request is validated against the
    /// endpoint rules (required state, 1-64 questions, per-question type and criteria rules) before
    /// it is sent.
    /// </summary>
    /// <param name="request"> The system one request to send. </param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual async Task<ClientResult<SystemOneResponse>> SystemOneAsync(SystemOneRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Validate();

        RequestOptions? options = cancellationToken.ToRequestOptions();
        using BinaryContent content = CreateBinaryContent(request);
        using PipelineMessage message = CreateSystemOneRequest(content, options);
        PipelineResponse response = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);

        return ClientResult.FromValue(JsonUtilities.FromResponse<SystemOneResponse>(response), response);
    }

    /// <summary>
    /// [Protocol Method] Answers questions using raw request content.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options for configuring the response behavior. </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual ClientResult SystemOne(BinaryContent content, RequestOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        using PipelineMessage message = CreateSystemOneRequest(content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Answers questions using raw request content.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options for configuring the response behavior. </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ClientResultException"></exception>
    public virtual async Task<ClientResult> SystemOneAsync(BinaryContent content, RequestOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        using PipelineMessage message = CreateSystemOneRequest(content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    internal virtual PipelineMessage CreateSystemOneRequest(BinaryContent content, RequestOptions? options)
    {
        PipelineMessage message = Pipeline.CreateMessage(ClientPipelineExtensions.BuildUri(_endpoint, "/systemone"), "POST", PipelineMessageClassifier200);
        PipelineRequest request = message.Request;
        request.Headers.Set("Accept", "application/json");
        request.Headers.Set("Content-Type", "application/json");
        request.Content = content;
        message.Apply(options);
        return message;
    }

    private BinaryContent CreateBinaryContent(SystemOneRequest request)
    {
        if (string.IsNullOrEmpty(request.Model) && _model is { Length: > 0 })
        {
            request.Model = _model;
        }

        return BinaryContent.CreateJson(request, JsonUtilities.s_options);
    }
}
