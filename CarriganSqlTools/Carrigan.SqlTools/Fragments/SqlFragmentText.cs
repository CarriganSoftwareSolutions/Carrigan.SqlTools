using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents an SQL fragment containing literal SQL text.
/// </summary>
/// <remarks>
/// This fragment is used for SQL tokens that are not parameters, such as keywords, punctuation, or whitespace.
/// </remarks>
public class SqlFragmentText : SqlFragment
{
    /// <summary>
    /// The literal SQL text represented by this fragment.
    /// </summary>
    protected readonly string SqlText;

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentText"/> with the provided SQL text.
    /// </summary>
    /// <param name="text">The literal SQL text for the fragment.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> is <c>null</c>.
    /// </exception>
    internal SqlFragmentText(string text) =>
        SqlText = text ?? throw new ArgumentNullException(nameof(text));

    /// <summary>
    /// Converts this fragment into its SQL representation.
    /// </summary>
    /// <returns>The literal SQL text for this fragment.</returns>
    internal override string ToSql() =>
        SqlText;

    /// <summary>
    /// Gets the parameters contained in this fragment. Since this fragment represents literal SQL text, it does not contain any parameters.
    /// </summary>
    /// <returns>An empty enumerable collection, as this fragment does not contain any parameters.</returns>
    internal override IEnumerable<Parameter> GetParameters() =>
        [];
}
