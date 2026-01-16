using Carrigan.SqlTools.OrderByItems;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class OrderByBaseTests
{
    [Fact]
    public void AsOrderBy_ReturnsSameInstanceForOrderBy()
    {
        OrderBy orderBy = OrderBy.Empty;

        OrderBy actual = orderBy.AsOrderBy();

        Assert.Same(orderBy, actual);
    }

    [Fact]
    public void WithAppend_DoesNotMutateOriginal()
    {
        OrderBy orderBy = OrderBy.Empty;
        OrderByItemBase item = new OrderByItem<TestEntity>("Name");

        OrderBy appended = orderBy.WithAppend(item);

        Assert.True(orderBy.IsEmpty());
        Assert.False(appended.IsEmpty());
    }

    [Fact]
    public void WithConcat_AppendsItemsInOrder()
    {
        OrderBy orderBy = OrderBy.Empty;

        OrderByItemBase[] items =
        [
            new OrderByItem<TestEntity>("Name"),
            new OrderByItem<TestEntity>("Id")
        ];

        OrderBy appended = orderBy.WithConcat(items);

        string sql = appended.ToSql();
        Assert.Contains("[TestEntity].[Name]", sql);
        Assert.Contains("[TestEntity].[Id]", sql);
    }

    private sealed class TestEntity
    {
        public string Name { get; } = string.Empty;
        public int Id { get; }
    }
}
