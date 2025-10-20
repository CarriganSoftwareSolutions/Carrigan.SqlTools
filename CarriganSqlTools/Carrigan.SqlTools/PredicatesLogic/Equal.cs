namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s equality (<c>=</c>) comparison operator, used in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref = "Column{T}" /> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Name", "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, null, equalName, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class Equal : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Equal"/> class,
    /// representing a predicate that compares two values for equality (<c>=</c>).
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison.
    /// Typically a <see cref="Column{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison.
    /// Typically a <see cref="Parameter"/> or another <see cref="Predicates"/> expression.
    /// </param>
    public Equal(Predicates left, Predicates right) : base (left, right, "=")
    {
    }
}
