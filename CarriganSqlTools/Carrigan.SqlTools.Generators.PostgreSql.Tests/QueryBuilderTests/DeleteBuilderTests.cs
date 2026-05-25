using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.QueryBuilderTests;

public class DeleteBuilderTests
{
    private readonly SqlGenerator<JoinLeftTable> generator = new();

    [Fact]
    public void DeleteBuilder_WithUsingsJoinsAndWhere_RendersExpectedSql()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>(nameof(JoinLeftTable.RightId)), new Column<JoinRightTable>(nameof(JoinRightTable.Id)));
        Predicates predicateId = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Id)), new Parameter("Id", 3));
        Joins<JoinLeftTable> joins = new(new InnerJoin<JoinRightTable>(joinId));

        DeleteBuilder<JoinLeftTable> deleteBuilder = new()
        {
            Usings = [TableTag.Get<JoinRightTable>()],
            Joins = joins,
            Where = predicateId
        };

        SqlQuery query = generator.Delete<JoinRightTable>(deleteBuilder);

        Assert.Equal("DELETE FROM  \"Left\" USING \"Right\" INNER JOIN \"Right\" ON (\"Left\".\"RightId\" = \"Right\".\"Id\") WHERE (\"Right\".\"Id\" = $1)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 3);
    }
}
