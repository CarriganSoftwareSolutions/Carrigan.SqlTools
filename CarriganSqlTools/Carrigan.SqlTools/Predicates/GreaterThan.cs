namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's Greater Than, >, comparison operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterTotal = new("Total", 1776.00m);
/// Columns&lt;Order&gt; columnTotal = new(nameof(Order.Total));
/// GreaterThan predicate = new(columnTotal, parameterTotal);
/// SqlQuery query = orderGenerator.Select(null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)
/// ]]></code>
/// </example>
public class GreaterThan : ComparisonOperators
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's Greater Than, >, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public GreaterThan(PredicatesBase left, PredicatesBase right) : base (left, right, ">")
    {
    }
}
