using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Model.SystemOne;

/// <summary>
/// Request for the System One endpoint (<c>/v1/systemone</c>): answers every question in
/// <see cref="Questions"/> about <see cref="State"/> in a single pass.
/// Compatible with the TypeSafe Jev <c>/v1/systemone</c> API.
/// </summary>
public sealed class SystemOneRequest
{
    /// <summary>
    /// The System One model to use. Optional; the service defaults to <c>Qwen/Qwen3.5-2B</c>.
    /// Served ids: <c>Qwen/Qwen3.5-2B</c> and <c>convaiinnovations/laya</c>, plus the convenience
    /// aliases <c>systemone</c>, <c>systemone-qwen3.5-2b</c>, <c>systemone-laya</c> and <c>laya-latest</c>.
    /// Unknown ids are rejected by the service with 404.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// The application state the questions are asked about. Required.
    /// May be a string, an object or an array; nested structures are allowed.
    /// </summary>
    [JsonPropertyName("state")]
    public object? State { get; set; }

    /// <summary>
    /// The questions to answer, keyed by a caller-chosen id. Between 1 and 64 questions;
    /// all are answered in parallel in a single pass.
    /// </summary>
    [JsonPropertyName("questions")]
    public Dictionary<string, SystemOneQuestion> Questions { get; set; } = new();

    /// <summary>
    /// Additional fields in the request payload that are not mapped to a strongly typed property.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    /// <summary>
    /// Validates the request against the System One endpoint rules (required state, 1-64 questions,
    /// per-question type and criteria rules), throwing <see cref="ArgumentException"/> when invalid.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void Validate()
    {
        if (State is null)
        {
            throw new ArgumentException("A state is required: the application state the questions are asked about.", nameof(State));
        }

        if (Model is not null && string.IsNullOrWhiteSpace(Model))
        {
            throw new ArgumentException("Model must not be empty when set; omit it to use the service default.", nameof(Model));
        }

        if (Questions is null || Questions.Count == 0)
        {
            throw new ArgumentException("At least one question is required.", nameof(Questions));
        }

        if (Questions.Count > 64)
        {
            throw new ArgumentException($"At most 64 questions are allowed per request, but got {Questions.Count}.", nameof(Questions));
        }

        foreach (KeyValuePair<string, SystemOneQuestion> pair in Questions)
        {
            if (string.IsNullOrWhiteSpace(pair.Key))
            {
                throw new ArgumentException("Question ids must not be empty.", nameof(Questions));
            }

            if (pair.Value is null)
            {
                throw new ArgumentException($"Question '{pair.Key}' must not be null.", nameof(Questions));
            }

            try
            {
                pair.Value.Validate();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Question '{pair.Key}' is invalid: {ex.Message}", nameof(Questions), ex);
            }
        }
    }
}
