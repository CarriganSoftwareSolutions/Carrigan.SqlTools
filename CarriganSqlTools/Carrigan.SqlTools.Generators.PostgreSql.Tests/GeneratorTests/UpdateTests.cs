using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.GeneratorTests;

public class UpdateTests
{
    private readonly SqlGenerator<JoinLeftTable> generator = new();

    [Fact]
    public void Update_WithMultipleFromTables()
    {
        JoinLeftTable values = new()
        {
            Col1 = "Hello"
        };
        ColumnCollection<JoinLeftTable> updateColumns = new(nameof(JoinLeftTable.Col1));
        Predicates leftToRightPredicate = new Equal(new Column<JoinLeftTable>(nameof(JoinLeftTable.RightId)), new Column<JoinRightTable>(nameof(JoinRightTable.Id)));
        Predicates rightToLastPredicate = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.LastId)), new Column<JoinLastTable>(nameof(JoinLastTable.Id)));

        SqlQuery query = generator.Update
        (
            values,
            updateColumns,
            [TableTag.Get<JoinRightTable>(), TableTag.Get<JoinLastTable>()],
            new And(leftToRightPredicate, rightToLastPredicate)
        );

        string expectedSql =
            "UPDATE \"Left\" SET \"Col1\" = $1 FROM \"Right\", \"Last\" "
            + "WHERE ((\"Left\".\"RightId\" = \"Right\".\"Id\") "
            + "AND (\"Right\".\"LastId\" = \"Last\".\"Id\"))";

        Assert.Equal(expectedSql, query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hello");
    }

    [Fact]
    public void Update_WithTargetFrom_Exception()
    {
        JoinLeftTable values = new()
        {
            Col1 = "Hello"
        };
        ColumnCollection<JoinLeftTable> updateColumns = new(nameof(JoinLeftTable.Col1));

        Assert.Throws<InvalidTableException>(() => generator.Update(values, updateColumns, [TableTag.Get<JoinLeftTable>()], null));
    }

    [Fact]
    public void Update_WithJoinMissingFrom_Exception()
    {
        JoinLeftTable values = new()
        {
            Col1 = "Hello"
        };
        ColumnCollection<JoinLeftTable> updateColumns = new(nameof(JoinLeftTable.Col1));
        Predicates joinPredicate = new ColumnEqualsColumn<JoinRightTable, JoinLastTable>(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));
        Joins<JoinRightTable> joins = new InnerJoin<JoinLastTable>(joinPredicate);

        Assert.Throws<InvalidTableException>(() => generator.Update(values, updateColumns, null, joins, null));
    }

    [Fact]
    public void Update_WithJoinRootMissingFrom_Exception()
    {
        JoinLeftTable values = new()
        {
            Col1 = "Hello"
        };
        ColumnCollection<JoinLeftTable> updateColumns = new(nameof(JoinLeftTable.Col1));
        Predicates joinPredicate = new ColumnEqualsColumn<JoinRightTable, JoinLastTable>(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));
        Joins<JoinRightTable> joins = new InnerJoin<JoinLastTable>(joinPredicate);

        Assert.Throws<InvalidTableException>(() => generator.Update(values, updateColumns, [TableTag.Get<JoinLastTable>()], joins, null));
    }
}
