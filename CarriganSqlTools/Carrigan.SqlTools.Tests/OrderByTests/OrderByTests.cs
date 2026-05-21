using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class OrderByTests
{
    private readonly static ISqlDialects Dialect = new SqlServerDialect();
    [Fact]
    public void OrderByTests_Constructor()
    {
        OrderByItem<Address> orderByItem = new("City", SortDirectionEnum.Ascending);
        OrderBy orderBy = new(orderByItem);


        Assert.Single(orderBy.TableTags);
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void OrderByTests_Constructor_With_Default_Ascending()
    {
        OrderByItem<Address> orderByItem = new("City");
        OrderBy orderBy = new(orderByItem);


        Assert.Single(orderBy.TableTags);
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void OrderByTests_Constructor_MultipleOrderByItems()
    {
        OrderByItem<Address> orderByItem1 = new("City", SortDirectionEnum.Ascending);
        OrderByItem<Address> orderByItem2 = new("Street", SortDirectionEnum.Descending);
        OrderBy orderBy = new (orderByItem1, orderByItem2);


        Assert.Equal(2, orderBy.TableTags.Count());
        Assert.Equal("ORDER BY [Address].[City] ASC, [Address].[Street] DESC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void OrderByTests_Empty_Constructor()
    {
        OrderBy order = new();

        Assert.True(order.IsEmpty());
    }

    [Fact]
    public void OrderByTests_Constructor_EmptyItemList()
    {
        OrderBy order = new([]);

        Assert.True(order.IsEmpty());
    }

    [Fact]
    public void OrderByTests_Constructor_MultipleTables()
    {
        OrderByItem<Address> orderByItem1 = new("City", SortDirectionEnum.Ascending);
        OrderByItem<ColumnTable> orderByItem2 = new("D000destruct0", SortDirectionEnum.Descending);
        OrderByItem<BooleanColumnTable> orderByItem3 = new("Id", SortDirectionEnum.Ascending);
        OrderBy orderBy = new (orderByItem1, orderByItem2, orderByItem3);


        Assert.Equal(3, orderBy.TableTags.Count());
        Assert.Equal("ORDER BY [Address].[City] ASC, [ColumnTable].[D000destruct0] DESC, [BooleanColumnTable].[Id] ASC", orderBy.ToSql(Dialect));
    }
    [Fact]
    public void WithAppend()
    {
        OrderBy order = new ();                            // initially empty
        OrderByItem<Address> item = new("Street");

        OrderBy returned = order.WithAppend(item);

        // Add should be chain-able (same instance)
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
    public void WithConcat()
    {
        OrderByItem<Address> initial = new("City", SortDirectionEnum.Ascending);
        OrderByItem<Address> more1 = new("Street", SortDirectionEnum.Descending);
        OrderByItem<Address> more2 = new("City", SortDirectionEnum.Descending); // same column, different direction

        OrderBy order = new(initial);

        OrderBy newOrder = order.WithConcat(more1, more2);

        // Should have one items now
        List<OrderByItemBase> oldItems =
            [.. order.OrderByItemsAsEnumerable()];
        Assert.Single(oldItems);

        // Should have three items now
        List<OrderByItemBase> newItems =
            [.. newOrder.OrderByItemsAsEnumerable()];
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
        OrderByItem<Address> item = new("City");
        OrderBy order = new(item);

        Assert.True(order.Contains(item));
    }

    [Fact]
    public void Contains_ReturnsTrue_ForDifferentInstanceWithSameTableAndColumn()
    {
        OrderByItem<Address> original = new("City", SortDirectionEnum.Descending);
        OrderBy order = new (original);

        // Different instance but same TableTag + ColumnTag
        OrderByItem<Address> lookup = new("City", SortDirectionEnum.Ascending);

        Assert.True(order.Contains(lookup));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        OrderByItem<Address> streetItem = new("Street");
        OrderBy order = new (streetItem);

        // Different column
        OrderByItem<Address> notPresent = new("City");
        Assert.False(order.Contains(notPresent));

        // Different table (using a different entity type)
        OrderByItem<ColumnTable> otherTableItem = new("D000destruct0");
        Assert.False(order.Contains(otherTableItem));
    }
}
