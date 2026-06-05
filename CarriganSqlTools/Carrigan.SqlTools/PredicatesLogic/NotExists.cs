using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.SqlGenerators;

//IGNORE SPELLING: subquery, subqueries, intellisense, exists, parameterization

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a SQL NOT EXISTS predicate that evaluates a subquery and is true when the subquery returns no rows.
/// </summary>
/// <remarks>Constructed with a <see cref="SubqueryPredicateBase"/> that supplies the subquery. Produces the SQL fragment <c>NOT EXISTS (&lt;subquery&gt;)</c> and relies on <see cref="SubQueryPredicateBase"/> for rendering and parameterization.</remarks>
///
/// <example>
/// <code language="csharp"><![CDATA[
/// Predicates orderTotalGreaterThan = new GreaterThan
/// (
///     new Column<Order>(nameof(Order.Total)),
///     new Parameter(100.00m, "Total")
/// );
/// Subquery<Order> subQuery = orderGenerator.Subquery(null, null, null, orderTotalGreaterThan, null, null);
/// NotExists notExists = new(subQuery);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer"
/// WHERE (NOT EXISTS (SELECT "Order".* FROM "Order" WHERE ("Order"."Total" > $1)))
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE (NOT EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Total_1)))
/// ]]></code>
/// </example>
public class NotExists : SubqueryPredicateBase
{
    /// <summary>
    /// Initializes a new NotExists instance representing a SQL NOT EXISTS expression for the specified subquery.
    /// </summary>
    /// <remarks>Passes the subquery and the NOT EXISTS operator to the base class.</remarks>
    /// <param name="subQuery">Subquery to evaluate with the NOT EXISTS operator.</param>
    public NotExists(SubqueryBase subQuery) : base(subQuery, "NOT EXISTS")
    {
    }
}

