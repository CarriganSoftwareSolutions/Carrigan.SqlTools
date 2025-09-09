using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Extensions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    public SqlQuery SelectAll() =>
        Select(null, null, null, null);
    public SqlQuery SelectAll(IOrderByClause orderBy) =>
        Select(null, null, orderBy, null);

    /// <summary>
    /// Builds an SqlQuery object, which contains a parameterized Sql SELECT * with a Dictionary representing the parameter value pairs.
    /// </summary>
    /// <param name="joins">Defines the joins. Leave as null to leave out joins.</param>
    /// <param name="predicates">Defines the WHERE clause. Leave as null to leave out the WHERE clause.</param>
    /// <param name="OrderBy">Defines the ORDER BY clause. Leave as null to leave out the ORDER BY clause.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <param name="orderBy"></param>
    public SqlQuery Select(IJoins? joins, PredicatesBase? predicates, IOrderByClause? orderBy, OffsetNext? offsetNext)
    {
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(TableTag).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. (predicates?.Column?.Select(col => col.TableTag)?.Distinct() ?? [])];
        IEnumerable<TableTag> orderByTableTags = [.. (orderBy?.TableTags?.Distinct() ?? [])];
        IEnumerable<TableTag> invalidPredicateTags = predicateTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> invalidOrderByTags = orderByTableTags.Except(selectableTableTags);
        StringBuilder queryBuilder = new($"SELECT {TableTag}.* FROM {TableTag}");

        if (invalidPredicateTags.Any())
        {
            throw new SqlIdentifierException(invalidPredicateTags);
        }
        if (invalidOrderByTags.Any())
        {
            throw new SqlIdentifierException(invalidOrderByTags);
        }

        if (offsetNext is not null)
        {
            //add the key to orderby when using an offset next, this is to overcome a limitation in SQL Server that has unexpected behavior if the order by values are not unique
            orderBy ??= new OrderBy();
            IEnumerable<OrderByItem<T>> oderByKeyItems = [.. _Key.Select(key => new OrderByItem<T>(key.Name, SortDirectionEnum.Ascending)).Where(item => orderBy.Contains(item) == false)];
            orderBy = orderBy.WithConcat(oderByKeyItems);
        }

        if (orderBy.IsNullOrEmpty() && offsetNext is not null)
        {
            throw new ArgumentException($"Including an {nameof(offsetNext)} is not supported when {orderBy} is null or empty.");
        }

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {string.Join(' ', joins.ToSql())}");
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
        }
        if(orderBy.IsNotNullOrEmpty())
        {
            queryBuilder.Append($" {orderBy.AsOrderBy().ToSql()}");
        }
        if(offsetNext is not null)
        {
            queryBuilder.Append($" {offsetNext.ToSql()}");
        }
        return new SqlQuery()
        {
            QueryText = queryBuilder.ToString(),
            Parameters = predicates?.GetParameters() ?? [],
            CommandType = CommandType.Text
        };
        
    }

    public SqlQuery SelectById(params T[] entities) =>
        Select(null, new Or(entities.Select(entity => new And(SqlGenerator<T>.GetByKeyPredicates(entity)))), null, null);
}