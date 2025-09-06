using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

public class OrderByItem<T> : IOrderByItem
{
    public OrderByItem(string columnName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
    {
        if (SqlToolsReflectorCache<T>.ColumnNamesHashSet.DoesNotContain(columnName))
            throw SqlIdentifierException.FromInvalidColumnNames<T>(columnName);
        ColumnName = columnName;
        SortDirection = sortDirection;
    }

    public ColumnTag ColumnTag =>
        new(TableTag, ColumnName);

    public TableTag TableTag 
        => SqlToolsReflectorCache<T>.TableTag;

    public string ColumnName { get; private set; }

    public SortDirectionEnum SortDirection { get; private set; }

    public string ToSql() =>
        $"{ColumnTag} {SortDirection.ToSql()}";


    public bool Equals(IOrderByItem? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return TableTag.Equals(other.TableTag)
            && ColumnTag.Equals(other.ColumnTag);
    }

    public override bool Equals(object? obj)
        => Equals(obj as IOrderByItem);

    public override int GetHashCode()
        => HashCode.Combine(TableTag, ColumnTag);
}
