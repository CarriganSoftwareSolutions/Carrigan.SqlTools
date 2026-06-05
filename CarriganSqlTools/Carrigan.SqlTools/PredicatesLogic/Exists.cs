using Carrigan.Core.Interfaces.IModels;
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
///     new Parameter(100.00m, "Total")
/// );
/// Subquery<Order> subQuery = orderGenerator.Subquery(null, null, null, orderTotalGreaterThan, null, null);
/// Exists exists = new(subQuery);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = exists
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer" 
/// WHERE (EXISTS (SELECT "Order".* 
/// FROM "Order"
/// WHERE ("Order"."Total" > $1)))
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE (EXISTS (SELECT [Order].* 
/// FROM [Order] 
/// WHERE ([Order].[Total] > @Total_1)))
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
