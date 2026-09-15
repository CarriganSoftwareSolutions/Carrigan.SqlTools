using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL <c>NULLIF</c> expression that returns <c>null</c> if two expressions are equal, otherwise returns the first expression.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// NullIf nullIf = new(new Column<Customer>(nameof(Customer.Phone)), new Parameter(string.Empty));
/// SelectTags selects = new(new SelectTag(nullIf, "NullIf"));
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
/// SELECT NULLIF([Customer].[Phone], @Parameter_1) AS [NullIf] FROM [Customer]
/// --PostgreSql
/// SELECT NULLIF(\"Customer\".\"Phone\", $1) AS \"NullIf\" FROM \"Customer\"
/// ]]></code>
/// </example>
public class NullIf : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName => 
        "NULLIF";
    /// <summary>
    /// Initializes a new instance of the <see cref="NullIf"/> class with the specified left and right values.
    /// </summary>
    /// <param name="leftValue">
    /// The expression to evaluate and return if the right value is not equal to it.
    /// </param>
    /// <param name="rightValue">
    /// The expression to compare with the left value.
    /// </param>
    public NullIf(SqlExpression leftValue, SqlExpression rightValue) : 
        base([leftValue ?? throw new ArgumentNullException(nameof(leftValue)), rightValue ?? throw new ArgumentNullException(nameof(rightValue))])
    {
    }
}
