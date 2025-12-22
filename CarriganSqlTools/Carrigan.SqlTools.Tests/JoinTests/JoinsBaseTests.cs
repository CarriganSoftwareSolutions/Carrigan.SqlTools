using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class JoinsBaseValidationTests
{
    [Fact]
    public void IsEmpty_WhenJointsIsNull_ThrowsInvalidOperationException()
    {
        JoinsBase joins = new NullJointsJoins();

        Assert.Throws<InvalidOperationException>(() => joins.IsEmpty());
    }

    [Fact]
    public void ToSql_WhenJointsContainsNull_ThrowsInvalidOperationException()
    {
        JoinsBase joins = new NullEntryJointsJoins();

        Assert.Throws<InvalidOperationException>(() => joins.ToSql());
    }

    [Fact]
    public void TableTags_WhenJointsContainsNull_ThrowsInvalidOperationException()
    {
        JoinsBase joins = new NullEntryJointsJoins();

        _ = Assert.Throws<InvalidOperationException>(() => joins.TableTags.ToList());
    }

    private sealed class NullJointsJoins : JoinsBase
    {
        protected override IEnumerable<JoinBase> Joints
        {
            get => null!;
            set => throw new NotSupportedException();
        }

        internal override TableTag TableTag =>
            new(null, "FakeTable");
    }

    private sealed class NullEntryJointsJoins : JoinsBase
    {
        protected override IEnumerable<JoinBase> Joints
        {
            get => [null!];
            set => throw new NotSupportedException();
        }

        internal override TableTag TableTag =>
            new(null, "FakeTable");
    }
}
