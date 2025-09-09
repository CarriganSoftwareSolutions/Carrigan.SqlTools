using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;
public interface IOrderByClause
{
    IEnumerable<TableTag> TableTags { get; }

    bool Contains(IOrderByItem orderByItem);
    bool IsEmpty();
    string ToSql();
    OrderBy WithAppend(IOrderByItem orderByItem);
    OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems);
    OrderBy AsOrderBy();
}