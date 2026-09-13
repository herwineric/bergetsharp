using System.ClientModel.Primitives;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Internal;

internal static class JsonUtilities
{
    internal static readonly JsonSerializerOptions s_options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static T FromResponse<T>(PipelineResponse response) where T : class, new()
    {
        if (response.Content is { Length: > 0 } content)
        {
            using JsonDocument document = JsonDocument.Parse(content);
            return document.Deserialize<T>(s_options) ?? new T();
        }

        return new T();
    }
}
