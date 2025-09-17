using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class OrderByTests
{
    [Fact]
    public void OrderByTests_Constructor()
    {
        OrderByItem<Address> orderByItem = new("City", SortDirectionEnum.Ascending);
        OrderBy orderBy = new(orderByItem);


        Assert.Single(orderBy.TableTags);
        Assert.Equal("[Address]", orderBy.TableTags.Single());
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql());
    }

    [Fact]
    public void OrderByTests_Constructor_With_Default_Ascending()
    {
        OrderByItem<Address> orderByItem = new("City");
        OrderBy orderBy = new(orderByItem);


        Assert.Single(orderBy.TableTags);
        Assert.Equal("[Address]", orderBy.TableTags.Single());
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql());
    }

    [Fact]
    public void OrderByTests_Constructor_MultipleOrderByItems()
    {
        OrderByItem<Address> orderByItem1 = new("City", SortDirectionEnum.Ascending);
        OrderByItem<Address> orderByItem2 = new("Street", SortDirectionEnum.Descending);
        IOrderByClause orderBy = new OrderBy(orderByItem1, orderByItem2);


        Assert.Equal(2, orderBy.TableTags.Count());
        Assert.Equal("[Address]", orderBy.TableTags.First());
        Assert.Equal("[Address]", orderBy.TableTags.Skip(1).First());
        Assert.Equal("ORDER BY [Address].[City] ASC, [Address].[Street] DESC", orderBy.ToSql());
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
        OrderByItem<ColumnTable> orderByItem2 = new("D000descruct0", SortDirectionEnum.Descending);
        OrderByItem<BooleanColumnTable> orderByItem3 = new("Id", SortDirectionEnum.Ascending);
        IOrderByClause orderBy = new OrderBy(orderByItem1, orderByItem2, orderByItem3);


        Assert.Equal(3, orderBy.TableTags.Count());
        Assert.Equal("[Address]", orderBy.TableTags.First());
        Assert.Equal("[ColumnTable]", orderBy.TableTags.Skip(1).First());
        Assert.Equal("[BooleanColumnTable]", orderBy.TableTags.Skip(2).First());
        Assert.Equal("ORDER BY [Address].[City] ASC, [ColumnTable].[D000descruct0] DESC, [BooleanColumnTable].[Id] ASC", orderBy.ToSql());
    }
    [Fact]
    public void WithAppend()
    {
        IOrderByClause order = new OrderBy();                            // initially empty
        OrderByItem<Address> item = new("Street");

        IOrderByClause returned = order.WithAppend(item);

        // Add should be chain-able (same instance)
        Assert.NotSame(order, returned);

        // empty?
        Assert.True(order.IsEmpty());
        Assert.False(returned.IsEmpty());

        // TableTags should reflect the added item
        Assert.Empty(order.TableTags);
        Assert.Single(returned.TableTags);
        Assert.Equal("[Address]", returned.TableTags.Single());

        // ToSql should produce ORDER BY clause
        Assert.Equal("ORDER BY [Address].[Street] ASC", returned.ToSql());
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
        List<IOrderByItem> oldItems =
            [.. order.OrderByItemsAsEnumerable()];
        Assert.Single(oldItems);

        // Should have three items now
        List<IOrderByItem> newItems =
            [.. newOrder.OrderByItemsAsEnumerable()];
        Assert.Equal(3, newItems.Count);

        // Insertion order must be preserved
        Assert.Equal(initial.ToSql(), oldItems[0].ToSql());
        Assert.Equal(initial.ToSql(), newItems[0].ToSql());
        Assert.Equal(more1.ToSql(), newItems[1].ToSql());
        Assert.Equal(more2.ToSql(), newItems[2].ToSql());

        // TableTags sequence likewise
        List<TableTag> oldTags =
            [.. order.TableTags];
        Assert.Equal("[Address]", oldTags[0]);

        // TableTags sequence likewise
        List<TableTag> newTags =
            [.. newOrder.TableTags];
        Assert.Equal("[Address]", newTags[0]);
        Assert.Equal("[Address]", newTags[1]);
        Assert.Equal("[Address]", newTags[2]);

        // ToSql must join all three
        string expectedSql = string.Join(", ", oldItems.Select(i => i.ToSql()));
        Assert.Equal($"ORDER BY {expectedSql}", order.ToSql());

        // ToSql must join all three
        expectedSql = string.Join(", ", newItems.Select(i => i.ToSql()));
        Assert.Equal($"ORDER BY {expectedSql}", newOrder.ToSql());
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
        IOrderByClause order = new OrderBy(original);

        // Different instance but same TableTag + ColumnTag
        OrderByItem<Address> lookup = new("City", SortDirectionEnum.Ascending);

        Assert.True(order.Contains(lookup));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        OrderByItem<Address> streetItem = new("Street");
        IOrderByClause order = new OrderBy(streetItem);

        // Different column
        OrderByItem<Address> notPresent = new("City");
        Assert.False(order.Contains(notPresent));

        // Different table (using a different entity type)
        OrderByItem<ColumnTable> otherTableItem = new("D000descruct0");
        Assert.False(order.Contains(otherTableItem));
    }
}
