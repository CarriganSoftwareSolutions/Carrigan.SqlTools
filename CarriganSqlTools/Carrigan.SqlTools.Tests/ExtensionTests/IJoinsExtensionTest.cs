using Carrigan.SqlTools.Extensions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
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
        PredicatesBase id = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        IJoins join = new LeftJoin<JoinLeftTable, JoinRightTable>(id);
        _ = new Joins([join]);
    }
}
