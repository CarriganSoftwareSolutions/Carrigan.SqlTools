using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for predicates that involve subqueries, such as EXISTS, IN, and comparison predicates with subqueries.
/// </summary>
public class SubQueryPredicateBase : Predicates
{
    /// <summary>
    /// The collection of SQL fragments that make up the subquery predicate. 
    /// This typically includes the opening clause (e.g., "EXISTS ("), the subquery itself,
    /// and the closing parenthesis.
    /// </summary>
    protected readonly IEnumerable<ISqlFragment> Fragments;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubQueryPredicateBase"/> class with the specified subquery 
    /// and command (e.g., "EXISTS", "IN", or a comparison operator). The constructor constructs the appropriate 
    /// SQL fragments
    /// </summary>
    /// <param name="subQueryBase">The subquery to include in the predicate.</param>
    /// <param name="command">The SQL command (e.g., "EXISTS", "IN") to use.</param>
    protected SubQueryPredicateBase(SubQueryBase subQueryBase, string command) : base([]) => 
        Fragments = [new SqlFragmentText($"({command} "), subQueryBase, new SqlFragmentText(")")];

    /// <summary>
    /// Converts the predicate into a sequence of SQL fragments for rendering. 
    /// This method returns the fragments that make up the subquery predicate, 
    /// which will be used by the SQL generator to produce the final SQL string.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the fragments. This may affect how certain fragments are rendered,
    /// but in this base implementation, the fragments are returned as-is since they are already constructed in the constructor.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ISqlFragment"/> that represents the SQL fragments of the subquery predicate.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) =>
        Fragments;
}
