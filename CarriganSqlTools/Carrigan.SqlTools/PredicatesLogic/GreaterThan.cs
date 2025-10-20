namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s greater-than (<c>&gt;</c>) comparison operator,
/// used to compare two expressions within <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref = "Column{T}" /> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterTotal = new("Total", 1776.00m);
/// Column<Order> columnTotal = new(nameof(Order.Total));
/// GreaterThan predicate = new(columnTotal, parameterTotal);
/// SqlQuery query = orderGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Order].* 
/// FROM [Order] 
/// WHERE ([Order].[Total] > @Parameter_Total)
/// ]]></code>
/// </example>
public class GreaterThan : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThan"/> class,
    /// representing a predicate that compares two values using the SQL
    /// greater-than (<c>&gt;</c>) operator.
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison, typically a <see cref="Column{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison, typically a <see cref="Parameter"/> or another <see cref="Predicates"/> expression.
    /// </param>
    public GreaterThan(Predicates left, Predicates right) : base (left, right, ">")
    {
    }
}
