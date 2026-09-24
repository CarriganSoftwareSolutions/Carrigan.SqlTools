using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using System.Linq.Expressions;

//IGNORE SPELLING: TRUNC

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL DATE_TRUNC function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// DateTrunc expression = new(DateTruncDateTimePartEnum.Month, new Parameter(new DateTime(2026, 9, 23)));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT DATE_TRUNC('month', $1) AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class DateTrunc : SqlExpression
{
    /// <summary>
    /// Gets the date part to truncate to, as a string.
    /// </summary>
    private readonly string _datePart;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTrunc"/> class.
    /// </summary>
    /// <param name="datePart">
    /// The date part to truncate to.
    /// </param>
    /// <param name="expression">
    /// The SQL expression representing the date/time value to truncate.
    /// </param>
    public DateTrunc(SharedDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTrunc"/> class.
    /// </summary>
    /// <param name="datePart">
    /// The date part to truncate to.
    /// </param>
    /// <param name="expression">
    /// The SQL expression representing the date/time value to truncate.
    /// </param>
    public DateTrunc(DateTruncDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    /// <summary>
    /// Determines whether the expression is an aggregate function.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the expression is an aggregate function; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool IsAggregate() => ChildNodes.Single().IsAggregate();

    /// <summary>
    /// Determines whether the current expression is equal to another expression.
    /// </summary>
    /// <param name="other">
    /// The other expression to compare with.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the current expression is equal to the other expression; otherwise, <see langword="false"/>.
    /// </returns>
    protected override bool EqualsCore(SqlExpression other) =>
        other is DateTrunc dateTrunc && string.Equals(_datePart, dateTrunc._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    /// <summary>
    /// Adds the current expression's components to the specified hash code.
    /// </summary>
    /// <param name="hashCode">
    /// The hash code to which the current expression's components will be added.
    /// </param>
    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    /// <summary>
    /// Generates the SQL fragments for the DATE_TRUNC expression based on the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for generating the SQL fragments.
    /// </param>
    /// <returns>
    /// An enumerable collection of <see cref="ISqlFragment"/> representing the SQL fragments for the DATE_TRUNC expression.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("DATE_TRUNC('");
        yield return new SqlFragmentText(_datePart);
        yield return new SqlFragmentText("', ");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return ISqlFragment.CloseParentheses;
    }

    /// <summary>
    ///     
    /// </summary>
    /// <param name="datePart">
    /// The date part to get the SQL representation for.
    /// </param>
    /// <returns>
    /// The SQL representation of the date part.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified date part is not a valid value of <see cref="SharedDateTimePartEnum"/>.
    /// </exception>
    private static string GetDatePart(SharedDateTimePartEnum datePart) => datePart switch
    {
        SharedDateTimePartEnum.Year => "year",
        SharedDateTimePartEnum.Month => "month",
        SharedDateTimePartEnum.Week => "week",
        SharedDateTimePartEnum.Day => "day",
        SharedDateTimePartEnum.Hour => "hour",
        SharedDateTimePartEnum.Minute => "minute",
        SharedDateTimePartEnum.Second => "second",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };

    /// <summary>
    /// Gets the SQL representation for the specified date part.
    /// </summary>
    /// <param name="datePart">The date part to get the SQL representation for.</param>
    /// <returns>The SQL representation of the date part.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified date part is not a valid value of <see cref="DateTruncDateTimePartEnum"/>.</exception>
    private static string GetDatePart(DateTruncDateTimePartEnum datePart) => datePart switch
    {
        DateTruncDateTimePartEnum.Microseconds => "microseconds",
        DateTruncDateTimePartEnum.Milliseconds => "milliseconds",
        DateTruncDateTimePartEnum.Second => "second",
        DateTruncDateTimePartEnum.Minute => "minute",
        DateTruncDateTimePartEnum.Hour => "hour",
        DateTruncDateTimePartEnum.Day => "day",
        DateTruncDateTimePartEnum.Week => "week",
        DateTruncDateTimePartEnum.Month => "month",
        DateTruncDateTimePartEnum.Quarter => "quarter",
        DateTruncDateTimePartEnum.Year => "year",
        DateTruncDateTimePartEnum.Decade => "decade",
        DateTruncDateTimePartEnum.Century => "century",
        DateTruncDateTimePartEnum.Millennium => "millennium",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };
}
