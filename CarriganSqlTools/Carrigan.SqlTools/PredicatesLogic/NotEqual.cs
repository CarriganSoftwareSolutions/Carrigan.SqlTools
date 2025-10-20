namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s <c>&lt;&gt;</c> (Not Equal) comparison operator,
/// used to test inequality between two expressions in <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Name", "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// NotEqual predicate = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] <> @Parameter_Name)
/// ]]></code>
/// </example>
public class NotEqual : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotEqual"/> class,
    /// representing SQL’s <c>&lt;&gt;</c> (Not Equal) comparison operator.
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison, typically a <see cref="Column{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison, typically a <see cref="Parameter"/> or another <see cref="Predicates"/> expression.
    /// </param>
    public NotEqual(Predicates left, Predicates right) : base (left, right, "<>")
    {
    }
}
