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
/// Represents the SQL CEILING function, which returns the smallest integer greater than or equal to a specified numeric expression.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Ceiling expression = new(new Column<Order>(nameof(Order.Total)));
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
/// SELECT CEILING([Order].[Total]) AS [Value] FROM [Order]
/// --PostgreSql
/// SELECT CEILING(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"
/// ]]></code>
/// </example>
public class Ceiling : FunctionalExpression
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Ceiling"/> class that represents the SQL CEILING function for the specified expression.
    /// </summary>
    /// <param name="expression">
    /// The expression to evaluate.
    /// </param>
    public Ceiling(SqlExpression expression) : base([ValidateValue(expression)])
    {

    }

    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "CEILING";
}
