using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL MAKE_DATE function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// MakeDate expression = new(new Parameter(2026), new Parameter(9), new Parameter(23));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT MAKE_DATE($1, $2, $3) AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class MakeDate : FunctionalExpression
{
    /// <summary>
    /// Gets the PostgreSQL function name.
    /// </summary>
    protected override string FunctionName => "MAKE_DATE";

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeDate"/> class.
    /// </summary>
    /// <param name="year">
    /// The year expression.
    /// </param>
    /// <param name="month">
    /// The month expression.
    /// </param>
    /// <param name="day">
    /// The day expression.
    /// </param>
    public MakeDate(SqlExpression year, SqlExpression month, SqlExpression day)
        : base([ValidateValue(year), ValidateValue(month), ValidateValue(day)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeDate"/> class with constant year, month, and day values.
    /// </summary>
    /// <param name="year">
    /// The year value.
    /// </param>
    /// <param name="month">
    /// The month value.
    /// </param>
    /// <param name="day">
    /// The day value.
    /// </param>
    public MakeDate(int year, int month, int day)
        : base([ValidateParameterValue(year), ValidateParameterValue(month), ValidateParameterValue(day)])
    {
    }
}
