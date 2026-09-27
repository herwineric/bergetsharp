using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Model.SystemOne;

/// <summary>
/// The structured answer to one <see cref="SystemOneQuestion"/>. Which properties are populated
/// depends on <see cref="Type"/>: <c>noul</c> answers set <see cref="Noul"/>; <c>choice</c> answers
/// set <see cref="Choice"/>, <see cref="Probabilities"/> and <see cref="Confidence"/>; <c>score</c>
/// answers set <see cref="Score"/>, <see cref="Legend"/>, <see cref="Probabilities"/> and
/// <see cref="Confidence"/>.
/// </summary>
public sealed class SystemOneAnswer
{
    /// <summary>
    /// Answer type, matching the question type: <c>noul</c>, <c>choice</c> or <c>score</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Noul answers: probability (0-1) that the statement is true.
    /// </summary>
    [JsonPropertyName("noul")]
    public double? Noul { get; set; }

    /// <summary>
    /// Choice answers: the highest-probability option.
    /// </summary>
    [JsonPropertyName("choice")]
    public string? Choice { get; set; }

    /// <summary>
    /// Score answers: probability-weighted value across the levels (can land between levels).
    /// </summary>
    [JsonPropertyName("score")]
    public double? Score { get; set; }

    /// <summary>
    /// Choice answers: every option mapped to its probability (sums to 1).
    /// Score answers: per level probability, keyed by level index.
    /// </summary>
    [JsonPropertyName("probabilities")]
    public Dictionary<string, double>? Probabilities { get; set; }

    /// <summary>
    /// Score answers: each level index mapped to its description.
    /// </summary>
    [JsonPropertyName("legend")]
    public Dictionary<string, string>? Legend { get; set; }

    /// <summary>
    /// Choice and score answers: confidence (0-1), derived from the probability distribution.
    /// </summary>
    [JsonPropertyName("confidence")]
    public double? Confidence { get; set; }

    /// <summary>
    /// Additional fields in the answer payload that are not mapped to a strongly typed property.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
