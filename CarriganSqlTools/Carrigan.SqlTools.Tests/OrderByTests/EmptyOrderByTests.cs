using Carrigan.SqlTools.OrderByItems;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class EmptyOrderByTests
{
    [Fact]
    public void EmptyOrderByTests_ToSql()
    {
        IOrderByClause orderBy = OrderBy.Empty;

        Assert.Equal(string.Empty, orderBy.ToSql());
    }
    [Fact]
    public void EmptyOrderByTests_TableTags()
    {
        IOrderByClause orderBy = OrderBy.Empty;

        Assert.Empty(orderBy.TableTags);
    }
    [Fact]
    public void EmptyOrderByTests_OrderByItems()
    {
        OrderBy orderBy = OrderBy.Empty;

        Assert.Empty(orderBy.OrderByItemsAsEnumerable());
    }

    [Fact]
    public void EmptyOrderByTests_IsEmpty()
    {
        OrderBy orderBy = OrderBy.Empty;

        Assert.True(orderBy.IsEmpty());
    }
}
