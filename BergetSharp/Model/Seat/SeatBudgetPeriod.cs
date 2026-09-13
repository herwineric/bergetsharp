using System.Text.Json.Serialization;

namespace BergetSharp.Model.Seat;

public sealed class SeatBudgetPeriod
{
    /// <summary>
    /// Fraction of this period's budget consumed (0-1).
    /// </summary>
    [JsonPropertyName("used_fraction")]
    public double UsedFraction { get; set; }

    /// <summary>
    /// Seconds until the rolling window resets; null when the window is inactive.
    /// </summary>
    [JsonPropertyName("reset_in_seconds")]
    public double? ResetInSeconds { get; set; }

    /// <summary>
    /// Per-model share of this period's consumption (of used, not of budget).
    /// </summary>
    [JsonPropertyName("models")]
    public Dictionary<string, double> Models { get; set; } = new();
}
