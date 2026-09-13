using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankResult
{
    /// <summary>
    /// Index into the original list of documents.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// Relevance score (0-1).
    /// </summary>
    [JsonPropertyName("relevance_score")]
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Echo of the input document, present only when return_documents is true.
    /// </summary>
    [JsonPropertyName("document")]
    public object? Document { get; set; }
}
