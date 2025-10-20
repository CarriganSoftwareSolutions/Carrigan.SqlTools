namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s logical <c>AND</c> operator, used to combine two or more boolean predicates.
/// </summary>
/// <remarks>
/// The <see cref="And"/> operator intelligently handles any number of predicates:
/// <list type="bullet">
/// <item><description>Throws an <see cref="ArgumentNullException"/> if no predicates are provided.</description></item>
/// <item><description>If a single predicate is provided, it is returned directly without additional parentheses or operators.</description></item>
/// <item><description>If multiple predicates are provided, each is chained together using the SQL <c>AND</c> operator.</description></item>
/// </list>
/// </remarks>
/// <example>
/// <para>
/// AND example, note it intelligently handles more than two predicates.
/// Note: <see cref="ColumnValue{T}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
/// ColumnValue<Customer> equalEmail = new(nameof(Customer.Email), "Hank@example.com");
/// ColumnValue<Customer> equalPhone = new(nameof(Customer.Phone), "+1(555)555-5555");
/// And and = new(equalName, equalEmail, equalPhone);
///
/// SqlQuery query = customerGenerator.Select(null, null, and, null, null);
/// ]]></code>
/// 
/// <para>Resulting SQL:</para>
/// 
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE (([Customer].[Name] = @Parameter_Name) 
///     AND ([Customer].[Email] = @Parameter_Email) 
///     AND ([Customer].[Phone] = @Parameter_Phone))
/// ]]></code>
/// </example>
/// 
/// <example>
/// <para>Edge case, single predicates are handled intelligently by AND.</para>
/// <code language="csharp"><![CDATA[
/// ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
/// And and = new(equalName);
/// SqlQuery query = customerGenerator.Select(null, null, and, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
/// 
public class And : LogicalOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="And"/> class, representing
    /// the SQL logical <c>AND</c> operator.
    /// </summary>
    /// <param name="predicates">
    /// One or more boolean predicates to combine using <c>AND</c>.
    /// </param>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Throws an <see cref="ArgumentNullException"/> if no predicates are provided.</description></item>
    /// <item><description>If only one predicate is provided, that predicate is used directly.</description></item>
    /// </list>
    /// </remarks>
    public And(params IEnumerable<Predicates> predicates) : base("AND", predicates)
    {
    }
}
