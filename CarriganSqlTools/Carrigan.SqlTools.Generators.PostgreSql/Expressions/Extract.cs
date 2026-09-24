using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using System.Linq.Expressions;

//IGNORE SPELLING: doy, isodow, isoyear, julian

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL EXTRACT expression.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Extract expression = new(ExtractDateTimePartEnum.Year, new Parameter(new DateTime(2026, 9, 23)));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT EXTRACT(year FROM $1) AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class Extract : SqlExpression
{
    /// <summary>
    /// Gets the date part to extract, as a string.
    /// </summary>
    private readonly string _datePart;

    /// <summary>
    /// Initializes a new instance of the <see cref="Extract"/> class.
    /// </summary>
    /// <param name="datePart">
    /// The date part to extract.
    /// </param>
    /// <param name="expression">
    /// The SQL expression representing the date/time value from which to extract the specified part.
    /// </param>
    public Extract(SharedDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    /// <summary>
    /// Initializes a new instance of the <see cref="Extract"/> class.
    /// </summary>
    /// <param name="datePart">
    /// The date part to extract.
    /// </param>
    /// <param name="expression">
    /// The SQL expression representing the date/time value from which to extract the specified part.
    /// </param>
    public Extract(ExtractDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    /// <summary>
    /// Determines whether the expression is an aggregate function.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the expression is an aggregate function; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool IsAggregate() => ChildNodes.Single().IsAggregate();

    /// <summary>
    /// Determines whether the specified <see cref="SqlExpression"/> is equal to the current <see cref="Extract"/> instance.
    /// </summary>
    /// <param name="other">
    /// The <see cref="SqlExpression"/> to compare with the current instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="SqlExpression"/> is equal to the current instance; otherwise, <see langword="false"/>.
    /// </returns>
    protected override bool EqualsCore(SqlExpression other) =>
        other is Extract extract && string.Equals(_datePart, extract._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    /// <summary>
    /// Adds the current instance's components to the specified <see cref="HashCode"/> instance for hash code generation.
    /// </summary>
    /// <param name="hashCode">
    /// The <see cref="HashCode"/> instance to which the current instance's components will be added.
    /// </param>
    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    /// <summary>
    /// Converts the current <see cref="Extract"/> instance to a sequence of SQL fragments that represent the corresponding SQL expression.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for generating the SQL fragments.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ISqlFragment"/> instances representing the SQL expression.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("EXTRACT(");
        yield return new SqlFragmentText(_datePart);
        yield return new SqlFragmentText(" FROM ");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return ISqlFragment.CloseParentheses;
    }

    /// <summary>
    /// Maps the specified <see cref="SharedDateTimePartEnum"/> value to its corresponding SQL string representation.
    /// </summary>
    /// <param name="datePart">
    /// The <see cref="SharedDateTimePartEnum"/> value representing the date part to extract.
    /// </param>
    /// <returns>
    /// A string representing the SQL equivalent of the specified <see cref="SharedDateTimePartEnum"/> value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified <see cref="SharedDateTimePartEnum"/> value is not recognized or supported.
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
    /// Maps the specified <see cref="ExtractDateTimePartEnum"/> value to its corresponding SQL string representation.
    /// </summary>
    /// <param name="datePart">
    /// The <see cref="ExtractDateTimePartEnum"/> value representing the date part to extract.
    /// </param>
    /// <returns>
    /// A string representing the SQL equivalent of the specified <see cref="ExtractDateTimePartEnum"/> value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified <see cref="ExtractDateTimePartEnum"/> value is not recognized or supported.
    /// </exception>
    private static string GetDatePart(ExtractDateTimePartEnum datePart) => datePart switch
    {
        ExtractDateTimePartEnum.Century => "century",
        ExtractDateTimePartEnum.Day => "day",
        ExtractDateTimePartEnum.Decade => "decade",
        ExtractDateTimePartEnum.DayOfWeek => "dow",
        ExtractDateTimePartEnum.DayOfYear => "doy",
        ExtractDateTimePartEnum.Epoch => "epoch",
        ExtractDateTimePartEnum.Hour => "hour",
        ExtractDateTimePartEnum.IsoDayOfWeek => "isodow",
        ExtractDateTimePartEnum.IsoYear => "isoyear",
        ExtractDateTimePartEnum.Julian => "julian",
        ExtractDateTimePartEnum.Microseconds => "microseconds",
        ExtractDateTimePartEnum.Millennium => "millennium",
        ExtractDateTimePartEnum.Milliseconds => "milliseconds",
        ExtractDateTimePartEnum.Minute => "minute",
        ExtractDateTimePartEnum.Month => "month",
        ExtractDateTimePartEnum.Quarter => "quarter",
        ExtractDateTimePartEnum.Second => "second",
        ExtractDateTimePartEnum.Timezone => "timezone",
        ExtractDateTimePartEnum.TimezoneHour => "timezone_hour",
        ExtractDateTimePartEnum.TimezoneMinute => "timezone_minute",
        ExtractDateTimePartEnum.Week => "week",
        ExtractDateTimePartEnum.Year => "year",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };
}
