using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// 
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Power expression = new(new Column<Order>(nameof(Order.Total)), 2);
/// 
/// SelectBuilder<Order> selectBuilder = new()
/// {
///     Selects = expression.AsSelectTag("Value")
/// };
/// 
/// SqlQuery query = orderGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT POWER([Order].[Total], @Parameter_1) AS [Value] FROM [Order]
/// --PostgreSql
/// SELECT POWER(\"Order\".\"Total\", $1) AS \"Value\" FROM \"Order\"
/// ]]></code>
/// </example>
public class Power : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Power"/> class.
    /// </summary>
    /// <param name="baseNumber">The base value.</param>
    /// <param name="exponent">The exponent value.</param>
    public Power(SqlExpression baseNumber, SqlExpression exponent) : base([ValidateValue(baseNumber), ValidateValue(exponent)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Power"/> class.
    /// </summary>
    /// <param name="baseNumber">The base value.</param>
    /// <param name="number">The exponent value.</param>
    public Power(SqlExpression baseNumber, int number) : base([ValidateValue(baseNumber), new Parameter(number)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Power"/> class.
    /// </summary>
    /// <param name="baseNumber">The base value.</param>
    /// <param name="number">The exponent value.</param>
    public Power(SqlExpression baseNumber, float number) : base([ValidateValue(baseNumber), new Parameter(number)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Power"/> class.
    /// </summary>
    /// <param name="baseNumber">The base value.</param>
    /// <param name="number">The exponent value.</param>
    public Power(SqlExpression baseNumber, double number) : base([ValidateValue(baseNumber), new Parameter(number)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Power"/> class.
    /// </summary>
    /// <param name="baseNumber">The base value.</param>
    /// <param name="number">The exponent value.</param>
    public Power(SqlExpression baseNumber, decimal number) : base([ValidateValue(baseNumber), new Parameter(number)])
    {
    }

    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "POWER";
}
