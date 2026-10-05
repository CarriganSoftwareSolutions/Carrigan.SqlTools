using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server EOMONTH function. EOMONTH is available in SQL Server 2012 (11.x) and later.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// EOMonth expression = new(new Parameter(new DateTime(2026, 9, 23)));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT EOMONTH(@Parameter_1) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class EOMonth : FunctionalExpression
{
    protected override string FunctionName => "EOMONTH";

    /// <summary>
    /// Initializes an <c>EOMONTH</c> expression for the month containing <paramref name="startDate"/>.
    /// </summary>
    /// <param name="startDate">The date expression whose month end is returned.</param>
    public EOMonth(SqlExpression startDate) : base([ValidateValue(startDate)])
    {
    }

    /// <summary>
    /// Initializes an <c>EOMONTH</c> expression with an optional month offset.
    /// </summary>
    /// <param name="startDate">The date expression used as the starting month.</param>
    /// <param name="monthToAdd">The expression that yields the number of months to add before determining the month end.</param>
    public EOMonth(SqlExpression startDate, SqlExpression monthToAdd)
        : base([ValidateValue(startDate), ValidateValue(monthToAdd)])
    {
    }
}
