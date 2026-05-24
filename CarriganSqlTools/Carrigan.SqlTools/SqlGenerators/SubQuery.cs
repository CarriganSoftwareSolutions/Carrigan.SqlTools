using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

//IGNORE SPELLING: subquery, subqueries, intellisense

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents a subquery that can be used as part of a larger SQL query. This class is typically returned by
/// methods like <see cref="SqlGeneratorBase{T}.SubQuery"/> and is designed to be consumed by predicates such as
/// <see cref="Exists"/> or to build complex queries.
/// </summary>
/// <typeparam name="T">
/// The model type associated with the subquery. This type parameter is used for type safety and to enable
/// intellisense when building subqueries.
/// </typeparam>
public class SubQuery<T> : SubQueryBase where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubQuery{T}"/> class with the specified SQL fragments and dialect.
    /// </summary>
    /// <param name="sqlFragments">
    /// The sequence of SQL fragments that make up the subquery. These fragments will be rendered together to form the complete
    /// SQL text of the subquery when it is consumed by a predicate or included in a larger query.
    /// </param>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the fragments of this subquery.
    /// This ensures that the generated SQL is compatible with the target database system.
    /// </param>
    internal SubQuery(IEnumerable<ISqlFragment> sqlFragments, ISqlDialects dialect)
        : base(sqlFragments, dialect)
    {
    }
}
