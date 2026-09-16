using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL CONCAT function, which concatenates two or more string expressions into a single string.
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
public class Concat : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "CONCAT";
    /// <summary>
    /// Initializes a new instance of the <see cref="Concat"/> class with the specified SQL expression.
    /// </summary>
    /// <param name="sqlExpression">
    /// The expression to evaluate.
    /// </param>
    public Concat(params IEnumerable<SqlExpression> sqlExpression) : base(ValidateValues(2, sqlExpression))
    {
    }
}
