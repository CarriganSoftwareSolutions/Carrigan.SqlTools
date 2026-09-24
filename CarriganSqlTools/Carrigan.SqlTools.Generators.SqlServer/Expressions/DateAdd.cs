using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

//IGNORE SPELLING: dayofyear

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server DATEADD function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// DateAdd expression = new(DateAddDateTimePartEnum.Day, new Parameter(2), new Parameter(new DateTime(2026, 9, 23)));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT DATEADD(day, @Parameter_1, @Parameter_2) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class DateAdd : SqlExpression
{
    private readonly string _datePart;

    public DateAdd(SharedDateTimePartEnum datePart, SqlExpression number, SqlExpression date)
        : base([ValidateValue(number), ValidateValue(date)]) => _datePart = GetDatePart(datePart);

    public DateAdd(DateAddDateTimePartEnum datePart, SqlExpression number, SqlExpression date)
        : base([ValidateValue(number), ValidateValue(date)]) => _datePart = GetDatePart(datePart);

    public override bool IsAggregate()
    {
        bool[] aggregateStates = [.. ChildNodes
            .Select(expression => (Expression: expression, IsAggregate: expression.IsAggregate()))
            .Where(candidate => candidate.IsAggregate || candidate.Expression.HasColumns())
            .Select(static candidate => candidate.IsAggregate)];

        if (aggregateStates.AllEqual() is false)
            throw new AggregateInconsistencyException();

        return aggregateStates.FirstOrDefault();
    }

    protected override bool EqualsCore(SqlExpression other) =>
        other is DateAdd dateAdd && string.Equals(_datePart, dateAdd._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("DATEADD(");
        yield return new SqlFragmentText(_datePart);
        yield return ISqlFragment.CommaSpace;
        foreach (ISqlFragment fragment in ChildNodes.ElementAt(0).ToSqlFragments(dialect))
            yield return fragment;
        yield return ISqlFragment.CommaSpace;
        foreach (ISqlFragment fragment in ChildNodes.ElementAt(1).ToSqlFragments(dialect))
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

    private static string GetDatePart(DateAddDateTimePartEnum datePart) => datePart switch
    {
        DateAddDateTimePartEnum.Year => "year",
        DateAddDateTimePartEnum.Quarter => "quarter",
        DateAddDateTimePartEnum.Month => "month",
        DateAddDateTimePartEnum.DayOfYear => "dayofyear",
        DateAddDateTimePartEnum.Day => "day",
        DateAddDateTimePartEnum.Week => "week",
        DateAddDateTimePartEnum.Weekday => "weekday",
        DateAddDateTimePartEnum.Hour => "hour",
        DateAddDateTimePartEnum.Minute => "minute",
        DateAddDateTimePartEnum.Second => "second",
        DateAddDateTimePartEnum.Millisecond => "millisecond",
        DateAddDateTimePartEnum.Microsecond => "microsecond",
        DateAddDateTimePartEnum.Nanosecond => "nanosecond",
        _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
    };
}
