using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Alias class for the SQL Server <c>CHARINDEX</c> function while exposing the same value-first API as the PostgreSQL <c>STRPOS</c> expression.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// IndexOf expression = new(new Column<Customer>(nameof(Customer.Name)), "a");
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
public class IndexOf : CharIndex
{
    /// <summary>
    /// Gets the SQL Server function name.
    /// </summary>
    protected override string FunctionName =>
        "CHARINDEX";

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOf"/> class with a constant search string.
    /// </summary>
    /// <param name="expression">The string expression to search.</param>
    /// <param name="find">The string to find. The value is bound through a SQL parameter.</param>
    public IndexOf(SqlExpression expression, string find) : base(find, expression)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOf"/> class with a search value supplied by another SQL expression.
    /// </summary>
    /// <param name="expression">The string expression to search.</param>
    /// <param name="find">The SQL expression that supplies the string to find.</param>
    public IndexOf(SqlExpression expression, SqlExpression find) : base(find, expression)
    {
    }
}
