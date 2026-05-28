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
    public void DeleteBuilder_WithUsingsAndWhere_RendersExpectedSql()
    {
        Predicates where = new ColumnValue<JoinRightTable>(nameof(JoinRightTable.Id), 3);

        DeleteBuilder<JoinLeftTable> deleteBuilder = new()
        {
            Usings = [TableTag.Get<JoinRightTable>()],
            Where = where
        };

        SqlQuery query = generator.Delete(deleteBuilder);

        Assert.Equal("DELETE USING \"Right\" WHERE (\"Right\".\"Id\" = $1)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 3);
    }

    [Fact]
    public void DeleteBuilder_WithUsingsJoinsAndWhere_RendersExpectedSql()
    {
        Predicates where = new ColumnValue<JoinRightTable>(nameof(JoinRightTable.Id), 3);

        Predicates joinOn = new ColumnEqualsColumn<JoinRightTable, JoinLastTable>(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));
        Joins<JoinRightTable> joins = new InnerJoin<JoinLastTable>(joinOn);

        DeleteBuilder<JoinLeftTable, JoinRightTable> deleteBuilder = new()
        {
            Usings = [TableTag.Get<JoinRightTable>()],
            Joins = joins,
            Where = where
        };

        SqlQuery query = generator.Delete(deleteBuilder);

        Assert.Equal("DELETE FROM  \"Left\" USING \"Right\" INNER JOIN \"Last\" ON (\"Right\".\"LastId\" = \"Last\".\"Id\") WHERE (\"Right\".\"Id\" = $1)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 3);
    }
}
