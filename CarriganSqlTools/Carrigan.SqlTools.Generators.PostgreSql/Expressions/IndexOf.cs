using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Alias used to get the PostgreSQL <c>STRPOS</c> function.
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
/// SELECT STRPOS("Customer"."Name", $1) AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class IndexOf : StrPos
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOf"/> class with a constant search string.
    /// </summary>
    /// <param name="expression">The string expression to search.</param>
    /// <param name="find">The string to find. The value is bound through a SQL parameter.</param>
    public IndexOf(SqlExpression expression, string find) : base(expression, find)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOf"/> class with a search value supplied by another SQL expression.
    /// </summary>
    /// <param name="expression">The string expression to search.</param>
    /// <param name="find">The SQL expression that supplies the string to find.</param>
    public IndexOf(SqlExpression expression, SqlExpression find) : base(expression, find)
    {
    }
}
