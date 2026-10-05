using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server DATEFROMPARTS function. DATEFROMPARTS is available in SQL Server 2012 (11.x) and later.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// DateFromParts expression = new(new Parameter(2026), new Parameter(9), new Parameter(23));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT DATEFROMPARTS(@Parameter_1, @Parameter_2, @Parameter_3) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class DateFromParts : FunctionalExpression
{
    protected override string FunctionName => "DATEFROMPARTS";

    /// <summary>
    /// Initializes a <c>DATEFROMPARTS</c> expression from SQL expressions representing the year, month, and day.
    /// </summary>
    /// <param name="year">The expression that yields the year component.</param>
    /// <param name="month">The expression that yields the month component.</param>
    /// <param name="day">The expression that yields the day component.</param>
    public DateFromParts(SqlExpression year, SqlExpression month, SqlExpression day)
        : base([ValidateValue(year), ValidateValue(month), ValidateValue(day)])
    {
    }

    /// <summary>
    /// Initializes a <c>DATEFROMPARTS</c> expression from integer date components, represented as SQL parameters.
    /// </summary>
    /// <param name="year">The year component.</param>
    /// <param name="month">The month component.</param>
    /// <param name="day">The day component.</param>
    public DateFromParts(int year, int month, int day)
        : base([ValidateParameterValue(year), ValidateParameterValue(month), ValidateParameterValue(day)])
    {
    }
}
