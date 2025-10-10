namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's Not Equal, <>, comparison operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterName = new("Name", "Hank");
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
///NotEqual predicate = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] <> @Parameter_Name)
/// ]]></code>
/// </example>
public class NotEqual : ComparisonOperator
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's Not Equal, <>, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public NotEqual(Predicates left, Predicates right) : base (left, right, "<>")
    {
    }
}
