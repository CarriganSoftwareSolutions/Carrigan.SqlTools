using SqlTools.Exceptions;
using SqlTools.Tags;

namespace SqlTools.Joins;

/// <typeparam name="T">A data model representing the main table, left table or base table. This is the table you are selecting from, updating or deleting.</typeparam>
/// <typeparam name="J">A data model representing the right table or joined table. This is the table being joined to the main table.</typeparam>
public class LeftJoin<T, J> : JoinBaseClass
{
    private readonly string _sql;
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

    public override string ToSql() =>
        _sql;
}
