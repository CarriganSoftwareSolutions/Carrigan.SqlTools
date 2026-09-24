using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL <c>LENGTH</c> function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Length expression = new(new Column<Customer>(nameof(Customer.Name)));
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
/// SELECT LENGTH("Customer"."Name") AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class Length : FunctionalExpression
{
    /// <summary>
    /// Gets the PostgreSQL function name.
    /// </summary>
    protected override string FunctionName =>
        "LENGTH";

    /// <summary>
    /// Initializes a new instance of the <see cref="Length"/> class.
    /// </summary>
    /// <param name="expression">The string expression whose length is returned.</param>
    public Length(SqlExpression expression) : base([ValidateValue(expression)])
    {
    }
}
