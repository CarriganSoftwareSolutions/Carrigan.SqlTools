using Carrigan.SqlTools.SqlGenerators;
using System;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's LIKE comparison operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterEmail = new("Email", "%@example.com");
/// Columns&lt;Customer&gt; columnEmail = new(nameof(Customer.Email));
/// Like predicate = new(columnEmail, parameterEmail);
/// SqlQuery query = customerGenerator.Select(null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Email] LIKE @Parameter_Email)
/// ]]></code>
/// </example>
public class Like : ComparisonOperators
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's LIKE, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public Like(PredicatesBase left, PredicatesBase right) : base (left, right, "LIKE")
    {
    }
}
