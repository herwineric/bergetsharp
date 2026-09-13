using System.Text.Json.Serialization;

namespace BergetSharp.Model.Seat;

public sealed class SeatBudgetPeriods
{
    /// <summary>
    /// Consumption within the monthly rolling window.
    /// Only enabled periods (budget &gt; 0) are present.
    /// </summary>
    [JsonPropertyName("monthly")]
    public SeatBudgetPeriod? Monthly { get; set; }

    /// <summary>
    /// Consumption within the weekly rolling window.
    /// Only enabled periods (budget &gt; 0) are present.
    /// </summary>
    [JsonPropertyName("weekly")]
    public SeatBudgetPeriod? Weekly { get; set; }

    /// <summary>
    /// Consumption within the daily rolling window.
    /// Only enabled periods (budget &gt; 0) are present.
    /// </summary>
    [JsonPropertyName("daily")]
    public SeatBudgetPeriod? Daily { get; set; }
}
