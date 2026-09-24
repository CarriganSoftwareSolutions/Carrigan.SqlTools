using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

//IGNORE SPELLING: TRUNC, dayofyear

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server DATETRUNC function. DATETRUNC requires SQL Server 2022 (16.x) or later.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// DateTrunc expression = new(DateTruncDateTimePartEnum.Month, new Parameter(new DateTime(2026, 9, 23)));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT DATETRUNC(month, @Parameter_1) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class DateTrunc : SqlExpression
{
    private readonly string _datePart;

    public DateTrunc(SharedDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public DateTrunc(DateTruncDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public override bool IsAggregate() => ChildNodes.Single().IsAggregate();

    protected override bool EqualsCore(SqlExpression other) =>
        other is DateTrunc dateTrunc && string.Equals(_datePart, dateTrunc._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("DATETRUNC(");
        yield return new SqlFragmentText(_datePart);
        yield return ISqlFragment.CommaSpace;
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return ISqlFragment.CloseParentheses;
    }

    private static string GetDatePart(SharedDateTimePartEnum datePart) => datePart switch
    {
        SharedDateTimePartEnum.Year => "year",
        SharedDateTimePartEnum.Quarter => "quarter",
        SharedDateTimePartEnum.Month => "month",
        SharedDateTimePartEnum.DayOfYear => "dayofyear",
        SharedDateTimePartEnum.Day => "day",
        SharedDateTimePartEnum.Week => "week",
        SharedDateTimePartEnum.Hour => "hour",
        SharedDateTimePartEnum.Minute => "minute",
        SharedDateTimePartEnum.Second => "second",
        SharedDateTimePartEnum.Millisecond => "millisecond",
        SharedDateTimePartEnum.Microsecond => "microsecond",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };

    private static string GetDatePart(DateTruncDateTimePartEnum datePart) => datePart switch
    {
        DateTruncDateTimePartEnum.Year => "year",
        DateTruncDateTimePartEnum.Quarter => "quarter",
        DateTruncDateTimePartEnum.Month => "month",
        DateTruncDateTimePartEnum.DayOfYear => "dayofyear",
        DateTruncDateTimePartEnum.Day => "day",
        DateTruncDateTimePartEnum.Week => "week",
        DateTruncDateTimePartEnum.IsoWeek => "iso_week",
        DateTruncDateTimePartEnum.Hour => "hour",
        DateTruncDateTimePartEnum.Minute => "minute",
        DateTruncDateTimePartEnum.Second => "second",
        DateTruncDateTimePartEnum.Millisecond => "millisecond",
        DateTruncDateTimePartEnum.Microsecond => "microsecond",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };
}
