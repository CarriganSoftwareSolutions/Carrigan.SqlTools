using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using System;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's Not Equal, <>, comparison operator.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Name", "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// NotEqual predicate = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] <> @Parameter_Name)
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
