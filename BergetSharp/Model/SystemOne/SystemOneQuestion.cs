using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BergetSharp.Model.SystemOne;

/// <summary>
/// A typed question about the application state, keyed by a caller-chosen id in
/// <see cref="SystemOneRequest.Questions"/>. Use the factory methods <see cref="Noul"/>,
/// <see cref="Choice"/> and <see cref="Score"/> to create questions.
/// </summary>
public sealed class SystemOneQuestion
{
    /// <summary>
    /// The question type: <c>noul</c>, <c>choice</c> or <c>score</c> (see <see cref="SystemOneQuestionType"/>).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The statement or question to evaluate. May be a string, an object or an array; use an
    /// object to give the question extra context, referencing data fields by name in backticks.
    /// </summary>
    [JsonPropertyName("instructions")]
    public object? Instructions { get; set; }

    /// <summary>
    /// Type-specific criteria:
    /// <c>noul</c>: optional map describing what yes and no mean;
    /// <c>choice</c>: required map of option to rubric description, 2-255 options (use null when an option needs no detail);
    /// <c>score</c>: required ordered list of 2-10 level descriptions.
    /// </summary>
    [JsonPropertyName("criteria")]
    public object? Criteria { get; set; }

    /// <summary>
    /// Additional fields in the question payload that are not mapped to a strongly typed property.
    /// Type-specific fields are forwarded unchanged.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    /// <summary>
    /// Creates a yes/no question. The answer is the probability (0-1) that the statement is true.
    /// </summary>
    /// <param name="instructions"> The statement to evaluate. </param>
    /// <param name="criteria"> Optional map describing what yes and no mean. </param>
    public static SystemOneQuestion Noul(object instructions, object? criteria = null)
        => new() { Type = SystemOneQuestionType.Noul, Instructions = instructions, Criteria = criteria };

    /// <summary>
    /// Creates a question that picks exactly one option from <paramref name="criteria"/>.
    /// </summary>
    /// <param name="instructions"> The question to evaluate. </param>
    /// <param name="criteria"> Map of option to rubric description, 2-255 options. </param>
    public static SystemOneQuestion Choice(object instructions, IDictionary<string, string?> criteria)
        => new() { Type = SystemOneQuestionType.Choice, Instructions = instructions, Criteria = criteria };

    /// <summary>
    /// Creates a question that rates the state on the ordered scale given by <paramref name="criteria"/>.
    /// </summary>
    /// <param name="instructions"> The question to evaluate. </param>
    /// <param name="criteria"> Ordered list of 2-10 level descriptions. </param>
    public static SystemOneQuestion Score(object instructions, IEnumerable<string> criteria)
        => new() { Type = SystemOneQuestionType.Score, Instructions = instructions, Criteria = criteria.ToList() };

    /// <summary>
    /// Validates the question against the System One endpoint rules,
    /// throwing <see cref="ArgumentException"/> when invalid.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Type))
        {
            throw new ArgumentException("Question type is required: noul, choice or score.", nameof(Type));
        }

        if (Type is not (SystemOneQuestionType.Noul or SystemOneQuestionType.Choice or SystemOneQuestionType.Score))
        {
            throw new ArgumentException($"Unknown question type '{Type}'; expected noul, choice or score.", nameof(Type));
        }

        if (Instructions is null || Instructions is string s && string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentException("Question instructions are required.", nameof(Instructions));
        }

        switch (Type)
        {
            case SystemOneQuestionType.Choice:
                if (Criteria is not IDictionary options)
                {
                    throw new ArgumentException("Choice questions require criteria: a map of option to rubric description.", nameof(Criteria));
                }

                if (options.Count is < 2 or > 255)
                {
                    throw new ArgumentException($"Choice questions require 2-255 options, but criteria contains {options.Count}.", nameof(Criteria));
                }

                break;

            case SystemOneQuestionType.Score:
                if (Criteria is not IList levels)
                {
                    throw new ArgumentException("Score questions require criteria: an ordered list of level descriptions.", nameof(Criteria));
                }

                if (levels.Count is < 2 or > 10)
                {
                    throw new ArgumentException($"Score questions require 2-10 levels, but criteria contains {levels.Count}.", nameof(Criteria));
                }

                break;
        }
    }
}
