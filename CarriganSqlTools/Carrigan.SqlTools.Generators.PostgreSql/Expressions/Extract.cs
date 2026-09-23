using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

//IGNORE SPELLING: doy, isodow, isoyear, julian

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL EXTRACT expression.
/// </summary>
public class Extract : SqlExpression
{
    private readonly string _datePart;

    public Extract(SharedDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public Extract(ExtractDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public override bool IsAggregate() => ChildNodes.Single().IsAggregate();

    protected override bool EqualsCore(SqlExpression other) =>
        other is Extract extract && string.Equals(_datePart, extract._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("EXTRACT(");
        yield return new SqlFragmentText(_datePart);
        yield return new SqlFragmentText(" FROM ");
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
