using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL TRUNC function, which truncates a numeric value to a specified number of decimal places.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Truncate expression = new(new Column<Order>(nameof(Order.Total)), 2);
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
/// SELECT TRUNC(\"Order\".\"Total\", ) AS \"Value\" FROM \"Order\"
/// ]]></code>
/// </example>
public class Truncate : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "TRUNC";

    /// <summary>
    /// Initializes a new instance of the <see cref="Truncate"/> class with the specified expression.
    /// </summary>
    /// <param name="expression">
    /// The SQL expression to truncate.
    /// </param>
    public Truncate(SqlExpression expression) : base([ValidateValue(expression), ValidateParameterValue(0)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Truncate"/> class with the specified expression and precision.
    /// </summary>
    /// <param name="expression">
    /// The SQL expression to truncate.
    /// </param>
    /// <param name="precision">
    /// The number of decimal places to truncate to.
    /// </param>
    public Truncate(SqlExpression expression, int precision) : base([ValidateValue(expression), ValidateParameterValue(precision)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Truncate"/> class with the specified expression and precision defined by another SQL expression.
    /// </summary>
    /// <param name="expression">
    /// The SQL expression to truncate.
    /// </param>
    /// <param name="precision">
    /// The SQL expression defining the precision to truncate to.
    /// </param>
    public Truncate(SqlExpression expression, SqlExpression precision) : base([ValidateValue(expression), ValidateValue(precision)])
    {
    }
}
