using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_UpdateJoinsAndPredicatesTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<JoinLeftTable> _sqlGeneratorForJoinLeftTable;
    private readonly SetColumns<JoinLeftTable> _leftLabelSetColumns = new("Col1", "Col2");
    public SqlGenerator_UpdateJoinsAndPredicatesTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForJoinLeftTable = new SqlGenerator<JoinLeftTable>(_mockEncrypter);
    }

    [Fact]
    public void SqlUpdate_WithInnerJoin_WithJoinsAndPredicates()
    {
        JoinLeftTable entity = new()
        {
            Col1 = "Hello",
            Col2 = "World"
        };

        PredicateBase joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        PredicateBase predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter("Id", 3));
        IJoins join = new InnerJoin<JoinLeftTable, JoinRightTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Update(entity, _leftLabelSetColumns, new Joins([join]), predicateId);

        string expectedSql = "UPDATE [Left] SET [Left].[Col1] = @ParameterSet_Col1, [Left].[Col2] = @ParameterSet_Col2 FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Parameter_Id)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 3;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Parameter_Id").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        string expectedStringValue = "Hello";
        string actualStringValue = (string)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@ParameterSet_Col1").Single().Value;

        Assert.Equal(expectedStringValue, actualStringValue);

        expectedStringValue = "World";
        actualStringValue = (string)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@ParameterSet_Col2").Single().Value;

        Assert.Equal(expectedStringValue, actualStringValue);
    }

    [Fact]
    public void SqlUpdate_WithLeftAndInnerJoin_WithPredicates_WithTableAttribute()
    {
        JoinLeftTable entity = new()
        {
            Col1 = "Hello",
            Col2 = "World"
        };

        PredicateBase joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        PredicateBase joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        PredicateBase predicateId = new Equal(new Column<JoinLastTable>("Id"), new Parameter("Id", 3));
        IJoins join1 = new InnerJoin<JoinLeftTable, JoinRightTable>(joinId1);
        IJoins join2 = new LeftJoin<JoinRightTable, JoinLastTable>(joinId2);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Update(entity, _leftLabelSetColumns, new Joins(join1, join2), predicateId);

        string expectedSql = "UPDATE [Left] SET [Left].[Col1] = @ParameterSet_Col1, [Left].[Col2] = @ParameterSet_Col2 FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Parameter_Id)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 3;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Parameter_Id").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        string expectedStringValue = "Hello";
        string actualStringValue = (string)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@ParameterSet_Col1").Single().Value;

        Assert.Equal(expectedStringValue, actualStringValue);

        expectedStringValue = "World";
        actualStringValue = (string)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@ParameterSet_Col2").Single().Value;

        Assert.Equal(expectedStringValue, actualStringValue);
    }
}
