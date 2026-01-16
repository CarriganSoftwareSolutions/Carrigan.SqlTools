using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class EmptyOrderByTests
{
    [Fact]
    public void EmptyOrderByTests_ToSql()
    {
        OrderBy orderBy = OrderBy.Empty;

        Assert.Equal(string.Empty, orderBy.ToSql());
    }
    [Fact]
    public void EmptyOrderByTests_TableTags()
    {
        OrderBy orderBy = OrderBy.Empty;

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

    [Fact]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        OrderByItemBase item = new OrderByItem<Address>("Street");

        Assert.False(item.Equals((object)"Street"));
    }
}
