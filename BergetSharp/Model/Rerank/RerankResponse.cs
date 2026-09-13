using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankResponse
{
    /// <summary>
    /// Unique identifier for the rerank operation.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Canonical model ID.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Object type discriminator (always "reranking").
    /// </summary>
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    /// <summary>
    /// An ordered list of ranked documents.
    /// </summary>
    [JsonPropertyName("results")]
    public List<RerankResult> Results { get; set; } = new();

    /// <summary>
    /// Request metadata. Not all fields are populated by upstream providers.
    /// </summary>
    [JsonPropertyName("meta")]
    public RerankMeta? Meta { get; set; }

    /// <summary>
    /// Token usage (Berget/upstream extension; not part of the Cohere v1 spec).
    /// </summary>
    [JsonPropertyName("usage")]
    public RerankUsage? Usage { get; set; }

    /// <summary>
    /// Additional fields in the response payload that are not mapped to a strongly typed property.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
