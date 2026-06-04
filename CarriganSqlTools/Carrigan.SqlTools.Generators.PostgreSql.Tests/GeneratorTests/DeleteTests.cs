using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.GeneratorTests;

public class DeleteTests
{
    private readonly SqlGenerator<JoinLeftTable> generator = new();

    [Fact]
    public void Delete_WithUsingOnly()
    {
        SqlQuery query = generator.Delete([TableTag.Get<JoinRightTable>()], null);

        Assert.Equal("DELETE FROM \"Left\" USING \"Right\"", query.QueryText);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void Delete_WithTargetUsing_Exception()
    {
        Assert.Throws<InvalidTableException>(() => generator.Delete([TableTag.Get<JoinLeftTable>()], null));
    }

    [Fact]
    public void Delete_WithJoinMissingUsing_Exception()
    {
        Predicates joinPredicate = new ColumnEqualsColumn<JoinRightTable, JoinLastTable>(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));
        Joins<JoinRightTable> joins = new InnerJoin<JoinLastTable>(joinPredicate);

        Assert.Throws<InvalidTableException>(() => generator.Delete(null, joins, null));
    }

    [Fact]
    public void Delete_WithJoinRootMissingUsing_Exception()
    {
        Predicates joinPredicate = new ColumnEqualsColumn<JoinRightTable, JoinLastTable>(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));
        Joins<JoinRightTable> joins = new InnerJoin<JoinLastTable>(joinPredicate);

        Assert.Throws<InvalidTableException>(() => generator.Delete([TableTag.Get<JoinLastTable>()], joins, null));
    }
}
