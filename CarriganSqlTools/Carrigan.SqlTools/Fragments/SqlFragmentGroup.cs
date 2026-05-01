using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// A SqlFragment that is made up of multiple other SqlFragments. This is used to combine multiple fragments into one.
/// </summary>
public class SqlFragmentGroup : SqlFragment
{
    /// <summary>
    /// Represents the collection of SQL fragments associated with the current instance.
    /// </summary>
    /// <remarks>This collection is intended to be used by derived classes to build or manipulate SQL
    /// statements. The contents of the collection may affect the resulting SQL output or query behavior.</remarks>
    protected readonly IEnumerable<SqlFragment> sqlFragments;

    /// <summary>
    /// Initializes a new instance of the SqlFragmentGroup class with the specified collection of SQL fragments.
    /// </summary>
    /// <param name="sqlFragments">The collection of SQL fragments to include in the group. Cannot be null.</param>
    internal SqlFragmentGroup(params IEnumerable<SqlFragment> sqlFragments) =>
        this.sqlFragments = sqlFragments;

    /// <summary>
    /// Generates the SQL representation of the current object by concatenating the SQL fragments.
    /// </summary>
    /// <returns>A string containing the complete SQL statement formed by joining all SQL fragments. The string may be empty if
    /// there are no fragments.</returns>
    internal override string ToSql() =>
        string.Join("", sqlFragments.Select(f => f.ToSql()));

    /// <summary>
    /// Retrieves the collection of parameters from all SQL fragments contained within the group. This method aggregates
    /// </summary>
    /// <returns></returns>
    internal override IEnumerable<Parameter> GetParameters() =>
        sqlFragments.SelectMany(f => f.GetParameters());
}
