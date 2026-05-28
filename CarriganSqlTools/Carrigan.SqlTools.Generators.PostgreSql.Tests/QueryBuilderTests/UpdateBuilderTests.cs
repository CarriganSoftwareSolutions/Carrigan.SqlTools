using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using System.Data;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.QueryBuilderTests;

public class UpdateBuilderTests
{
    private readonly SqlGenerator<JoinLeftTable> generator = new();

    [Fact]
    public void UpdateBuilder_WithValuesUpdateColumnsJoinsAndPredicates_RendersExpectedSql()
    {
        JoinLeftTable values = new()
        {
            Col1 = "Hello",
            Col2 = "World"
        };
        Predicates joinId = new Equal(new Column<JoinLeftTable>(nameof(JoinLeftTable.RightId)), new Column<JoinRightTable>(nameof(JoinRightTable.Id)));
        Predicates predicateId = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Id)), new Parameter("Id", 3));
        Joins<JoinLeftTable> joins = new(new InnerJoin<JoinRightTable>(joinId));

        UpdateBuilder<JoinLeftTable> updateBuilder = new()
        {
            Values = values,
            UpdateColumns = new ColumnCollection<JoinLeftTable>(nameof(JoinLeftTable.Col1), nameof(JoinLeftTable.Col2)),
            Joins = joins,
            Where = predicateId
        };

        SqlQuery query = generator.Update(updateBuilder);

        Assert.Equal("UPDATE \"Left\" SET \"Col1\" = $1, \"Col2\" = $2 FROM \"Left\" INNER JOIN \"Right\" ON (\"Left\".\"RightId\" = \"Right\".\"Id\") WHERE (\"Right\".\"Id\" = $3)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hello");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "World");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", 3);
    }
}
