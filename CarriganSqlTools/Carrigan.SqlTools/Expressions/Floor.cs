using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL FLOOR function expression.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Floor expression = new(new Column<Order>(nameof(Order.Total)));
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
/// SELECT FLOOR([Order].[Total]) AS [Value] FROM [Order]
/// --PostgreSql
/// SELECT FLOOR(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"
/// ]]></code>
/// </example>
public class Floor : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Floor"/> class that represents the SQL FLOOR function applied to the specified expression.
    /// </summary>
    /// <param name="expression">
    /// The expression to evaluate.
    /// </param>
    public Floor(SqlExpression expression) : base([ValidateValue(expression)])
    {

    }

    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "FLOOR";
}
