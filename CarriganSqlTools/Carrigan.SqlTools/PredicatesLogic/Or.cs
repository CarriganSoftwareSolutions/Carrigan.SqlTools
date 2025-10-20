namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s logical <c>OR</c> operator for combining one or more predicate expressions
/// in <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// OR example, note it intelligently handles more than two predicates.
/// <see cref="ColumnValue{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
/// ColumnValue<Customer> equalEmail = new(nameof(Customer.Email), "Hank@example.com");
/// ColumnValue<Customer> equalPhone = new(nameof(Customer.Phone), "+1(555)555-5555");
/// Or or = new(equalName, equalEmail, equalPhone);
/// 
/// SqlQuery query = customerGenerator.Select(null, null, or, null, null);
/// ]]></code>
/// 
/// <para>Resulting SQL:</para>
/// 
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE (([Customer].[Name] = @Parameter_Name) 
///    OR ([Customer].[Email] = @Parameter_Email) 
///    OR ([Customer].[Phone] = @Parameter_Phone))
/// ]]></code>
/// </example>
/// 
/// <example>
/// <para>
/// Edge case, single predicates are handled intelligently by OR.
/// <see cref="ColumnValue{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Name", "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// Or or = new(equalName);
/// SqlQuery query = customerGenerator.Select(null, null, or, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class Or : LogicalOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Or"/> class,
    /// representing SQL’s logical <c>OR</c> operator.
    /// </summary>
    /// <remarks>
    /// Behavior:
    /// <list type="bullet">
    ///   <item><description>
    ///   If no predicates are provided, an <see cref="ArgumentNullException"/> is thrown.
    ///   </description></item>
    ///   <item><description>
    ///   If exactly one predicate is provided, it is emitted directly without the <c>OR</c> operator.
    ///   </description></item>
    ///   <item><description>
    ///   If multiple predicates are provided, they are joined with the <c>OR</c> operator and wrapped in parentheses.
    ///   </description></item>
    /// </list>
    /// </remarks>
    /// <param name="predicates">One or more boolean predicate expressions to combine.</param>
    public Or(params IEnumerable<Predicates> predicates) : base ("OR", predicates)
    {
    }
}
