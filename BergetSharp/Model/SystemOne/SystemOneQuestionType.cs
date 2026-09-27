namespace BergetSharp.Model.SystemOne;

/// <summary>
/// Question types supported by the System One endpoint (<c>/v1/systemone</c>).
/// </summary>
public static class SystemOneQuestionType
{
    /// <summary>
    /// A yes/no question. The answer is the probability (0-1) that the statement is true.
    /// </summary>
    public const string Noul = "noul";

    /// <summary>
    /// Pick exactly one option from a map of options.
    /// </summary>
    public const string Choice = "choice";

    /// <summary>
    /// Rate the state on an ordered scale of levels.
    /// </summary>
    public const string Score = "score";
}
