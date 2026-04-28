using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

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
        JoinsBase joins = new NullJointsJoins();

        Assert.Throws<InvalidOperationException>(() => joins.ToSql());
    }

    [Fact]
    public void TableTags_WhenJointsContainsNull_ThrowsInvalidOperationException()
    {
        JoinsBase joins = new NullJointsJoins();

        _ = Assert.Throws<InvalidOperationException>(() => joins.TableTags.ToList());
    }

    [Fact]
    public void Constructor_NullJoins_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new Joins<JoinLeftTable>(null!));

    [Fact]
    public void JoinsCrossJoin()
    {
        string actual = Joins<JoinLeftTable>.CrossJoin<JoinRightTable>().ToSql();
        string expected = "CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    private sealed class NullJointsJoins : JoinsBase
    {
        protected override IEnumerable<JoinBase> Joints
        {
            get => [null!];
            set => throw new NotSupportedException();
        }

        internal override TableTag TableTag =>
            new(new SqlServerDialect(), null, "FakeTable");
    }
}
