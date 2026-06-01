using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.JoinTests;

public class RelationsExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_Null()
    {
        Joins<Customer>? relation = null;

        Assert.True(relation.IsNullOrEmpty());
        Assert.False(relation.IsNotNullOrEmpty());
    }
}
