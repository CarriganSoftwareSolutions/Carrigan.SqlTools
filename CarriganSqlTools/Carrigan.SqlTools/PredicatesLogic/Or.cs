using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's logical <c>OR</c> operator, used to combine two or more boolean predicates.
/// </summary>
/// <remarks>
/// The <see cref="Or"/> operator intelligently handles any number of predicates:
/// <list type="bullet">
/// <item><description>Throws an <see cref="ArgumentNullException"/> if no predicates are provided.</description></item>
/// <item><description>If a single predicate is provided, it is returned directly without additional parentheses or operators.</description></item>
/// <item><description>If multiple predicates are provided, each is chained together using the SQL <c>OR</c> operator.</description></item>
/// </list>
/// </remarks>
/// <example>
/// <para>
/// OR example, note it intelligently handles more than two predicates.
/// Note: <see cref="ColumnValueBase{T}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
/// ColumnValue<Customer> equalEmail = new(nameof(Customer.Email), "Hank@example.com");
/// ColumnValue<Customer> equalPhone = new(nameof(Customer.Phone), "+1(555)555-5555");
/// Or or = new(equalName, equalEmail, equalPhone);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = or
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
///
/// <para>Resulting SQL:</para>
///
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer" 
/// WHERE (("Customer"."Name" = $1) 
///    OR ("Customer"."Email" = $2) 
///    OR ("Customer"."Phone" = $3))
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE (([Customer].[Name] = @Name_1)
///     OR ([Customer].[Email] = @Email_2)
///     OR ([Customer].[Phone] = @Phone_3))
/// ]]></code>
/// </example>
///
/// <example>
/// <para>Edge case, single predicates are handled intelligently by OR.</para>
/// <code language="csharp"><![CDATA[
/// ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
/// Or or = new(equalName);
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = or
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer"
/// WHERE ("Customer"."Name" = $1)
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE ([Customer].[Name] = @Name_1)
/// ]]></code>
/// </example>
public class Or : LogicalOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Or"/> class, representing
    /// the SQL logical <c>OR</c> operator.
    /// </summary>
    /// <param name="predicates">
    /// One or more boolean predicates to combine using <c>OR</c>.
    /// </param>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Throws an <see cref="ArgumentNullException"/> if no predicates are provided.</description></item>
    /// <item><description>If only one predicate is provided, that predicate is used directly.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c> or contains no elements.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="predicates"/> contains disallowed <c>null</c> values.
    /// </exception>
    public Or(params IEnumerable<Predicates> predicates) : base("OR", predicates)
    {
    }
}
