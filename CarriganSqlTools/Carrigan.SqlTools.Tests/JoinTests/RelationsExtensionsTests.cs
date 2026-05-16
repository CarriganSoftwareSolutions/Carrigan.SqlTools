using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

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
