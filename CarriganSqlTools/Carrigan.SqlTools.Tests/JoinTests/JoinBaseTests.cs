using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
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
}
