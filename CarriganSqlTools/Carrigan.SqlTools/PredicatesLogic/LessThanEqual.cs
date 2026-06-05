using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's less-than-or-equal (<c>&lt;=</c>) comparison operator,
/// used to compare two expressions within <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterTotal = new(1776.00m, "Total");
/// Column<Order> columnTotal = new(nameof(Order.Total));
/// LessThanEqual predicate = new(columnTotal, parameterTotal);
/// SelectBuilder<Order> selectBuilder = new()
/// {
///     Where = predicate
/// };
/// 
/// SqlQuery query = orderGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Order".* 
/// FROM "Order" 
/// WHERE ("Order"."Total" <= $1)
/// 
/// -SqlServer
/// SELECT [Order].*
/// FROM [Order]
/// WHERE ([Order].[Total] <= @Total_1)
/// ]]></code>
/// </example>
public class LessThanEqual : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanEqual"/> class,
    /// representing a predicate that compares two values using the SQL
    /// less-than-or-equal (<c>&lt;=</c>) operator.
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison, typically a <see cref="ColumnBase{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison, typically a <see cref="Parameter"/> or another <see cref="Predicates"/> expression.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
    /// </exception>
    public LessThanEqual(Predicates left, Predicates right) : base(left, right, "<=")
    {
    }
}
