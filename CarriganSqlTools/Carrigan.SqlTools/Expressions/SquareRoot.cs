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
/// Represents the SQL SQRT function, which calculates the square root of a given expression.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// SquareRoot expression = new(new Column<Order>(nameof(Order.Total)));
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
/// SELECT SQRT([Order].[Total]) AS [Value] FROM [Order]
/// --PostgreSql
/// SELECT SQRT(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"
/// ]]></code>
/// </example>
public class SquareRoot : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SquareRoot"/> class that calculates the square root of the specified SQL expression.
    /// </summary>
    /// <param name="expression">
    /// The expression to evaluate.
    /// </param>
    public SquareRoot(SqlExpression expression) : base([ValidateValue(expression)])
    {

    }

    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "SQRT";
}
