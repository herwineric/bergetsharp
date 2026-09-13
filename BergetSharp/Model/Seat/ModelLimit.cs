using System.Text.Json.Serialization;

namespace BergetSharp.Model.Seat;

public sealed class ModelLimit
{
    /// <summary>
    /// Hard usage cap for the monthly window, as a fraction of the monthly period budget (0-1).
    /// </summary>
    [JsonPropertyName("monthly")]
    public double Monthly { get; set; }

    /// <summary>
    /// Hard usage cap for the weekly window, as a fraction of the weekly period budget (0-1).
    /// </summary>
    [JsonPropertyName("weekly")]
    public double Weekly { get; set; }

    /// <summary>
    /// Hard usage cap for the daily window, as a fraction of the daily period budget (0-1).
    /// </summary>
    [JsonPropertyName("daily")]
    public double Daily { get; set; }
}
