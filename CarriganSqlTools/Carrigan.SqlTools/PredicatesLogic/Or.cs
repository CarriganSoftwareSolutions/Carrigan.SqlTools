using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's logical OR operator for logical operations on one more predicate values.
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
    /// Constructor for the logical boolean operator "OR".
    /// If no predicate values are passed in, then a <see cref="ArgumentNullException"/> is thrown.
    /// If only one predicate value is provided, then this class is deigned to use just that predicate in place of the logical operator.
    /// If two or more are provided then each predicate is chained together with the OR logical operator.
    /// </summary>
    /// <param name="predicates">One or more boolean predicates.</param>
    public Or(params IEnumerable<Predicates> predicates) : base ("OR", predicates)
    {
    }
}
