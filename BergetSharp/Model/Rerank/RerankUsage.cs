using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankUsage
{
    /// <summary>
    /// Number of prompt (input) tokens consumed, when reported by the provider.
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public double? PromptTokens { get; set; }

    /// <summary>
    /// Total number of tokens consumed by the rerank request.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public double TotalTokens { get; set; }
}
