using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents a single fragment of an SQL statement.
/// </summary>
/// <remarks>
/// This type is the common base for fragment implementations such as:
/// <list type="bullet">
/// <item><description><see cref="SqlFragmentText"/> for literal SQL text.</description></item>
/// <item><description><see cref="SqlFragmentParameter"/> for parameter references.</description></item>
/// </list>
/// </remarks>
public interface ISqlFragment
{
    /// <summary>
    /// Represents a single SQL space character.
    /// </summary>
    internal static readonly ISqlFragment Space = new SqlFragmentText(" ");
    /// <summary>
    /// Represents a SQL statement terminator.
    /// </summary>
    internal static readonly ISqlFragment Semicolon = new SqlFragmentText(";");
    /// <summary>
    /// Represents the current platform newline sequence.
    /// </summary>
    internal static readonly ISqlFragment NewLine = new SqlFragmentText(Environment.NewLine);

    /// <summary>
    /// Converts this fragment into its SQL representation.
    /// </summary>
    /// <returns>The SQL text for this fragment.</returns>
    internal string ToSql(ISqlDialects dialect);

    /// <summary>
    /// Gets the parameters that are referenced by this fragment.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="SqlFragmentParameter"/> objects referenced by this fragment.</returns>
    IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters();

    /// <summary>
    /// Returns a flattened sequence of all child SQL fragments contained within this fragment.
    /// </summary>
    /// <remarks>Use this method to enumerate all nested SQL fragments in a single, flat sequence, regardless
    /// of their original hierarchical structure.</remarks>
    /// <returns>An enumerable collection of <see cref="ISqlFragment"/> objects representing the flattened structure of this
    /// fragment. The collection may be empty if there are no child fragments.</returns>
    IEnumerable<ISqlFragment> Flatten();
}
