using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using System.Xml.Linq;

namespace Carrigan.SqlTools.OrderByItems;

public class OrderByItem<T> : IOrderByItem, IOrderByClause
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

    public IEnumerable<TableTag> TableTags => 
        [TableTag];

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

    /// <summary>
    /// In this context Contains is basically an alias to determine if this single item equals the item being passed in.
    /// This is because an order by item is being modeled as a single item enumeration, so an OrderBy or OrderByItem can be used interchangeably in query generations.
    /// Therefore, I wanted this method to use the same logic the Contains method uses to determine equality in an IEnumerable, just applied to a single item.
    /// </summary>
    /// <param name="orderByItem"></param>
    /// <returns>true if equal, else false.</returns>
    public bool Contains(IOrderByItem orderByItem) =>
        EqualityComparer<IOrderByItem>.Default.Equals(this, orderByItem);

    /// <summary>
    /// The item is considered empty if the Column name is null, empty or whitespace.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool IsEmpty() =>
        ColumnName.IsNullOrWhiteSpace();

    public OrderBy WithAppend(IOrderByItem orderByItem) =>
        new (new List<IOrderByItem>([this, orderByItem]));

    public OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems) =>
        new (orderByItems.Prepend(this));

    public OrderBy AsOrderBy() =>
        new (this);
}
