using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.GroupByClause;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GroupByTests;

public class EmptyGroupByTests
{
    private readonly static ISqlDialects Dialect = new SqlServerDialect();

    [Fact]
    public void EmptyGroupByTests_ToSql()
    {
        GroupBys groupBy = GroupBys.Empty;

        Assert.Equal(string.Empty, groupBy.ToSql(Dialect));
    }
    [Fact]
    public void EmptyGroupByTests_TableTags()
    {
        GroupBys groupBy = GroupBys.Empty;

        Assert.Empty(groupBy.TableTags);
    }
    [Fact]
    public void EmptyGroupByTests_GroupByItems()
    {
        GroupBys groupBy = GroupBys.Empty;

        Assert.Empty(groupBy.AsEnumerable());
    }

    [Fact]
    public void EmptyGroupByTests_IsEmpty()
    {
        GroupBys groupBy = GroupBys.Empty;

        Assert.True(groupBy.IsEmpty());
    }

    [Fact]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        GroupBy<Address> item = new("Street");

        Assert.False(item.Equals((object)"Street"));
    }
}
