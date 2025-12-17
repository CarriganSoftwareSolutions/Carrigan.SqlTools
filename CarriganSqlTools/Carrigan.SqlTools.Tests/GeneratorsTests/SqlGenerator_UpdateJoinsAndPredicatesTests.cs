using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_UpdateJoinsAndPredicatesTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<JoinLeftTable> _sqlGeneratorForJoinLeftTable;
    private readonly ColumnCollection<JoinLeftTable> _leftLabelColumnCollection = new("Col1", "Col2");
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

        Predicates joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter("Id", 3));
        InnerJoin<JoinRightTable> join = new (joinId);
        Joins<JoinLeftTable> joins = new(join);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Update(entity, _leftLabelColumnCollection, joins, predicateId);

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

        Predicates joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinLastTable>("Id"), new Parameter("Id", 3));
        InnerJoin<JoinRightTable> join1 = new (joinId1);
        LeftJoin<JoinLastTable> join2 = new (joinId2);
        Joins<JoinLeftTable> joins = new(join1, join2);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Update(entity, _leftLabelColumnCollection, joins, predicateId);

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


    [Fact]
    public void InvalidPredicateTable_WithJoins()
    {
        JoinLeftTable entity = new()
        {
            Col1 = "Hello",
            Col2 = "World"
        };

        Predicates joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        Predicates predicateId = new Equal(new Column<Customer>("Id"), new Parameter("Id", 3));

        InnerJoin<JoinRightTable> join1 = new(joinId1);
        LeftJoin<JoinLastTable> join2 = new(joinId2);
        Joins<JoinLeftTable> joins = new(join1, join2);
        Assert.Throws<InvalidTableException>(() => _ = _sqlGeneratorForJoinLeftTable.Update(entity, _leftLabelColumnCollection, joins, predicateId));
    }
    [Fact]
    public void InvalidPredicateTable_WithoutJoins()
    {
        JoinLeftTable entity = new()
        {
            Col1 = "Hello",
            Col2 = "World"
        };

        Predicates predicateId = new Equal(new Column<Customer>("Id"), new Parameter("Id", 3));

        Assert.Throws<InvalidTableException>(() => _ = _sqlGeneratorForJoinLeftTable.Update(entity, _leftLabelColumnCollection, null, predicateId));
    }
}
