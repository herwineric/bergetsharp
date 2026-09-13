using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankTokens
{
    /// <summary>
    /// Number of input tokens processed, when reported by the provider.
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public double? InputTokens { get; set; }

    /// <summary>
    /// Number of output tokens generated, when reported by the provider.
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public double? OutputTokens { get; set; }
}
