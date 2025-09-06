using SqlTools.Joins;
using SqlTools.OffsetNexts;
using SqlTools.OrderByItems;
using SqlTools.Predicates;
using SqlTools.SqlGenerators;
using SqlToolsTests.TestEntities;

namespace SqlToolsTests.GeneratorsTests;

public class SqlGenerator_SelectTests
{
    private readonly MockEncryption _mockEncryptor;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<JoinLeftTable> _sqlGeneratorForJoinLeftTable;
    private readonly SqlGenerator<ColumnTable> _sqlGeneratorForColumnTable;
    public SqlGenerator_SelectTests()
    {
        _mockEncryptor = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncryptor);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncryptor);
        _sqlGeneratorForJoinLeftTable = new SqlGenerator<JoinLeftTable>(_mockEncryptor);
        _sqlGeneratorForColumnTable = new SqlGenerator<ColumnTable>(_mockEncryptor);
    }

    [Fact]
    public void SqlSelect_WithTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.SelectAll();

        string expectedSql = "SELECT [Test].* FROM [Test]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_EmptyOrderBy()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Select(null, null, OrderBy.Empty, null);

        string expectedSql = "SELECT [Test].* FROM [Test]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_InnerInnerJoin_NoPredicates_WithTableAttribute()
    {
        PredicatesBase id = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        ISingleJoin join = new InnerJoin<JoinLeftTable, JoinRightTable>(id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(new Joins(join), null, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_InnerLeftJoin_NoPredicates_WithTableAttribute()
    {
        PredicatesBase id = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        ISingleJoin join = new LeftJoin<JoinLeftTable, JoinRightTable>(id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(new Joins(join), null, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_NoJoins_WithPredicates_WithTableAttribute()
    {
        PredicatesBase id = new Equal(new Columns<ColumnTable>("Col1"), new Parameters("Col1", 3));
        SqlQuery query = _sqlGeneratorForColumnTable.Select(null, id, null, null);

        string expectedSql = "SELECT [ColumnTable].* FROM [ColumnTable] WHERE ([ColumnTable].[Col1] = @Parameter_Col1)";
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
    public void SqlSelect_WithInnerJoin_WithPredicates_WithTableAttribute()
    {
        PredicatesBase joinId = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        PredicatesBase predicateId = new Equal(new Columns<JoinRightTable>("Id"), new Parameters("Id", 3));
        ISingleJoin join = new InnerJoin<JoinLeftTable, JoinRightTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(new Joins(join), predicateId, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Parameter_Id)";
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
    public void SqlSelect_WithLeftJoin_WithPredicates_WithTableAttribute()
    {
        PredicatesBase joinId = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        PredicatesBase predicateId = new Equal(new Columns<JoinRightTable>("Id"), new Parameters("Id", 3));
        ISingleJoin join = new LeftJoin<JoinLeftTable, JoinRightTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(new Joins(join), predicateId, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Parameter_Id)";
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
    public void SqlSelect_WithLeftAndInnerJoin_WithPredicates_WithTableAttribute()
    {
        PredicatesBase joinId1 = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        PredicatesBase joinId2 = new Equal(new Columns<JoinRightTable>("LastId"), new Columns<JoinLastTable>("Id"));
        PredicatesBase predicateId = new Equal(new Columns<JoinLastTable>("Id"), new Parameters("Id", 3));
        ISingleJoin join1 = new InnerJoin<JoinLeftTable, JoinRightTable>(joinId1);
        ISingleJoin join2 = new LeftJoin<JoinRightTable, JoinLastTable>(joinId2);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(new Joins(join1, join2), predicateId, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Parameter_Id)";
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
    public void SqlSelect_WithLeftAndInnerJoin_WithPredicates_WithOrderBy_WithTableAttribute()
    {
        PredicatesBase joinId1 = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        PredicatesBase joinId2 = new Equal(new Columns<JoinRightTable>("LastId"), new Columns<JoinLastTable>("Id"));
        PredicatesBase predicateId = new Equal(new Columns<JoinLastTable>("Id"), new Parameters("Id", 3));
        ISingleJoin join1 = new InnerJoin<JoinLeftTable, JoinRightTable>(joinId1);
        ISingleJoin join2 = new LeftJoin<JoinRightTable, JoinLastTable>(joinId2);
        OrderByItem<JoinLeftTable> orderByItem1 = new("Id", SortDirectionEnum.Ascending);
        OrderByItem<JoinRightTable> orderByItem2 = new("Id", SortDirectionEnum.Descending);
        OrderByItem<JoinLastTable> orderByItem3 = new("Id", SortDirectionEnum.Ascending);
        OrderBy orderBy = new(orderByItem1, orderByItem2, orderByItem3);
        OffsetNext offsetNext = new(new DefinePage(3, 50));
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(new Joins(join1, join2), predicateId, orderBy, offsetNext);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Parameter_Id) ORDER BY [Left].[Id] ASC, [Right].[Id] DESC, [Last].[Id] ASC OFFSET 100 ROWS FETCH NEXT 50 ROWS ONLY";
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
