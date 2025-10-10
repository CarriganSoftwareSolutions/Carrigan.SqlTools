namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's logical AND operator for logical operations on one more predicate values.
/// </summary>
/// <example>
/// <para>AND example, note it intelligently handles more than two predicates.</para>
/// 
/// <code language="csharp"><![CDATA[
/// Parameters parameterName = new("Name", "Hank");
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// 
/// Parameters parameterEmail = new("Email", "Hank@example.com");
/// Columns&lt;Customer&gt; columnEmail = new(nameof(Customer.Email));
/// Equal equalEmail = new(columnEmail, parameterEmail);
/// 
/// Parameters parameterPhone = new("Phone", ("+1(555)555-5555"));
/// Columns&lt;Customer&gt; columnPhone = new(nameof(Customer.Phone));
/// Equal equalPhone = new(columnPhone, parameterPhone);
/// 
/// And and = new(equalName, equalEmail, equalPhone);
/// 
/// SqlQuery query = customerGenerator.Select(null, and, null, null);
/// ]]></code>
/// 
/// <para>Resulting SQL:</para>
/// 
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// WHERE (([Customer].[Name] = @Parameter_Name) 
/// AND ([Customer].[Email] = @Parameter_Email) 
/// AND ([Customer].[Phone] = @Parameter_Phone))
/// ]]></code>
/// </example>
/// 
/// <example>
/// <para>Edge case, single predicates are handled intelligently by AND.</para>
/// <code language="csharp"><![CDATA[
///  Parameters parameterName = new("Name", "Hank");
///  Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
///  Equal equalName = new(columnName, parameterName);
///  And and = new(equalName);
///  SqlQuery query = customerGenerator.Select(null, and, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
/// 
public class And : LogicalOperator
{
    /// <summary>
    /// Constructor for the logical boolean operator "AND".
    /// If no predicate values are passed in, then a <see cref="ArgumentNullException"/> is thrown.
    /// If only one predicate value is provided, then this class is deigned to use just that predicate in place of the logical operator.
    /// If two or more are provided then each predicate is chained together with the AND logical operator.
    /// </summary>
    /// <param name="predicates">One or more boolean predicates.</param>
    public And(params IEnumerable<Predicates> predicates) : base("AND", predicates)
    {
    }
}
