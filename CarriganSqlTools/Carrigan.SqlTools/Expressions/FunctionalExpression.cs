using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL functional expression that applies a function to a set of child expressions.
/// </summary>
public abstract class FunctionalExpression : SqlExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected abstract string FunctionName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionalExpression"/> class with the specified values.
    /// </summary>
    /// <param name="values">
    /// The expressions to evaluate in order. The sequence must contain at least one value.
    /// </param>
    public FunctionalExpression(params IEnumerable<SqlExpression> values) : base(values)
    {
    }

    /// <summary>
    /// Converts the Funcational expression into SQL fragments for the specified dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render each child expression.</param>
    /// <returns>The SQL fragments representing the Funcational expression.</returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        IEnumerable<ISqlFragment> childFragments =
            ChildNodes.Select(value => new SqlFragmentGroup(value.ToSqlFragments(dialect))).JoinFragments(ISqlFragment.CommaSpace).Flatten(dialect);
        yield return new SqlFragmentText(FunctionName);
        yield return ISqlFragment.OpenParentheses;
        foreach (ISqlFragment sqlFragment in childFragments)
        {
            yield return sqlFragment;
        }
        yield return ISqlFragment.CloseParentheses;
    }

    /// <summary>
    /// Determines whether the Funcational expression is aggregate based on the aggregate status of its values.
    /// </summary>
    /// <returns>
    /// <c>true</c> when all values are aggregate expressions; <c>false</c> when all values are non-aggregate expressions.
    /// </returns>
    /// <exception cref="AggregateInconsistencyException">
    /// Thrown when aggregate and non-aggregate values are mixed within the expression.
    /// </exception>
    public override bool IsAggregate()
    {
        if (ChildNodes.Select(value => value.IsAggregate()).AllEqual() ?? false)
        {
            return ChildNodes.First().IsAggregate();
        }
        else
        {
            throw new AggregateInconsistencyException();
        }
    }
}
