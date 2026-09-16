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
/// Represents the SQL UPPER function, which converts a string to uppercase.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Upper expression = new(new Column<Customer>(nameof(Customer.Name)));
/// SelectTags selects = new(new SelectTag(expression, "Value"));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = selects
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT UPPER([Customer].[Name]) AS [Value] FROM [Customer]
/// --PostgreSql
/// SELECT UPPER(\"Customer\".\"Name\") AS \"Value\" FROM \"Customer\"
/// ]]></code>
/// </example>
public class Upper : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "UPPER";

    /// <summary>
    /// Initializes a new instance of the <see cref="Upper"/> class with the specified SQL expression.
    /// </summary>
    /// <param name="sqlExpression">
    /// The expression to evaluate.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is null.
    /// </exception>
    public Upper(SqlExpression sqlExpression) : base([sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression))])
    { }
}
