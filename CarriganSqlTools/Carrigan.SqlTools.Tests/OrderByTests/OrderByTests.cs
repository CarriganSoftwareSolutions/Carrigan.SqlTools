using SqlTools.OrderByItems;
using SqlTools.Tags;
using SqlToolsTests.TestEntities;

namespace SqlToolsTests.OrderByTests;

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
        OrderBy orderBy = new(orderByItem1, orderByItem2);


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
        OrderBy orderBy = new(orderByItem1, orderByItem2, orderByItem3);


        Assert.Equal(3, orderBy.TableTags.Count());
        Assert.Equal("[Address]", orderBy.TableTags.First());
        Assert.Equal("[ColumnTable]", orderBy.TableTags.Skip(1).First());
        Assert.Equal("[BooleanColumnTable]", orderBy.TableTags.Skip(2).First());
        Assert.Equal("ORDER BY [Address].[City] ASC, [ColumnTable].[D000descruct0] DESC, [BooleanColumnTable].[Id] ASC", orderBy.ToSql());
    }
    [Fact]
    public void Add_SingleItem_ToEmptyOrderBy_BehavesCorrectly()
    {
        OrderBy order = new();                            // initially empty
        OrderByItem<Address> item = new("Street");

        OrderBy returned = order.Add(item);

        // Add should be chain-able (same instance)
        Assert.Same(order, returned);

        // Now it should no longer be empty
        Assert.False(order.IsEmpty());

        // TableTags should reflect the added item
        Assert.Single(order.TableTags);
        Assert.Equal("[Address]", order.TableTags.Single());

        // ToSql should produce ORDER BY clause
        Assert.Equal("ORDER BY [Address].[Street] ASC", order.ToSql());
    }

    [Fact]
    public void Add_MultipleItems_ToExistingOrderBy_AppendsThemInOrder()
    {
        OrderByItem<Address> initial = new("City", SortDirectionEnum.Ascending);
        OrderByItem<Address> more1 = new("Street", SortDirectionEnum.Descending);
        OrderByItem<Address> more2 = new("City", SortDirectionEnum.Descending); // same column, different direction

        OrderBy order = new(initial);

        order.Add(more1, more2);

        // Should have three items now
        List<IOrderByItem> items =
            [.. order.OrderByItemsAsEnumerable()];
        Assert.Equal(3, items.Count);

        // Insertion order must be preserved
        Assert.Equal(initial.ToSql(), items[0].ToSql());
        Assert.Equal(more1.ToSql(), items[1].ToSql());
        Assert.Equal(more2.ToSql(), items[2].ToSql());

        // TableTags sequence likewise
        List<TableTag> tags =
            [.. order.TableTags];
        Assert.Equal("[Address]", tags[0]);
        Assert.Equal("[Address]", tags[1]);
        Assert.Equal("[Address]", tags[2]);

        // ToSql must join all three
        string expectedSql = string.Join(", ", items.Select(i => i.ToSql()));
        Assert.Equal($"ORDER BY {expectedSql}", order.ToSql());
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
        OrderBy order = new(original);

        // Different instance but same TableTag + ColumnTag
        OrderByItem<Address> lookup = new("City", SortDirectionEnum.Ascending);

        Assert.True(order.Contains(lookup));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        OrderByItem<Address> streetItem = new("Street");
        OrderBy order = new(streetItem);

        // Different column
        OrderByItem<Address> notPresent = new("City");
        Assert.False(order.Contains(notPresent));

        // Different table (using a different entity type)
        OrderByItem<ColumnTable> otherTableItem = new("D000descruct0");
        Assert.False(order.Contains(otherTableItem));
    }
}
