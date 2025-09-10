using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// This class represents an SQL Inner Join
/// </summary>
/// <typeparam name="T">A data model representing the main table, left table or base table. This is the table you are selecting from, updating or deleting.</typeparam>
/// <typeparam name="J">A data model representing the right table or joined table. This is the table being joined to the main table.</typeparam>
/// /// <example>
/// <para>Note: Columns&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
/// <code language="csharp"><![CDATA[
/// Columns&lt;Customer&gt; id = new(nameof(Customer.Id));
/// Columns&lt;Order&gt; customerId = new(nameof(Order.CustomerId));
/// Equal equals = new(id, customerId);
/// InnerJoin&lt;Customer, Order&gt; join = new(equals);
///
/// SqlQuery query = customerGenerator.Select(join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// INNER JOIN [Order]  ON 
/// ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class InnerJoin<T, J> : JoinBaseClass
{
    private readonly string _sql;
    /// <summary>
    /// Constructor for the inner join class.
    /// </summary>
    /// <param name="predicate">Represents the "on" part of the join clause.</param>
    /// <exception cref="SqlIdentifierException"></exception>
    public InnerJoin(Predicates.PredicatesBase predicate)
    {
        TableTag leftTableTag = SqlToolsReflectorCache<T>.TableTag;
        TableTag rightTableTag = SqlToolsReflectorCache<J>.TableTag;
        IEnumerable<ColumnTag> invalidTags = 
            predicate
                .Column
                .Where(column => column.TableTag != leftTableTag && column.TableTag != rightTableTag)
                .Select(column => column.ColumnTag);

        if (predicate.Column.Where(column => column.TableTag != leftTableTag && column.TableTag != rightTableTag).Any())
            throw new SqlIdentifierException(invalidTags);

        _sql = $"INNER JOIN {rightTableTag} ON {predicate.ToSql()}";
        _tableTags = [leftTableTag, rightTableTag];
    }

    /// <summary>
    /// Convert the object to a SQL string.
    /// </summary>
    /// <returns>An SQL string for the inner join.</returns>
    public override string ToSql() =>
        _sql;
}
