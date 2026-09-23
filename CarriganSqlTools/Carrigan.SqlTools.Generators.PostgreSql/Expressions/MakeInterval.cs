using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

//IGNORE Spelling: mins

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents PostgreSQL MAKE_INTERVAL with one explicitly selected interval component.
/// API consumers can combine the resulting interval with Carrigan.SqlTools arithmetic expressions for date/time arithmetic.
/// </summary>
public class MakeInterval : SqlExpression
{
    private readonly string _datePart;

    public MakeInterval(SharedDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public MakeInterval(MakeIntervalDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    public override bool IsAggregate() => ChildNodes.Single().IsAggregate();

    protected override bool EqualsCore(SqlExpression other) =>
        other is MakeInterval makeInterval && string.Equals(_datePart, makeInterval._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("MAKE_INTERVAL(");
        yield return new SqlFragmentText(_datePart);
        yield return new SqlFragmentText(" => ");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return ISqlFragment.CloseParentheses;
    }

    private static string GetDatePart(SharedDateTimePartEnum datePart) => datePart switch
    {
        SharedDateTimePartEnum.Year => "years",
        SharedDateTimePartEnum.Month => "months",
        SharedDateTimePartEnum.Week => "weeks",
        SharedDateTimePartEnum.Day => "days",
        SharedDateTimePartEnum.Hour => "hours",
        SharedDateTimePartEnum.Minute => "mins",
        SharedDateTimePartEnum.Second => "secs",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };

    private static string GetDatePart(MakeIntervalDateTimePartEnum datePart) => datePart switch
    {
        MakeIntervalDateTimePartEnum.Year => "years",
        MakeIntervalDateTimePartEnum.Month => "months",
        MakeIntervalDateTimePartEnum.Week => "weeks",
        MakeIntervalDateTimePartEnum.Day => "days",
        MakeIntervalDateTimePartEnum.Hour => "hours",
        MakeIntervalDateTimePartEnum.Minute => "mins",
        MakeIntervalDateTimePartEnum.Second => "secs",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };
}
