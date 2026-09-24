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

    public DateFromParts(SqlExpression year, SqlExpression month, SqlExpression day)
        : base([ValidateValue(year), ValidateValue(month), ValidateValue(day)])
    {
    }

    public DateFromParts(int year, int month, int day)
        : base([ValidateParameterValue(year), ValidateParameterValue(month), ValidateParameterValue(day)])
    {
    }
}
