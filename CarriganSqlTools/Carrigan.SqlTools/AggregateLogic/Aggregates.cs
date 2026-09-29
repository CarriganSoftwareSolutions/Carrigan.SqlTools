using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Base class for SQL aggregate expressions.
/// </summary>
public abstract class Aggregates : SqlExpression
{
    /// <summary>
    /// The aggregate function name to render.
    /// </summary>
    protected readonly string FunctionName;
    private readonly bool Distinct;

    /// <summary>
    /// Initializes a new aggregate expression without <c>DISTINCT</c>.
    /// </summary>
    /// <param name="functionName">The aggregate function name.</param>
    /// <param name="expressions">The expressions supplied to the aggregate function.</param>
    protected Aggregates(string functionName, params IEnumerable<SqlExpression> expressions)
        : this(functionName, false, expressions)
    {
    }

    /// <summary>
    /// Initializes a new aggregate expression.
    /// </summary>
    /// <param name="functionName">The aggregate function name.</param>
    /// <param name="distinct">Whether duplicate input values are removed before aggregation.</param>
    /// <param name="expressions">The expressions supplied to the aggregate function.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="functionName"/> or <paramref name="expressions"/> is <c>null</c>.
    /// </exception>
    protected Aggregates(string functionName, bool distinct, params IEnumerable<SqlExpression> expressions)
        : base(expressions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(functionName, nameof(functionName));

        if (distinct && ChildNodes.None())
            throw new ArgumentException("DISTINCT aggregate expressions require at least one input expression.", nameof(distinct));

        FunctionName = functionName;
        Distinct = distinct;
    }

    protected override object EqualityContract =>
        typeof(Aggregates);

    protected override bool EqualsCore(SqlExpression other) =>
        other is Aggregates aggregate &&
        string.Equals(FunctionName, aggregate.FunctionName, StringComparison.OrdinalIgnoreCase) &&
        Distinct == aggregate.Distinct &&
        base.EqualsCore(other);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(FunctionName, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(Distinct);
        base.AddToHashCode(ref hashCode);
    }

    /// <summary>
    /// Aggregate functions are valid aggregate SELECT expressions.
    /// </summary>
    /// <returns>Always <c>true</c>.</returns>
    public override bool IsAggregate() =>
        true;

    /// <summary>
    /// Produces the SQL fragment represented by this aggregate expression.
    /// </summary>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText($"{FunctionName}(");

        if (ChildNodes.Any())
        {
            if (Distinct)
                yield return new SqlFragmentText("DISTINCT ");
            bool isFirstExpression = true;

            foreach (SqlExpression expression in ChildNodes)
            {
                if (!isFirstExpression)
                {
                    yield return new SqlFragmentText(", ");
                }

                foreach (ISqlFragment fragment in expression.ToSqlFragments(dialect))
                {
                    yield return fragment;
                }

                isFirstExpression = false;
            }
        }
        else
            yield return new SqlFragmentText("*");

        yield return new SqlFragmentText(")");
    }

}
