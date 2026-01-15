namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents a fragment of a SQL statement.
/// </summary>
internal abstract class SqlFragment
{
    /// <summary>
    /// Converts this fragment to its SQL string representation.
    /// </summary>
    /// <returns>
    /// A string containing the SQL text for this fragment.
    /// </returns>
    internal abstract string ToSql();

    /// <summary>
    /// Converts this fragment to its SQL string representation.
    /// </summary>
    /// <remarks>
    /// This method delegates to <see cref="ToSql"/>. Any exceptions thrown by <see cref="ToSql"/>
    /// will propagate through this method.
    /// </remarks>
    /// <returns>
    /// A string containing the SQL text for this fragment.
    /// </returns>
    public override string ToString() =>
        ToSql();
}
