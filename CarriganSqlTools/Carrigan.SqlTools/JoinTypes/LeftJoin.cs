using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;
/// <summary>
/// Represents an SQL Left Join.
/// </summary>
/// <typeparam name="T">A data model representing the main table, left table or base table. This is the table you are selecting from, updating or deleting.</typeparam>
/// <typeparam name="J">A data model representing the right table or joined table. This is the table being joined to the main table.</typeparam>
/// /// /// <example>
/// //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
///         
/// Columns<Customer> id = new(nameof(Customer.Id));
/// Columns<Order> customerId = new(nameof(Order.CustomerId));
/// Equal equals = new(id, customerId);
/// LeftJoin<Customer, Order> join = new(equals);
///
/// SqlQuery query = customerGenerator.Select(join, null, null, null);
/// 
/// // SELECT [Customer].* FROM [Customer] 
/// // LEFT JOIN [Order]  ON 
/// // ([Customer].[Id] = [Order].[CustomerId])
/// </example>
public class LeftJoin<T, J> : JoinBaseClass
{
    private readonly string _sql;

    /// <summary>
    /// Constructor for the left join class.
    /// </summary>
    /// <param name="predicate">Represents the "on" part of the left join clause.</param>
    public LeftJoin(Predicates.PredicatesBase predicate)
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

        _sql = $"LEFT JOIN {rightTableTag} ON {predicate.ToSql()}";
        _tableTags = [leftTableTag, rightTableTag];
    }

    /// <summary>
    /// This generates the SQL for the left Join as a string.
    /// </summary>
    /// <returns>A string for the left Join's SQL</returns>
    public override string ToSql() =>
        _sql;
}
