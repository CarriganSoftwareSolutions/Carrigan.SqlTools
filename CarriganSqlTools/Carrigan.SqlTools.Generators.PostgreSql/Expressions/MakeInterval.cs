using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

//IGNORE Spelling: mins

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents PostgreSQL MAKE_INTERVAL with one explicitly selected interval component.
/// API consumers can combine the resulting interval with Carrigan.SqlTools arithmetic expressions for date/time arithmetic.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// MakeInterval expression = new(MakeIntervalDateTimePartEnum.Day, new Parameter(2));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT MAKE_INTERVAL(days => $1) AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class MakeInterval : SqlExpression
{
    /// <summary>
    /// Gets the date part to create an interval for, as a string.
    /// </summary>
    private readonly string _datePart;

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeInterval"/> class.
    /// </summary>
    /// <param name="datePart">
    /// The date part to create an interval for.
    /// </param>
    /// <param name="expression">
    /// The SQL expression representing the value for the specified interval component.
    /// </param>
    public MakeInterval(SharedDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeInterval"/> class.
    /// </summary>
    /// <param name="datePart">
    /// The date part to create an interval for.
    /// </param>
    /// <param name="expression">
    /// The SQL expression representing the value for the specified interval component.
    /// </param>
    public MakeInterval(MakeIntervalDateTimePartEnum datePart, SqlExpression expression) : base([ValidateValue(expression)]) =>
        _datePart = GetDatePart(datePart);

    /// <summary>
    /// Determines whether the expression is an aggregate function.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the expression is an aggregate function; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool IsAggregate() => ChildNodes.Single().IsAggregate();

    /// <summary>
    /// Determines whether the specified <see cref="SqlExpression"/> is equal to the current <see cref="MakeInterval"/> instance.
    /// </summary>
    /// <param name="other">
    /// The <see cref="SqlExpression"/> to compare with the current instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="SqlExpression"/> is equal to the current instance; otherwise, <see langword="false"/>.
    /// </returns>
    protected override bool EqualsCore(SqlExpression other) =>
        other is MakeInterval makeInterval && string.Equals(_datePart, makeInterval._datePart, StringComparison.Ordinal) && base.EqualsCore(other);

    /// <summary>
    /// Adds the current expression's components to the specified hash code.
    /// </summary>
    /// <param name="hashCode">
    ///     The hash code to which the current expression's components will be added.
    /// </param>
    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_datePart, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

    /// <summary>
    /// Converts the current expression to a sequence of SQL fragments for the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect for which to generate the SQL fragments.
    /// </param>
    /// <returns>
    /// A sequence of SQL fragments representing the current expression in the specified SQL dialect.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("MAKE_INTERVAL(");
        yield return new SqlFragmentText(_datePart);
        yield return new SqlFragmentText(" => ");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return ISqlFragment.CloseParentheses;
    }

    /// <summary>
    /// Returns the string representation of the specified date part enum value.
    /// </summary>
    /// <param name="datePart">
    /// The date part enum value for which to get the string representation.
    /// </param>
    /// <returns>
    /// A string representing the specified date part enum value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified date part enum value is not a valid value of <see cref="SharedDateTimePartEnum"/>.
    /// </exception>
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

    /// <summary>
    /// Returns the string representation of the specified date part enum value for MAKE_INTERVAL.
    /// </summary>
    /// <param name="datePart">
    /// The date part enum value for which to get the string representation.
    /// </param>
    /// <returns>
    /// A string representing the specified date part enum value for MAKE_INTERVAL.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified date part enum value is not a valid value of <see cref="MakeIntervalDateTimePartEnum"/>.
    /// </exception>
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
