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
/// Represents the SQL SIGN function, which returns the sign of a number.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Sign expression = new(new Column<Order>(nameof(Order.Total)));
/// SelectTags selects = new(new SelectTag(expression, "Value"));
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
/// SELECT SIGN([Order].[Total]) AS [Value] FROM [Order]
/// --PostgreSql
/// SELECT SIGN(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"
/// ]]></code>
/// </example>
public class Sign : FunctionalExpression
{
    protected override string FunctionName =>
        "SIGN";

    public Sign(SqlExpression sqlExpression) : base([sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression))])
    { }
}
