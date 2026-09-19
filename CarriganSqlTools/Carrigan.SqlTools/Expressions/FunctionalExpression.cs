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
    /// Gets a value indicating whether parentheses are rendered after the function name.
    /// </summary>
    protected virtual bool RenderParentheses =>
        true;

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

        if (!RenderParentheses)
            yield break;

        yield return ISqlFragment.OpenParentheses;
        foreach (ISqlFragment sqlFragment in childFragments)
        {
            yield return sqlFragment;
        }
        yield return ISqlFragment.CloseParentheses;
    }

    /// <summary>
    /// Determines whether the functional expression is aggregate based on its row-dependent values.
    /// </summary>
    /// <remarks>
    /// Row-independent values, such as parameters, do not change aggregate status. This allows valid expressions such as
    /// <c>COALESCE(AVG(Value), @Fallback)</c> while still rejecting a mix of aggregate and non-aggregate column values.
    /// </remarks>
    /// <returns>
    /// <c>true</c> when the row-dependent values are aggregate expressions; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="AggregateInconsistencyException">
    /// Thrown when aggregate and non-aggregate row-dependent values are mixed within the expression.
    /// </exception>
    public override bool IsAggregate()
    {
        IEnumerable<(SqlExpression Expression, bool IsAggregate)> aggregateCandidates = ChildNodes
            .Select(expression => (Expression: expression, IsAggregate: expression.IsAggregate()))
            .Where(candidate => candidate.IsAggregate || candidate.Expression.HasColumns());

        bool[] aggregateStates = [.. aggregateCandidates.Select(static candidate => candidate.IsAggregate)];

        if (aggregateStates.AllEqual() is false)
            throw new AggregateInconsistencyException();

        return aggregateStates.FirstOrDefault();
    }


    /// <summary>
    /// Validates the provided values for the specified function, ensuring that the number of arguments meets the minimum requirement.
    /// </summary>
    /// <param name="minArguments">The expressions to validate.</param>
    /// <param name="values">The expressions to validate.</param>
    /// <returns>A materialized sequence containing the validated expressions.</returns>
    protected static IEnumerable<SqlExpression> ValidateValues(int minArguments, IEnumerable<SqlExpression> values)
    {
        ArgumentNullException.ThrowIfNull(values, nameof(values));

        if (values.Count() < minArguments)
            throw new ArgumentException($"Scalar function requires {minArguments} or more expressions.", nameof(values));

        return values;
    }
}
