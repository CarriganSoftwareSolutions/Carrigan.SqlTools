using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

//IGNORE SPELLING: dayofyear, tzoffset

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server DATEPART function.
/// </summary>
public class DatePart : SqlExpression
{
    private readonly string _datePart;

    public DatePart(SharedDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public DatePart(DatePartDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public override bool IsAggregate() => ChildNodes.Single().IsAggregate();

    protected override bool EqualsCore(SqlExpression other) =>
        other is DatePart datePart && string.Equals(_datePart, datePart._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("DATEPART(");
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

    private static string GetDatePart(DatePartDateTimePartEnum datePart) => datePart switch
    {
        DatePartDateTimePartEnum.Year => "year",
        DatePartDateTimePartEnum.Quarter => "quarter",
        DatePartDateTimePartEnum.Month => "month",
        DatePartDateTimePartEnum.DayOfYear => "dayofyear",
        DatePartDateTimePartEnum.Day => "day",
        DatePartDateTimePartEnum.Week => "week",
        DatePartDateTimePartEnum.Weekday => "weekday",
        DatePartDateTimePartEnum.Hour => "hour",
        DatePartDateTimePartEnum.Minute => "minute",
        DatePartDateTimePartEnum.Second => "second",
        DatePartDateTimePartEnum.Millisecond => "millisecond",
        DatePartDateTimePartEnum.Microsecond => "microsecond",
        DatePartDateTimePartEnum.Nanosecond => "nanosecond",
        DatePartDateTimePartEnum.IsoWeek => "iso_week",
        DatePartDateTimePartEnum.TimezoneOffset => "tzoffset",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };
}
