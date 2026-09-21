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
public class Floor : FunctionalExpression
{
    /// <summary>
    /// 
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
