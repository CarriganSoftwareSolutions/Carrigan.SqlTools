using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.ExtensionTests;

public class IJoinsExtensionTest
{
    [Fact]
    public void IsNullOrEmpty_Null()
    {
        Joins<Order>? joins = null;
        Assert.True(joins.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_Empty()
    {
        Joins<Order> joins = new ([]);
        Assert.True(joins.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_Single()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> join = new (id);
        Joins<JoinLeftTable> joins_ = new (join);
        Assert.False(joins_.IsNullOrEmpty());
    }
}
