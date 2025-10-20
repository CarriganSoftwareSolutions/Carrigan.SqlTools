namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s greater-than-or-equal-to (<c>&gt;=</c>) comparison operator,
/// used to compare two expressions within <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameter parameterTotal = new("Total", 1776.00m);
/// Column<Order> columnTotal = new(nameof(Order.Total));
/// GreaterThanEqual predicate = new(columnTotal, parameterTotal);
/// SqlQuery query = orderGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Order].* 
/// FROM [Order] 
/// WHERE ([Order].[Total] >= @Parameter_Total)
/// ]]></code>
/// </example>
public class GreaterThanEqual : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanEqual"/> class,
    /// representing a predicate that compares two values using the SQL
    /// greater-than-or-equal-to (<c>&gt;=</c>) operator.
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison, typically a <see cref="Column{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison, typically a <see cref="Parameter"/> or another <see cref="Predicates"/> expression.
    /// </param>
    public GreaterThanEqual(Predicates left, Predicates right) : base (left, right, ">=")
    {
    }
}
