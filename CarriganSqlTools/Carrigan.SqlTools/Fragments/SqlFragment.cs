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
/// <para>
/// <see cref="ToString"/> is intentionally implemented as an alias for <see cref="ToSql"/> so fragments can be
/// easily written into logs, debuggers, and interpolated strings.
/// </para>
/// </remarks>
internal abstract class SqlFragment
{
    /// <summary>
    /// Converts this fragment into its SQL representation.
    /// </summary>
    /// <returns>The SQL text for this fragment.</returns>
    internal abstract string ToSql();

    /// <summary>
    /// Returns the SQL representation of this fragment.
    /// </summary>
    /// <returns>The SQL text for this fragment.</returns>
    /// <remarks>
    /// Any exception thrown by <see cref="ToSql"/> will be propagated to the caller.
    /// </remarks>
    public override string ToString() =>
        ToSql();
}
