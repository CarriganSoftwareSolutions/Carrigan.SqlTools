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

    /// <summary>
    /// Gets the type used for equality comparisons. This is used to ensure that two different derived types are not considered equal even if they have the same properties.
    /// </summary>
    protected override object EqualityContract =>
        typeof(Aggregates);

    /// <summary>
    /// Determines whether the current aggregate expression is equal to another SQL expression.
    /// </summary>
    /// <param name="other">
    /// The other SQL expression to compare with the current aggregate expression.
    /// </param>
    /// <returns>
    /// <c>true</c> if the current aggregate expression is equal to <paramref name="other"/>; otherwise, <c>false</c>.
    /// </returns>
    protected override bool EqualsCore(SqlExpression other) =>
        other is Aggregates aggregate &&
        string.Equals(FunctionName, aggregate.FunctionName, StringComparison.OrdinalIgnoreCase) &&
        Distinct == aggregate.Distinct &&
        base.EqualsCore(other);

    /// <summary>
    /// Adds the properties of the aggregate expression to the provided <see cref="HashCode"/> instance for generating a hash code.
    /// </summary>
    /// <param name="hashCode">
    /// The <see cref="HashCode"/> instance to which the properties of the aggregate expression will be added.
    /// </param>
    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(FunctionName, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(Distinct);
        base.AddToHashCode(ref hashCode);
    }

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
