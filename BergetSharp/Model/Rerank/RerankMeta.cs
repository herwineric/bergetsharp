using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankMeta
{
    /// <summary>
    /// API version information for the request.
    /// </summary>
    [JsonPropertyName("api_version")]
    public RerankApiVersion? ApiVersion { get; set; }

    /// <summary>
    /// Billable units consumed by the request.
    /// </summary>
    [JsonPropertyName("billed_units")]
    public RerankBilledUnits? BilledUnits { get; set; }

    /// <summary>
    /// Token counts for the request.
    /// </summary>
    [JsonPropertyName("tokens")]
    public RerankTokens? Tokens { get; set; }

    /// <summary>
    /// Number of tokens served from cache, when reported by the provider.
    /// </summary>
    [JsonPropertyName("cached_tokens")]
    public double? CachedTokens { get; set; }
}
