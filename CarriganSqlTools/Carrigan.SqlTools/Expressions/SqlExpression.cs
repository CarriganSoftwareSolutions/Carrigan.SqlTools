using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Expressions;

public abstract class SqlExpression
{
    /// <summary>
    /// Represents the direct children of the current expression.
    /// </summary>
    internal IEnumerable<SqlExpression> ChildNodes { get; }

    /// <summary>
    /// Base constructor for all expression classes.
    /// </summary>
    /// <param name="childExpressions">Represents all child nodes for a given expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="childExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="childExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected SqlExpression(IEnumerable<SqlExpression> childExpressions)
    {
        ArgumentNullException.ThrowIfNull(childExpressions, nameof(childExpressions));

        ChildNodes = childExpressions.Materialize(NullOptionsEnum.Exception);
    }

    /// <summary>
    /// Gets all parameter expressions reachable below the current expression node.
    /// </summary>
    internal IEnumerable<Parameter> DescendantParameters =>
        DescendantNodes.OfType<Parameter>();

    /// <summary>
    /// Represents all parameters that have the same unmodified parameter name.
    /// </summary>
    protected IEnumerable<ParameterTag> DuplicateParameters =>
        DescendantParameters
            .Select(static parameter => parameter.Name)
            .GroupBy(static parameter => parameter)
            .Where(static nameGroup => nameGroup.Count() > 1)
            .Select(static nameGroup => nameGroup.Key);

    /// <summary>
    /// Retrieves all descendant expressions regardless of type.
    /// </summary>
    internal IEnumerable<SqlExpression> DescendantNodes =>
        GetAllDescendantExpressions(ChildNodes);

    /// <summary>
    /// Retrieves all descendants of type <see cref="ColumnBase"/>.
    /// </summary>
    internal IEnumerable<ColumnBase> DescendantColumns =>
        DescendantNodes.OfType<ColumnBase>();

    /// <summary>
    /// Retrieves all descendant expressions that are not also <see cref="ColumnBase"/> or <see cref="Parameter"/>.
    /// These are the non-leaf operator nodes (e.g., <see cref="LogicalOperator"/>, <see cref="ComparisonOperator"/>).
    /// </summary>
    internal IEnumerable<SqlExpression> DescendantExpressions =>
        DescendantNodes.Where(static expression => expression is not Parameter && expression is not ColumnBase);

    /// <summary>
    /// Retrieves all direct children of type <see cref="Parameter"/>.
    /// </summary>
    internal IEnumerable<Parameter> ChildParameters =>
        ChildNodes.OfType<Parameter>();

    /// <summary>
    /// Retrieves all direct children of type <see cref="ColumnBase"/>.
    /// </summary>
    internal IEnumerable<ColumnBase> ChildColumns =>
        ChildNodes.OfType<ColumnBase>();

    /// <summary>
    /// Retrieves all direct children that are not also <see cref="ColumnBase"/> or <see cref="Parameter"/>.
    /// These are the non-leaf operator nodes.
    /// </summary>
    internal IEnumerable<SqlExpression> ChildExpressions =>
        ChildNodes.Where(static expression => expression is not Parameter && expression is not ColumnBase);

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
