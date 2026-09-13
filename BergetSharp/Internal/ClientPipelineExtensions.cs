using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace BergetSharp.Internal;

internal static class ClientPipelineExtensions
{
    public static PipelineResponse ProcessMessage(this ClientPipeline pipeline, PipelineMessage message, RequestOptions? options)
    {
        pipeline.Send(message);
        PipelineResponse response = message.Response!;

        if (response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
        {
            throw CreateClientException(response);
        }

        return message.BufferResponse ? response : message.ExtractResponse()!;
    }

    public static async Task<PipelineResponse> ProcessMessageAsync(this ClientPipeline pipeline, PipelineMessage message, RequestOptions? options)
    {
        await pipeline.SendAsync(message).ConfigureAwait(false);
        PipelineResponse response = message.Response!;

        if (response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
        {
            throw CreateClientException(response);
        }

        return message.BufferResponse ? response : message.ExtractResponse()!;
    }

    public static RequestOptions? ToRequestOptions(this CancellationToken cancellationToken)
    {
        return cancellationToken.CanBeCanceled ? new RequestOptions() { CancellationToken = cancellationToken } : null;
    }

    public static Uri BuildUri(Uri endpoint, string path)
    {
        string basePath = endpoint.AbsolutePath.TrimEnd('/');
        return new UriBuilder(endpoint.Scheme, endpoint.Host, endpoint.IsDefaultPort ? -1 : endpoint.Port)
        {
            Path = basePath + path,
        }.Uri;
    }

    private static ClientResultException CreateClientException(PipelineResponse response)
    {
        string? errorMessage = TryGetErrorMessage(response);
        return errorMessage is { Length: > 0 } ? new ClientResultException(errorMessage, response) : new ClientResultException(response);
    }

    private static string? TryGetErrorMessage(PipelineResponse response)
    {
        try
        {
            if (response.Content is { Length: > 0 } content)
            {
                using JsonDocument document = JsonDocument.Parse(content);

                if (document.RootElement.ValueKind == JsonValueKind.Object
                    && document.RootElement.TryGetProperty("error", out JsonElement error)
                    && error.TryGetProperty("message", out JsonElement message)
                    && message.GetString() is { Length: > 0 } text)
                {
                    return text;
                }
            }
        }
        catch (JsonException)
        {
        }

        return null;
    }
}
