using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankRequest
{
    /// <summary>
    /// The identifier of the model to use. Optional in v1, though callers should send it.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// The search query.
    /// </summary>
    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// A list of document objects or strings to rerank.
    /// </summary>
    [JsonPropertyName("documents")]
    public List<object> Documents { get; set; } = new();

    /// <summary>
    /// The number of most relevant documents/indices to return.
    /// Defaults to the length of documents.
    /// </summary>
    [JsonPropertyName("top_n")]
    public int? TopN { get; set; }

    /// <summary>
    /// For object documents, which keys to consider for reranking, in priority order.
    /// Forwarded to the upstream provider when supported.
    /// </summary>
    [JsonPropertyName("rank_fields")]
    public List<string>? RankFields { get; set; }

    /// <summary>
    /// If true, returns document text/objects in results.
    /// Defaults to false per the Cohere v1 spec.
    /// </summary>
    [JsonPropertyName("return_documents")]
    public bool ReturnDocuments { get; set; }

    /// <summary>
    /// The maximum number of chunks to produce internally from a document.
    /// Forwarded to the upstream provider when supported. Defaults to 10.
    /// </summary>
    [JsonPropertyName("max_chunks_per_doc")]
    public int MaxChunksPerDoc { get; set; } = 10;

    /// <summary>
    /// Additional fields in the request payload that are not mapped to a strongly typed property.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
