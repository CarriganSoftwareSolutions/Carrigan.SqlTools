using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class RelationsExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_Null()
    {
        JoinsBase? relation = null;

        Assert.True(relation.IsNullOrEmpty());
        Assert.False(relation.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_Empty()
    {
        TestJoins relation = new([]);

        Assert.True(relation.IsNullOrEmpty());
        Assert.False(relation.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_NotEmpty()
    {
        TestJoins relation = new([new DummyJoin()]);

        Assert.False(relation.IsNullOrEmpty());
        Assert.True(relation.IsNotNullOrEmpty());
    }

    private sealed class TestJoins : JoinsBase
    {
        protected override IEnumerable<JoinBase> Joints { get; set; }

        internal override TableTag TableTag =>
            new(null, "Left");

        internal TestJoins(IEnumerable<JoinBase> joints) =>
            Joints = joints;
    }

    private sealed class DummyJoin : JoinBase
    {
        internal override TableTag TableTag =>
            new(null, "Right");

        internal DummyJoin() : base(new EmptyPredicate()) { }

        internal override string ToSql(string branchPrefix) =>
            "JOIN [Right] ON (1 = 1)";
    }
}
