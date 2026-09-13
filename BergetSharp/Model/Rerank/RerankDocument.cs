using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Model.Rerank;

public sealed class RerankDocument
{
    /// <summary>
    /// The text of the document to rerank.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Additional fields on the document that are not mapped to a strongly typed property.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
