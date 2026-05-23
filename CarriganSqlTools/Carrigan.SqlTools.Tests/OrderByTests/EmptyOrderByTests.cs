using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class EmptyOrderByTests
{
    private readonly static ISqlDialects Dialect = new SqlServerDialect();

    [Fact]
    public void EmptyOrderByTests_ToSql()
    {
        OrderBys orderBy = OrderBys.Empty;

        Assert.Equal(string.Empty, orderBy.ToSql(Dialect));
    }
    [Fact]
    public void EmptyOrderByTests_TableTags()
    {
        OrderBys orderBy = OrderBys.Empty;

        Assert.Empty(orderBy.TableTags);
    }
    [Fact]
    public void EmptyOrderByTests_OrderByItems()
    {
        OrderBys orderBy = OrderBys.Empty;

        Assert.Empty(orderBy.AsEnumerable());
    }

    [Fact]
    public void EmptyOrderByTests_IsEmpty()
    {
        OrderBys orderBy = OrderBys.Empty;

        Assert.True(orderBy.IsEmpty());
    }

    [Fact]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        OrderByBase item = new OrderBy<Address>("Street");

        Assert.False(item.Equals((object)"Street"));
    }
}
