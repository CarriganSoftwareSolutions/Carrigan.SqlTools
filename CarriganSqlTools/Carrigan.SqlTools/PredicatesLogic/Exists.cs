using Carrigan.SqlTools.SqlGenerators;

//IGNORE SPELLING: subquery, subqueries, intellisense, exists

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents an EXISTS predicate in a SQL query, which evaluates to true if the specified subquery returns any rows.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="Exists"/> predicate is used to determine if any records exist that satisfy the conditions defined in the subquery.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// Predicates orderTotalGreaterThan = new GreaterThan
/// (
///     new Column<Order>(nameof(Order.Total)),
///     new Parameter("Total", 100.00m)
/// );
/// Subquery<Order> subQuery = orderGenerator.Subquery(null, null, null, orderTotalGreaterThan, null, null);
/// Exists exists = new(subQuery);
/// 
/// SqlQuery query = customerGenerator.Select(null, null, null, null, exists, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE (EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Total_1)))
/// ]]></code>
/// </example>
public class Exists : SubqueryPredicateBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Exists"/> class with the specified subquery.
    /// </summary>
    /// <param name="subQuery">
    /// The subquery to evaluate for the EXISTS predicate. 
    /// This subquery should return rows that satisfy the conditions being checked for existence.
    /// </param>
    public Exists(SubqueryBase subQuery) : base(subQuery, "EXISTS")
    { }
}
