using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankApiVersion
{
    /// <summary>
    /// Version identifier of the API that served the request.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this API version is deprecated.
    /// </summary>
    [JsonPropertyName("is_deprecated")]
    public bool? IsDeprecated { get; set; }

    /// <summary>
    /// Indicates whether this API version is experimental.
    /// </summary>
    [JsonPropertyName("is_experimental")]
    public bool? IsExperimental { get; set; }
}
