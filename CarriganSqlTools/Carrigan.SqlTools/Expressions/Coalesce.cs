using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL <c>COALESCE</c> expression that returns the first non-null value from a sequence of expressions.
/// </summary>
public class Coalesce : SqlExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Coalesce"/> class with the specified values.
    /// </summary>
    /// <param name="values">
    /// The expressions to evaluate in order. The sequence must contain at least two values.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="values"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="values"/> contains fewer than two expressions.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="values"/> contains a <c>null</c> expression.
    /// </exception>
    public Coalesce(params IEnumerable<SqlExpression> values) : base(ValidateValues(values))
    {
    }

    /// <summary>
    /// Validates and materializes the values supplied to the <c>COALESCE</c> expression.
    /// </summary>
    /// <param name="values">The expressions to validate.</param>
    /// <returns>A materialized sequence containing the validated expressions.</returns>
    private static IEnumerable<SqlExpression> ValidateValues(IEnumerable<SqlExpression> values)
    {
        ArgumentNullException.ThrowIfNull(values, nameof(values));

        if (values.Count() < 2)
            throw new ArgumentException("Coalesce requires two or more values.", nameof(values));

        return values;
    }

    /// <summary>
    /// Converts the <c>COALESCE</c> expression into SQL fragments for the specified dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render each child expression.</param>
    /// <returns>The SQL fragments representing the <c>COALESCE</c> expression.</returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        IEnumerable<ISqlFragment> childFragments =
            ChildNodes.Select(value => new SqlFragmentGroup(value.ToSqlFragments(dialect))).JoinFragments(ISqlFragment.CommaSpace).Flatten(dialect);
        yield return new SqlFragmentText("COALESCE(");
        foreach (ISqlFragment sqlFragment in childFragments)
        {
            yield return sqlFragment;
        }
        yield return ISqlFragment.CloseParentheses;
    }

    /// <summary>
    /// Determines whether the <c>COALESCE</c> expression is aggregate based on the aggregate status of its values.
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
