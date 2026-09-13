using System.Text.Json.Serialization;

namespace BergetSharp.Model.Seat;

public sealed class SeatBudget
{
    /// <summary>
    /// Object type discriminator (always "seat_budget").
    /// </summary>
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    /// <summary>
    /// Odoo seat ID the budget is scoped to.
    /// </summary>
    [JsonPropertyName("seat_id")]
    public long SeatId { get; set; }

    /// <summary>
    /// Seat tier from the auth claims.
    /// </summary>
    [JsonPropertyName("tier")]
    public string Tier { get; set; } = string.Empty;

    /// <summary>
    /// True when the limiter only observes (no 429s) - clients should render tentatively.
    /// </summary>
    [JsonPropertyName("shadow_mode")]
    public bool ShadowMode { get; set; }

    /// <summary>
    /// Consumption across the rolling windows. Only enabled periods (budget &gt; 0) are present.
    /// </summary>
    [JsonPropertyName("periods")]
    public SeatBudgetPeriods Periods { get; set; } = new();

    /// <summary>
    /// Hard per-model usage caps (fraction of each period budget).
    /// A seat is rate-limited for a listed model once that model alone crosses
    /// the fraction, even with total budget left.
    /// </summary>
    [JsonPropertyName("model_limits")]
    public Dictionary<string, ModelLimit> ModelLimits { get; set; } = new();
}
