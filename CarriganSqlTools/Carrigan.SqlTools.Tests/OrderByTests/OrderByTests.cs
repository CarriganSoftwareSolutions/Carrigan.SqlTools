using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Base.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class OrderByTests
{
    private readonly static ISqlDialects Dialect = new SqlServerDialect();
    [Fact]
    public void OrderByTests_Constructor()
    {
        OrderBy<Address> orderByItem = new("City", SortDirectionEnum.Ascending);
        OrderBys orderBy = new(orderByItem);


        Assert.Single(orderBy.TableTags);
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void OrderByTests_Constructor_With_Default_Ascending()
    {
        OrderBy<Address> orderByItem = new("City");
        OrderBys orderBy = new(orderByItem);


        Assert.Single(orderBy.TableTags);
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void OrderByTests_Constructor_MultipleOrderByItems()
    {
        OrderBy<Address> orderByItem1 = new("City", SortDirectionEnum.Ascending);
        OrderBy<Address> orderByItem2 = new("Street", SortDirectionEnum.Descending);
        OrderBys orderBy = new (orderByItem1, orderByItem2);


        Assert.Equal(2, orderBy.TableTags.Count());
        Assert.Equal("ORDER BY [Address].[City] ASC, [Address].[Street] DESC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void OrderByTests_Empty_Constructor()
    {
        OrderBys order = new();

        Assert.True(order.IsEmpty());
    }

    [Fact]
    public void OrderByTests_Constructor_EmptyItemList()
    {
        OrderBys order = new([]);

        Assert.True(order.IsEmpty());
    }

    [Fact]
    public void OrderByTests_Constructor_MultipleTables()
    {
        OrderBy<Address> orderByItem1 = new("City", SortDirectionEnum.Ascending);
        OrderBy<ColumnTable> orderByItem2 = new("D000destruct0", SortDirectionEnum.Descending);
        OrderBy<BooleanColumnTable> orderByItem3 = new("Id", SortDirectionEnum.Ascending);
        OrderBys orderBy = new (orderByItem1, orderByItem2, orderByItem3);


        Assert.Equal(3, orderBy.TableTags.Count());
        Assert.Equal("ORDER BY [Address].[City] ASC, [ColumnTable].[D000destruct0] DESC, [BooleanColumnTable].[Id] ASC", orderBy.ToSql(Dialect));
    }
    [Fact]
    public void Append()
    {
        OrderBys order = new ();                            // initially empty
        OrderBy<Address> item = new("Street");

        OrderBys returned = order.Append(item);

        // Add should be chain-able
        Assert.NotSame(order, returned);

        // empty?
        Assert.True(order.IsEmpty());
        Assert.False(returned.IsEmpty());

        // TableTags should reflect the added item
        Assert.Empty(order.TableTags);
        Assert.Single(returned.TableTags);

        // ToSql should produce ORDER BY clause
        Assert.Equal("ORDER BY [Address].[Street] ASC", returned.ToSql(Dialect));
    }

    [Fact]
    public void Concat()
    {
        OrderBy<Address> initial = new("City", SortDirectionEnum.Ascending);
        OrderBy<Address> more1 = new("Street", SortDirectionEnum.Descending);
        OrderBy<Address> more2 = new("City", SortDirectionEnum.Descending); // same column, different direction

        OrderBys order = new(initial);

        OrderBys newOrder = order.Concat(more1, more2);

        // Should have one items now
        List<OrderByBase> oldItems = [.. order.AsEnumerable()];
        Assert.Single(oldItems);

        // Should have three items now
        List<OrderByBase> newItems = [.. newOrder.AsEnumerable()];
        Assert.Equal(3, newItems.Count);

        // Insertion order must be preserved
        Assert.Equal(initial.ToSql(Dialect), oldItems[0].ToSql(Dialect));
        Assert.Equal(initial.ToSql(Dialect), newItems[0].ToSql(Dialect));
        Assert.Equal(more1.ToSql(Dialect), newItems[1].ToSql(Dialect));
        Assert.Equal(more2.ToSql(Dialect), newItems[2].ToSql(Dialect));

        // ToSql must join all three
        string expectedSql = string.Join(", ", oldItems.Select(i => i.ToSql(Dialect)));
        Assert.Equal($"ORDER BY {expectedSql}", order.ToSql(Dialect));

        // ToSql must join all three
        expectedSql = string.Join(", ", newItems.Select(i => i.ToSql(Dialect)));
        Assert.Equal($"ORDER BY {expectedSql}", newOrder.ToSql(Dialect));
    }

    [Fact]
    public void Contains_ReturnsTrue_ForExistingItemReference()
    {
        OrderBy<Address> item = new("City");
        OrderBys order = new(item);

        Assert.True(order.Contains(item));
    }

    [Fact]
    public void Contains_ReturnsTrue_ForDifferentInstanceWithSameTableAndColumn()
    {
        OrderBy<Address> original = new("City", SortDirectionEnum.Descending);
        OrderBys order = new (original);

        // Different instance but same TableTag + ColumnTag
        OrderBy<Address> lookup = new("City", SortDirectionEnum.Ascending);

        Assert.True(order.Contains(lookup));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        OrderBy<Address> streetItem = new("Street");
        OrderBys order = new (streetItem);

        // Different column
        OrderBy<Address> notPresent = new("City");
        Assert.False(order.Contains(notPresent));

        // Different table (using a different entity type)
        OrderBy<ColumnTable> otherTableItem = new("D000destruct0");
        Assert.False(order.Contains(otherTableItem));
    }
}
