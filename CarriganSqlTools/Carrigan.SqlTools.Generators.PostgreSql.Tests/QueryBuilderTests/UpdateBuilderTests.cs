using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.QueryBuilderTests;

public class UpdateBuilderTests
{
    private readonly SqlGenerator<JoinLeftTable> generator = new();

    [Fact]
    public void UpdateBuilder_WithFromAndWhere()
    {
        JoinLeftTable values = new()
        {
            Col1 = "Hello",
            Col2 = "World"
        };
        Predicates usingJoin = new Equal(new Column<JoinLeftTable>(nameof(JoinLeftTable.RightId)), new Column<JoinRightTable>(nameof(JoinRightTable.Id)));
        Predicates where = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Id)), new Parameter(3, "Id"));

        UpdateBuilder<JoinLeftTable> updateBuilder = new()
        {
            Values = values,
            UpdateColumns = new ColumnCollection<JoinLeftTable>(nameof(JoinLeftTable.Col1), nameof(JoinLeftTable.Col2)),
            From = [TableTag.Get<JoinRightTable>()],
            Where = new And(usingJoin, where)
        };

        SqlQuery query = generator.Update(updateBuilder);
        string expectedSql =
            "UPDATE \"Left\" SET \"Col1\" = $1, \"Col2\" = $2 FROM \"Right\" "
            + "WHERE ((\"Left\".\"RightId\" = \"Right\".\"Id\") AND (\"Right\".\"Id\" = $3))";

        Assert.Equal(expectedSql, query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hello");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "World");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", 3);
    }

    [Fact]
    public void UpdateBuilder_WithFromJoinsAndWhere()
    {
        JoinLeftTable values = new()
        {
            Col1 = "Hello",
            Col2 = "World"
        };
        Predicates usingJoin = new Equal(new Column<JoinLeftTable>(nameof(JoinLeftTable.RightId)), new Column<JoinRightTable>(nameof(JoinRightTable.Id)));
        Predicates where = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Id)), new Parameter(3, "Id"));
        Predicates lastJoin = new ColumnEqualsColumn<JoinRightTable, JoinLastTable>(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));
        Joins<JoinRightTable> joins = new(new InnerJoin<JoinLastTable>(lastJoin));

        UpdateBuilder<JoinLeftTable, JoinRightTable> updateBuilder = new()
        {
            Values = values,
            UpdateColumns = new ColumnCollection<JoinLeftTable>(nameof(JoinLeftTable.Col1), nameof(JoinLeftTable.Col2)),
            From = [TableTag.Get<JoinRightTable>()],
            Joins = joins,
            Where = new And(usingJoin, where)
        };

        SqlQuery query = generator.Update(updateBuilder);
        string expectedSql =
            "UPDATE \"Left\" SET \"Col1\" = $1, \"Col2\" = $2 FROM \"Right\" "
            + "INNER JOIN \"Last\" ON (\"Right\".\"LastId\" = \"Last\".\"Id\") "
            + "WHERE ((\"Left\".\"RightId\" = \"Right\".\"Id\") AND (\"Right\".\"Id\" = $3))";

        Assert.Equal(expectedSql, query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hello");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "World");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", 3);
    }
}
