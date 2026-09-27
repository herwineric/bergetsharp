using System.Text.Json.Serialization;

namespace BergetSharp.Model.SystemOne;

/// <summary>
/// Token usage for a System One request.
/// </summary>
public sealed class SystemOneUsage
{
    /// <summary>
    /// Number of input tokens consumed by the request.
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public long InputTokens { get; set; }

    /// <summary>
    /// Number of output tokens produced by the request.
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public long OutputTokens { get; set; }
}
