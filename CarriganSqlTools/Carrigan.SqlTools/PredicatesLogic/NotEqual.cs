using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's not-equal (<c>&lt;&gt;</c>) comparison operator,
/// used in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Hank", "Name");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// NotEqual predicate = new(columnName, parameterName);
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = predicate
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer" 
/// WHERE ("Customer"."Name" <> $1)
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE ([Customer].[Name] <> @Name_1)
/// ]]></code>
/// </example>
public class NotEqual : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotEqual"/> class,
    /// representing a predicate that compares two values using the SQL
    /// not-equal (<c>&lt;&gt;</c>) operator.
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison, typically a <see cref="ColumnBase{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison, typically a <see cref="Parameter"/> or another <see cref="SqlExpression"/> expression.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
    /// </exception>
    public NotEqual(SqlExpression left, SqlExpression right) : base(left, right, "<>")
    {
    }
}
