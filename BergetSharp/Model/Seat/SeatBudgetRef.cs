using System.Text.Json.Serialization;

namespace BergetSharp.Model.Seat;

public sealed class SeatBudgetRef
{
    /// <summary>
    /// Odoo seat ID.
    /// </summary>
    [JsonPropertyName("seat_id")]
    public long SeatId { get; set; }

    /// <summary>
    /// Seat tier (plan code) - resolved upstream by the gateway.
    /// Unknown tiers get the default budget.
    /// </summary>
    [JsonPropertyName("tier")]
    public string Tier { get; set; } = string.Empty;
}
