using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.GroupByClause;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GroupByTests;

public class GroupByTests
{
    private readonly static ISqlDialects Dialect = new SqlServerDialect();
    [Fact]
    public void GroupByTests_Constructor()
    {
        GroupBy<Address> groupByItem = new("City");
        GroupBys groupBy = new(groupByItem);


        Assert.Single(groupBy.TableTags);
        Assert.Equal("GROUP BY [Address].[City]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void GroupByTests_Constructor_MultipleGroupByItems()
    {
        GroupBy<Address> groupByItem1 = new("City");
        GroupBy<Address> groupByItem2 = new("Street");
        GroupBys groupBy = new (groupByItem1, groupByItem2);


        Assert.Equal(2, groupBy.TableTags.Count());
        Assert.Equal("GROUP BY [Address].[City], [Address].[Street]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void GroupByTests_Empty_Constructor()
    {
        GroupBys group = new();

        Assert.True(group.IsEmpty());
    }

    [Fact]
    public void GroupByTests_Constructor_EmptyItemList()
    {
        GroupBys group = new([]);

        Assert.True(group.IsEmpty());
    }

    [Fact]
    public void GroupByTests_Constructor_MultipleTables()
    {
        GroupBy<Address> groupByItem1 = new("City");
        GroupBy<ColumnTable> groupByItem2 = new("D000destruct0");
        GroupBy<BooleanColumnTable> groupByItem3 = new("Id");
        GroupBys groupBy = new (groupByItem1, groupByItem2, groupByItem3);


        Assert.Equal(3, groupBy.TableTags.Count());
        Assert.Equal("GROUP BY [Address].[City], [ColumnTable].[D000destruct0], [BooleanColumnTable].[Id]", groupBy.ToSql(Dialect));
    }
    [Fact]
    public void Append()
    {
        GroupBys group = new ();                            // initially empty
        GroupBy<Address> item = new("Street");

        GroupBys returned = group.Append(item);

        // Add should be chain-able
        Assert.NotSame(group, returned);

        // empty?
        Assert.True(group.IsEmpty());
        Assert.False(returned.IsEmpty());

        // TableTags should reflect the added item
        Assert.Empty(group.TableTags);
        Assert.Single(returned.TableTags);

        // ToSql should produce GROUP BY clause
        Assert.Equal("GROUP BY [Address].[Street]", returned.ToSql(Dialect));
    }

    [Fact]
    public void Concat()
    {
        GroupBy<Address> initial = new("City");
        GroupBy<Address> more1 = new("Street");
        GroupBy<Address> more2 = new("City"); // same column, different direction

        GroupBys group = new(initial);

        GroupBys newGroup = group.Concat(more1, more2);

        // Should have one items now
        List<GroupByBase> oldItems = [.. group.AsEnumerable()];
        Assert.Single(oldItems);

        // Should have three items now
        List<GroupByBase> newItems = [.. newGroup.AsEnumerable()];
        Assert.Equal(3, newItems.Count);

        // Insertion group must be preserved
        Assert.Equal(initial.ToSql(Dialect), oldItems[0].ToSql(Dialect));
        Assert.Equal(initial.ToSql(Dialect), newItems[0].ToSql(Dialect));
        Assert.Equal(more1.ToSql(Dialect), newItems[1].ToSql(Dialect));
        Assert.Equal(more2.ToSql(Dialect), newItems[2].ToSql(Dialect));

        // ToSql must join all three
        string expectedSql = string.Join(", ", oldItems.Select(i => i.ToSql(Dialect)));
        Assert.Equal($"GROUP BY {expectedSql}", group.ToSql(Dialect));

        // ToSql must join all three
        expectedSql = string.Join(", ", newItems.Select(i => i.ToSql(Dialect)));
        Assert.Equal($"GROUP BY {expectedSql}", newGroup.ToSql(Dialect));
    }

    [Fact]
    public void Contains_ReturnsTrue_ForExistingItemReference()
    {
        GroupBy<Address> item = new("City");
        GroupBys group = new(item);

        Assert.True(group.Contains(item));
    }

    [Fact]
    public void Contains_ReturnsTrue_ForDifferentInstanceWithSameTableAndColumn()
    {
        GroupBy<Address> original = new("City");
        GroupBys group = new (original);

        // Different instance but same TableTag + ColumnTag
        GroupBy<Address> lookup = new("City");

        Assert.True(group.Contains(lookup));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        GroupBy<Address> streetItem = new("Street");
        GroupBys group = new (streetItem);

        // Different column
        GroupBy<Address> notPresent = new("City");
        Assert.False(group.Contains(notPresent));

        // Different table (using a different entity type)
        GroupBy<ColumnTable> otherTableItem = new("D000destruct0");
        Assert.False(group.Contains(otherTableItem));
    }
}
