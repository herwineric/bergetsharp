using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Model.SystemOne;

/// <summary>
/// Response of the System One endpoint (<c>/v1/systemone</c>): a structured judgement
/// for every question in the request.
/// </summary>
public sealed class SystemOneResponse
{
    /// <summary>
    /// Public model id that answered the questions.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The answers, keyed by the question id used in the request.
    /// </summary>
    [JsonPropertyName("answers")]
    public Dictionary<string, SystemOneAnswer> Answers { get; set; } = new();

    /// <summary>
    /// Token usage for this request.
    /// </summary>
    [JsonPropertyName("usage")]
    public SystemOneUsage? Usage { get; set; }

    /// <summary>
    /// Additional fields in the response payload that are not mapped to a strongly typed property.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
