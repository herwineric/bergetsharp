using System.Text.Json.Serialization;

namespace BergetSharp.Model.Seat;

public sealed class SeatBudgetsRequest
{
    /// <summary>
    /// Seats to read budget consumption for (1-100 entries).
    /// </summary>
    [JsonPropertyName("seats")]
    public List<SeatBudgetRef> Seats { get; set; } = new();
}
