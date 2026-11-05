using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents a subquery that can be used as part of a larger SQL query. This is designed to be consumed by predicates such as
/// <see cref="Exists"/> or to build complex queries.
/// </summary>
/// <typeparam name="T">
/// The model type associated with the subquery. This type parameter is used for type safety and to enable
/// intellisense when building subqueries.
/// </typeparam>
[Obsolete("There is no longer a reason for a Generic version of this class. I don't know what I was thinking.")]
public class Subquery<T> : Subquery where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Subquery{T}"/> class with the specified SQL fragments and dialect.
    /// </summary>
    /// <param name="selects">
    /// The selected columns or expressions for the subquery.
    /// </param>
    /// <param name="sqlFragments">
    /// The sequence of SQL fragments that make up the subquery. These fragments will be rendered together to form the complete
    /// SQL text of the subquery when it is consumed by a predicate or included in a larger query.
    /// </param>
    internal Subquery(SelectTags selects, IEnumerable<ISqlFragment> sqlFragments)
        : base(selects, sqlFragments)
    {
    }
}

/// <summary>
/// Represents a base class for subqueries, encapsulating common logic for rendering and parameter management.
/// </summary>
public class Subquery : ISelectable, ISqlFragment
{
    /// <summary>
    /// The sequence of SQL fragments that make up the subquery. These fragments will be rendered
    /// together to form the complete SQL text of the subquery  when it is consumed by a predicate
    /// or included in a larger query.
    /// </summary>
    private readonly IReadOnlyList<ISqlFragment> SqlFragments;

    public SelectTags SelectTags { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Subquery"/> class with the specified SQL fragments and dialect.
    /// </summary>
    /// <param name="selectTags">
    /// The selected columns or expressions for the subquery.
    /// </param>
    /// <param name="sqlFragments">
    /// The sequence of SQL fragments that make up the subquery. These fragments will be rendered
    /// together to form the complete SQL text of the subquery when it is consumed  by a predicate
    /// or included in a larger query.
    /// </param>
    internal Subquery(SelectTags selectTags, IEnumerable<ISqlFragment> sqlFragments)
    {
        ArgumentNullException.ThrowIfNull(sqlFragments);
        ArgumentNullException.ThrowIfNull(selectTags);

        SelectTags = selectTags;

        IEnumerable<ISqlFragment> GetFragments()
        {
            yield return new SqlFragmentText("(");
            foreach (ISqlFragment fragment in sqlFragments)
                yield return fragment;
            yield return new SqlFragmentText(")");
        }

        SqlFragments = Array.AsReadOnly(GetFragments().ToArray());
    }

    /// <summary>
    /// Flattens the nested structure of SQL fragments into a single enumerable sequence. This
    /// is necessary because the SQL fragments may be composed of other fragments, and we need
    /// to ensure that we have a single level of fragments when rendering the SQL text or extracting parameters.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the fragments of this subquery.
    /// </param>
    /// <returns>The flattened SQL fragments for the subquery.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        SqlFragments.Flatten(dialect);

    /// <summary>
    /// Extracts the parameters from the SQL fragments of this subquery. This method traverses the fragments
    /// and collects any parameters that are defined within those fragments, returning them as a single
    /// enumerable sequence.
    /// </summary>
    /// <returns>
    /// An enumerable sequence of <see cref="SqlFragmentParameter"/> objects that represent the parameters defined
    /// within the SQL fragments of this subquery.
    /// </returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        SqlFragments.GetSqlFragmentParameters(dialect);

    /// <summary>
    /// Renders the SQL fragments of this subquery into a complete SQL string. This method concatenates the SQL
    /// text of each fragment, using the provided SQL dialect to ensure that any database-specific syntax is
    /// correctly applied.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the fragments of this subquery.
    /// This ensures that the generated SQL is compatible with the target database system.
    /// </param>
    /// <returns>
    /// A string that represents the complete SQL text of this subquery, rendered according
    /// to the specified SQL dialect.
    /// </returns>
    public string ToSql(ISqlDialects dialect)
    {
        StringBuilder stringBuilder = new();

        foreach (ISqlFragment fragment in SqlFragments.Flatten(dialect))
        {
            stringBuilder.Append(fragment.ToSql(dialect));
        }
        return stringBuilder.ToString();
    }
}
