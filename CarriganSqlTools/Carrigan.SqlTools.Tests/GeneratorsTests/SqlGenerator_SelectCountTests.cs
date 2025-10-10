using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_SelectCountTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<JoinLeftTable> _sqlGeneratorForJoinLeftTable;
    private readonly SqlGenerator<ColumnTable> _sqlGeneratorForColumnTable;
    public SqlGenerator_SelectCountTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForJoinLeftTable = new SqlGenerator<JoinLeftTable>(_mockEncrypter);
        _sqlGeneratorForColumnTable = new SqlGenerator<ColumnTable>(_mockEncrypter);
    }

    [Fact]
    public void SqlSelect_WithTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.SelectCount(null, null);

        string expectedSql = "SELECT COUNT(*) FROM [Test]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_InnerInnerJoin_NoPredicates_WithTableAttribute()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        InnerJoin<JoinRightTable> join = new (id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(new Joins<JoinLeftTable>(join), null);

        string expectedSql = "SELECT COUNT(*) FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_InnerLeftJoin_NoPredicates_WithTableAttribute()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> join = new (id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(join.AsJoins<JoinLeftTable>(), null);

        string expectedSql = "SELECT COUNT(*) FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_NoJoins_WithPredicates_WithTableAttribute()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<ColumnTable>("Col1"), new Parameter("Col1", 3));
        SqlQuery query = _sqlGeneratorForColumnTable.SelectCount(null, id);

        string expectedSql = "SELECT COUNT(*) FROM [ColumnTable] WHERE ([ColumnTable].[Col1] = @Parameter_Col1)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Parameter_Col1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelectCount_WithInnerJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter("Id", 3));
        Joins<JoinLeftTable> join = InnerJoin<JoinRightTable>.Joins<JoinLeftTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(join, predicateId);

        string expectedSql = "SELECT COUNT(*) FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Parameter_Id)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Parameter_Id";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelectCount_WithLeftJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter("Id", 3));
        Joins<JoinLeftTable> join = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(join, predicateId);

        string expectedSql = "SELECT COUNT(*) FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Parameter_Id)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Parameter_Id";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelectCount_WithLeftAndInnerJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinLastTable>("Id"), new Parameter("Id", 3));
        InnerJoin<JoinRightTable> join1 = new (joinId1);
        LeftJoin<JoinLastTable> join2 = new (joinId2);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(new Joins<JoinLeftTable>(join1, join2), predicateId);

        string expectedSql = "SELECT COUNT(*) FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Parameter_Id)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Parameter_Id";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }
}
