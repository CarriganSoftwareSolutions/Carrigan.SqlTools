using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// 
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
///
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// 
/// --PostgreSql
/// 
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
