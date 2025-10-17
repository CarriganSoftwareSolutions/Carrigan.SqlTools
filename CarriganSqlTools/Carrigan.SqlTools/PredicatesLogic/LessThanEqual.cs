using Carrigan.SqlTools.SqlGenerators;
using System;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's Less Than Or Equal To, <=, comparison operator.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterTotal = new("Total", 1776.00m);
/// Column<Order> columnTotal = new(nameof(Order.Total));
/// LessThanEqual predicate = new(columnTotal, parameterTotal);
/// SqlQuery query = orderGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Order].* 
/// FROM [Order] 
/// WHERE ([Order].[Total] <= @Parameter_Total)
/// ]]></code>
/// </example>
public class LessThanEqual : ComparisonOperator
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's Less Than Or Equal To, <=, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public LessThanEqual(Predicates left, Predicates right) : base (left, right, "<=")
    {
    }
}
