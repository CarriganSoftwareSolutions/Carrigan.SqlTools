using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's Equality, =, comparison operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterName = new("Name", "Hank");
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, equalName, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class Equal : ComparisonOperators
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's Equality, =, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public Equal(PredicatesBase left, PredicatesBase right) : base (left, right, "=")
    {
    }
}
