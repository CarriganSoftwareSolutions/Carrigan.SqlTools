using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL SIGN function, which returns the sign of a number.
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
public class Sign : FunctionalExpression
{
    protected override string FunctionName =>
        "SIGN";

    public Sign(SqlExpression sqlExpression) : base([sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression))])
    { }
}
