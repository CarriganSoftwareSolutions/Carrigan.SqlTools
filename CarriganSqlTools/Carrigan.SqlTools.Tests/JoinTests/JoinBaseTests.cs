using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class JoinBaseTests
{
    [Fact]
    public void Join_NullPredicate_ThrowsArgumentNullException()
    {
        Predicates? predicate = null;

        Assert.Throws<ArgumentNullException>(() => new Join<JoinRightTable>(predicate!));
    }

    [Fact]
    public void InnerJoin_NullPredicate_ThrowsArgumentNullException()
    {
        Predicates? predicate = null;

        Assert.Throws<ArgumentNullException>(() => new InnerJoin<JoinRightTable>(predicate!));
    }

    [Fact]
    public void LeftJoin_NullPredicate_ThrowsArgumentNullException()
    {
        Predicates? predicate = null;

        Assert.Throws<ArgumentNullException>(() => new LeftJoin<JoinRightTable>(predicate!));
    }

    [Fact]
    public void FullJoin_NullPredicate_ThrowsArgumentNullException()
    {
        Predicates? predicate = null;

        Assert.Throws<ArgumentNullException>(() => new FullJoin<JoinRightTable>(predicate!));
    }

    [Fact]
    public void Join_JoinsOn()
    {
        Predicates predicate = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Join<JoinRightTable> join = new(predicate);

        TableTag[] actual = [.. join.JoinsOn];

        Assert.Contains(new TableTag(new SqlServerDialect(), null, "Left"), actual);
        Assert.Contains(new TableTag(new SqlServerDialect(), null, "Right"), actual);
    }
}
