using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.SqlGenerators;
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
/// Abs abs = new(new Column<Order>(nameof(Order.Total)));
/// SelectTags selects = new(new SelectTag(abs, "AbsValue"));
/// 
/// SelectBuilder<Order> selectBuilder = new()
/// {
///     Selects = selects
/// };
/// 
/// SqlQuery query = orderGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT ABS([Order].[Total]) AS [AbsValue] FROM [Order]
/// --PostgreSql
/// SELECT ABS(\"Order\".\"Total\") AS \"AbsValue\" FROM \"Order\"
/// ]]></code>
/// </example>
public class Abs : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName => 
        "ABS";
    /// <summary>
    /// Initializes a new instance of the <see cref="Abs"/> class with the specified SQL expression.
    /// </summary>
    /// <param name="sqlExpression">
    /// The expression to evaluate.
    /// </param>
    public Abs(SqlExpression sqlExpression) : 
        base([sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression))])
    {
    }
}
