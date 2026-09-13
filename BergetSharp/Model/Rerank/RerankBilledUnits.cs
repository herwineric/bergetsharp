using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankBilledUnits
{
    /// <summary>
    /// Number of images billed for the request.
    /// </summary>
    [JsonPropertyName("images")]
    public double? Images { get; set; }

    /// <summary>
    /// Number of input tokens billed for the request.
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public double? InputTokens { get; set; }

    /// <summary>
    /// Number of image tokens billed for the request.
    /// </summary>
    [JsonPropertyName("image_tokens")]
    public double? ImageTokens { get; set; }

    /// <summary>
    /// Number of output tokens billed for the request.
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public double? OutputTokens { get; set; }

    /// <summary>
    /// Number of search units billed for the request.
    /// </summary>
    [JsonPropertyName("search_units")]
    public double? SearchUnits { get; set; }

    /// <summary>
    /// Number of classifications billed for the request.
    /// </summary>
    [JsonPropertyName("classifications")]
    public double? Classifications { get; set; }
}
