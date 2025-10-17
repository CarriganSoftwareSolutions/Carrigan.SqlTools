using Carrigan.SqlTools.SqlGenerators;
using System;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's LIKE comparison operator.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
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
    /// This is the constructor for the classes that represents SQL's LIKE, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public Like(Predicates left, Predicates right) : base (left, right, "LIKE")
    {
    }
}
