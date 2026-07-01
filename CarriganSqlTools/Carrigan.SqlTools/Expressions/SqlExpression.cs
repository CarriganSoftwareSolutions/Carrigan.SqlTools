using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.Core.DataTypes;

namespace Carrigan.SqlTools.Expressions;

public abstract class SqlExpression : StringWrapper
{
    /// <summary>
    /// Represents the direct children of the current expression.
    /// </summary>
    internal IEnumerable<SqlExpression> ChildNodes { get; }

    /// <summary>
    /// Base constructor for all expression classes.
    /// </summary>
    /// <param name="childExpressions">Represents all child nodes for a given expression.</param>
    /// <param name="dialectNeutralStringRepresentation">
    /// Represents a dialect-neutral string representation of the expression, used for debugging, logging, and key-value pairs.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="childExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="childExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected SqlExpression(IEnumerable<SqlExpression> childExpressions, string dialectNeutralStringRepresentation) 
        : base (dialectNeutralStringRepresentation)
    {
        ArgumentNullException.ThrowIfNull(childExpressions, nameof(childExpressions));

        ChildNodes = childExpressions.Materialize(NullOptionsEnum.Exception);
    }

    /// <summary>
    /// Gets all parameter expressions reachable below the current expression node.
    /// </summary>
    // TODO: still need?
    internal IEnumerable<Parameter> DescendantParameters =>
        DescendantNodes.OfType<Parameter>();

    /// <summary>
    /// Retrieves all descendant expressions regardless of type.
    /// </summary>
    internal IEnumerable<SqlExpression> DescendantNodes =>
        GetAllDescendantExpressions(ChildNodes);

    /// <summary>
    /// Retrieves all descendants of type <see cref="ColumnBase"/>.
    /// </summary>
    // TODO: Still need?
    internal IEnumerable<ColumnBase> DescendantColumns =>
        DescendantNodes.OfType<ColumnBase>();

    /// <summary>
    /// Gets the table tags represented by leaf expressions directly attached to this expression.
    /// </summary>
    public virtual IEnumerable<TableTag> LeafTables => [];

    /// <summary>
    /// Gets all table tags represented by leaf expressions anywhere underneath this expression.
    /// </summary>
    public IEnumerable<TableTag> DescendantLeafTables =>
        LeafTables
            .Concat(DescendantNodes.SelectMany(static expression => expression.LeafTables))
            .Distinct();

    /// <summary>
    /// Indicates whether this expression is valid in an aggregate SELECT list for the supplied <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="groupBys">The optional <c>GROUP BY</c> clause to check.</param>
    /// <returns><c>false</c> unless an expression type overrides this method.</returns>
    public virtual bool IsAggregate(GroupBysBase? groupBys) =>
        false;

    /// <summary>
    /// Generates the SQL fragments for this expression tree.
    /// </summary>
    /// <remarks>
    /// Before rendering, this method computes duplicate user-supplied parameter names and
    /// passes that set to the recursive <see cref="ToSqlFragments()"/> overload,
    /// which may add disambiguating prefixes to produce unique parameter names.
    /// </remarks>
    /// <returns>The SQL fragments represented by this expression tree.</returns>

    internal abstract IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect);

    /// <summary>
    /// Recursively enumerates every child expression below the supplied expression collection.
    /// </summary>
    /// <param name="expressions">The expression collection whose descendants should be enumerated.</param>
    /// <returns>All descendant expression nodes in depth-first order.</returns>
    private static IEnumerable<SqlExpression> GetAllDescendantExpressions(IEnumerable<SqlExpression> expressions)
    {
        foreach (SqlExpression expression in expressions)
        {
            yield return expression;

            foreach (SqlExpression childExpression in GetAllDescendantExpressions(expression.ChildNodes))
                yield return childExpression;
        }
    }
}
