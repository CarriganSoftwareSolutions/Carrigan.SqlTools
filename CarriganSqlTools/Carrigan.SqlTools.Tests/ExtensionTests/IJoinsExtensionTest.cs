using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.ExtensionTests;

public class IJoinsExtensionTest
{
    [Fact]
    public void IsNullOrEmpty_Null()
    {
        IJoins? joins = null;
        Assert.True(joins.IsNullOrEmpty());
    }
    [Fact]
    public void IsNullOrEmpty_Empty()
    {
        IJoins joins = new Joins([]);
        Assert.True(joins.IsNullOrEmpty());
    }
    [Fact]
    public void IsNullOrEmpty_Single()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        IJoins join = new LeftJoin<JoinLeftTable, JoinRightTable>(id);
        _ = new Joins([join]);
    }
}
