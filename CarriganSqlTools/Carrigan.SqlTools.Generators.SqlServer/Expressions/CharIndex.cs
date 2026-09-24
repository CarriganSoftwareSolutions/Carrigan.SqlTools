using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server <c>CHARINDEX</c> function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// CharIndex expression = new("a", new Column<Customer>(nameof(Customer.Name)));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = expression.AsSelectTag("Value")
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT CHARINDEX(@Parameter_1, [Customer].[Name]) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class CharIndex : FunctionalExpression
{
    /// <summary>
    /// Gets the SQL Server function name.
    /// </summary>
    protected override string FunctionName =>
        "CHARINDEX";

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOf"/> class with a constant search string.
    /// </summary>
    /// <param name="find">The string to find. The value is bound through a SQL parameter.</param>
    /// <param name="expression">The string expression to search.</param>
    public CharIndex(string find, SqlExpression expression) : base([ValidateParameterValue(find), ValidateValue(expression), ])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOf"/> class with a search value supplied by another SQL expression.
    /// </summary>
    /// <param name="find">The SQL expression that supplies the string to find.</param>
    /// <param name="expression">The string expression to search.</param>
    public CharIndex(SqlExpression find, SqlExpression expression) : base([ValidateValue(find), ValidateValue(expression)])
    {
    }
}
