using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

//IGNORE SPELLING: TRUNC

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL DATE_TRUNC function.
/// </summary>
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
        yield return new SqlFragmentText("DATE_TRUNC('");
        yield return new SqlFragmentText(_datePart);
        yield return new SqlFragmentText("', ");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return ISqlFragment.CloseParentheses;
    }

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
