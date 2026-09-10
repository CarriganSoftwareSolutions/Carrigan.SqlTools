using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for predicates that involve subqueries, such as EXISTS, IN, and comparison predicates with subqueries.
/// </summary>
public class SubqueryPredicateBase : Predicates
{
    /// <summary>
    /// The collection of SQL fragments that make up the subquery predicate.
    /// This typically includes the opening clause (e.g., "EXISTS ("), the subquery itself,
    /// and the closing parenthesis.
    /// </summary>
    protected readonly IEnumerable<ISqlFragment> Fragments;

    private readonly SubqueryBase _subQueryBase;
    private readonly string _command;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubqueryPredicateBase"/> class with the specified subquery
    /// and command (e.g., "EXISTS", "IN", or a comparison operator). The constructor constructs the appropriate
    /// SQL fragments
    /// </summary>
    /// <param name="subQueryBase">The subquery to include in the predicate.</param>
    /// <param name="command">The SQL command (e.g., "EXISTS", "IN") to use.</param>
    protected SubqueryPredicateBase(SubqueryBase subQueryBase, string command)
        : base([])
    {
        _subQueryBase = subQueryBase;
        _command = command;
        Fragments = ToSqlFragments(subQueryBase, command);
    }

    /// <summary>
    /// Converts the subquery and command into a sequence of SQL fragments for rendering.
    /// </summary>
    /// <param name="subQueryBase">
    /// The subquery to include in the predicate. This is an instance of <see cref="SubqueryBase"/> that represents the SQL subquery.
    /// </param>
    /// <param name="command">The SQL command (e.g., "EXISTS", "IN") to use.</param>
    /// <returns>An enumerable of <see cref="ISqlFragment"/> representing the SQL fragments of the subquery predicate.</returns>
    /// <remarks>
    /// This is a helper function for the one time generation stored in the property <see cref="Fragments"/>
    /// The internal method should be used for the final render.
    /// </remarks>
    private static IEnumerable<ISqlFragment> ToSqlFragments(SubqueryBase subQueryBase, string command) =>
        [new SqlFragmentText($"({command} "), subQueryBase, new SqlFragmentText(")")];

    protected override bool EqualsCore(SqlExpression other) =>
        other is SubqueryPredicateBase subqueryPredicate && ReferenceEquals(_subQueryBase, subqueryPredicate._subQueryBase) && string.Equals(_command, subqueryPredicate._command, StringComparison.Ordinal);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(RuntimeHelpers.GetHashCode(_subQueryBase));
        hashCode.Add(_command, StringComparer.Ordinal);
    }

    /// <summary>
    /// Returns the diagnostic representation of the stored subquery predicate fragments.
    /// </summary>
    public override string ToString() =>
        string.Join(' ', Fragments.Select(static fragment => fragment.ToString()));

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
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) =>
        Fragments;
}
