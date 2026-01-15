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

        Assert.Contains(new TableTag(null, "Left"), actual);
        Assert.Contains(new TableTag(null, "Right"), actual);
    }

    [Fact]
    public void Join_GetParameters()
    {
        Predicates predicate = new Equal(new Column<JoinLeftTable>("RightId"), new Parameter("Id", 5));
        Join<JoinRightTable> join = new(predicate);

        Dictionary<ParameterTag, object> actual = join.GetParameters("JoinParameter");

        Assert.Single(actual);
        Assert.Equal("@JoinParameter_Id", (string)actual.Keys.Single());
        Assert.Equal(5, actual.Values.Single());
    }

    [Fact]
    public void Join_GetParameters_NullBranchPrefix_ThrowsArgumentNullException()
    {
        Predicates predicate = new Equal(new Column<JoinLeftTable>("RightId"), new Parameter("Id", 5));
        Join<JoinRightTable> join = new(predicate);

        Assert.Throws<ArgumentNullException>(() => join.GetParameters(null!));
    }
}
