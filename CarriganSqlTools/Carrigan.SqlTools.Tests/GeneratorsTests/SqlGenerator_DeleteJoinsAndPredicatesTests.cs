using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_DeleteJoinsAndPredicatesTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<JoinLeftTable> _sqlGeneratorForJoinLeftTable;
    private readonly SqlGenerator<ColumnTable> _sqlGeneratorForColumnTable;
    public SqlGenerator_DeleteJoinsAndPredicatesTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForJoinLeftTable = new SqlGenerator<JoinLeftTable>(_mockEncrypter);
        _sqlGeneratorForColumnTable = new SqlGenerator<ColumnTable>(_mockEncrypter);
    }

    [Fact]
    public void SqlDelete_InnerInnerJoin_NoPredicates_WithTableAttribute()
    {
        PredicatesBase id = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        IJoins join = new InnerJoin<JoinLeftTable, JoinRightTable>(id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Delete(join, null);

        string expectedSql = "DELETE FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlDelete_InnerLeftJoin_NoPredicates_WithTableAttribute()
    {
        PredicatesBase id = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        IJoins join = new LeftJoin<JoinLeftTable, JoinRightTable>(id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Delete(new Joins(join), null);

        string expectedSql = "DELETE FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);

    }

    [Fact]
    public void SqlDelete_NoJoins_WithPredicates_WithTableAttribute()
    {
        PredicatesBase id = new Equal(new Columns<ColumnTable>("Col1"), new Parameters("Col1", 3));
        SqlQuery query = _sqlGeneratorForColumnTable.Delete(null,id);

        string expectedSql = "DELETE FROM [ColumnTable] WHERE ([ColumnTable].[Col1] = @Parameter_Col1)";
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
    public void SqlDelete_WithInnerJoin_WithPredicates_WithTableAttribute()
    {
        PredicatesBase joinId = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        PredicatesBase predicateId = new Equal(new Columns<JoinRightTable>("Id"), new Parameters("Id", 3));
        IJoins join = new InnerJoin<JoinLeftTable, JoinRightTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Delete(new Joins ( [join] ), predicateId);

        string expectedSql = "DELETE FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Parameter_Id)";
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
    public void SqlDelete_WithLeftJoin_WithPredicates_WithTableAttribute()
    {
        PredicatesBase joinId = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        PredicatesBase predicateId = new Equal(new Columns<JoinRightTable>("Id"), new Parameters("Id", 3));
        IJoins join = new LeftJoin<JoinLeftTable, JoinRightTable>(joinId);
        IEnumerable<IJoins> joins = new List<IJoins>([join]);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Delete(new Joins(joins), predicateId);

        string expectedSql = "DELETE FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Parameter_Id)";
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
    public void SqlDelete_WithLeftAndInnerJoin_WithPredicates_WithTableAttribute()
    {
        PredicatesBase joinId1 = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        PredicatesBase joinId2 = new Equal(new Columns<JoinRightTable>("LastId"), new Columns<JoinLastTable>("Id"));
        PredicatesBase predicateId = new Equal(new Columns<JoinLastTable>("Id"), new Parameters("Id", 3));
        IJoins join1 = new InnerJoin<JoinLeftTable, JoinRightTable>(joinId1);
        IJoins join2 = new LeftJoin<JoinRightTable, JoinLastTable>(joinId2);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Delete(new Joins( join1, join2 ), predicateId);

        string expectedSql = "DELETE FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Parameter_Id)";
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
