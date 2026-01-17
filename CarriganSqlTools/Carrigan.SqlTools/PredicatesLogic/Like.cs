namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's <c>LIKE</c> comparison operator, used for pattern matching
/// in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterEmail = new("Email", "%@example.com");
/// Column<Customer> columnEmail = new(nameof(Customer.Email));
/// Like predicate = new(columnEmail, parameterEmail);
/// SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Email] 
/// LIKE @Parameter_Email)
/// ]]></code>
/// </example>
public class Like : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Like"/> class,
    /// representing a predicate that performs a pattern match using SQL's
    /// <c>LIKE</c> operator.
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison, typically a <see cref="Column{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison, typically a <see cref="Parameter"/> or another <see cref="Predicates"/> expression.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
    /// </exception>
    public Like(Predicates left, Predicates right) : base(left, right, "LIKE")
    {
    }
}
