using System.Text.Json.Serialization;

namespace BergetSharp.Model.Seat;

public sealed class SeatBudgetList
{
    /// <summary>
    /// Object type discriminator (always "seat_budget_list").
    /// </summary>
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    /// <summary>
    /// One entry per requested seat, in request order.
    /// </summary>
    [JsonPropertyName("budgets")]
    public List<SeatBudget> Budgets { get; set; } = new();

    /// <summary>
    /// Hard per-model usage caps (fraction of each period budget).
    /// A seat is rate-limited for a listed model once that model alone crosses
    /// the fraction, even with total budget left.
    /// </summary>
    [JsonPropertyName("model_limits")]
    public Dictionary<string, ModelLimit> ModelLimits { get; set; } = new();
}
