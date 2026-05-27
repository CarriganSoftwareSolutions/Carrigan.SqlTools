using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Data;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.QueryBuilderTests;

public class DeleteBuilderTests
{
    private readonly SqlGenerator<JoinLeftTable> generator = new();

    [Fact]
    public void DeleteBuilder_WithJoinsAndWhere_RendersExpectedSql()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>(nameof(JoinLeftTable.RightId)), new Column<JoinRightTable>(nameof(JoinRightTable.Id)));
        Predicates predicateId = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Id)), new Parameter("Id", 3));
        Joins<JoinLeftTable> joins = new(new InnerJoin<JoinRightTable>(joinId));

        DeleteBuilder<JoinLeftTable> deleteBuilder = new()
        {
            Joins = joins,
            Where = predicateId
        };

        SqlQuery query = generator.Delete(deleteBuilder);

        Assert.Equal("DELETE [Left] FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Id_1)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", 3);
    }
}
