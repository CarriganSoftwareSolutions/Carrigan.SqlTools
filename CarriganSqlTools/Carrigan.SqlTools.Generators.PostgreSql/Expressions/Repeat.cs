using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL <c>REPEAT</c> function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Repeat expression = new(new Column<Customer>(nameof(Customer.Name)), 2);
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
/// SELECT REPEAT("Customer"."Name", $1) AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class Repeat : FunctionalExpression
{
    /// <summary>
    /// Gets the PostgreSQL function name.
    /// </summary>
    protected override string FunctionName =>
        "REPEAT";

    /// <summary>
    /// Initializes a new instance of the <see cref="Repeat"/> class with a constant repeat count.
    /// </summary>
    /// <param name="expression">The string expression to repeat.</param>
    /// <param name="count">The number of repetitions. The value is bound through a SQL parameter.</param>
    public Repeat(SqlExpression expression, int count) :  base([ValidateValue(expression), ValidateParameterValue(count)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Repeat"/> class with a repeat count supplied by another SQL expression.
    /// </summary>
    /// <param name="expression">The string expression to repeat.</param>
    /// <param name="count">The SQL expression that supplies the number of repetitions.</param>
    public Repeat(SqlExpression expression, SqlExpression count) : base([ValidateValue(expression), ValidateValue(count)])
    {
    }
}
