using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_SelectTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<JoinLeftTable> _sqlGeneratorForJoinLeftTable;
    private readonly SqlGenerator<ColumnTable> _sqlGeneratorForColumnTable;
    public SqlGenerator_SelectTests()
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
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.SelectAll();

        string expectedSql = "SELECT [Test].* FROM [Test]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_EmptyOrderBy()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Select(null, null, null, OrderBy.Empty, null);

        string expectedSql = "SELECT [Test].* FROM [Test]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_InnerInnerJoin_NoPredicates_WithTableAttribute()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Joins<JoinLeftTable> join = new InnerJoin<JoinRightTable>(id).AsJoins<JoinLeftTable>();
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(null, join, null, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_InnerLeftJoin_NoPredicates_WithTableAttribute()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Joins<JoinLeftTable> join = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(null, join, null, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_NoJoins_WithPredicates_WithTableAttribute()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<ColumnTable>("Col1"), new Parameter("Col1", 3, SqlTypeDefinition.AsInt()));
        SqlQuery query = _sqlGeneratorForColumnTable.Select(null, null, id, null, null);

        string expectedSql = "SELECT [ColumnTable].* FROM [ColumnTable] WHERE ([ColumnTable].[Col1] = @Col1_1)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Col1_1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelect_WithInnerJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter("Id", 3, SqlTypeDefinition.AsInt()));
        JoinsBase relation = InnerJoin<JoinRightTable>.Joins<JoinLeftTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(null, relation, predicateId, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Id_1)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Id_1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelect_WithLeftJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter("Id", 3, SqlTypeDefinition.AsInt()));
        JoinsBase relations = LeftJoin<JoinRightTable>.Joins<JoinLeftTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(null, relations, predicateId, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Id_1)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Id_1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelect_WithLeftAndInnerJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinLastTable>("Id"), new Parameter("Id", 3, SqlTypeDefinition.AsInt()));
        InnerJoin<JoinRightTable> join1 = new(joinId1);
        LeftJoin<JoinLastTable> join2 = new(joinId2);
        Joins<JoinLeftTable> joins = new(join1, join2);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(null, joins, predicateId, null, null);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Id_1)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Id_1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelect_WithLeftAndInnerJoin_WithPredicates_WithOrderBy_WithTableAttribute()
    {
        Predicates joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinLastTable>("Id"), new Parameter("Id", 3, SqlTypeDefinition.AsInt()));
        InnerJoin<JoinRightTable> join1 = new(joinId1);
        LeftJoin<JoinLastTable> join2 = new(joinId2);
        JoinsBase joins = new Joins<JoinLeftTable>(join1, join2);
        OrderByItem<JoinLeftTable> orderByItem1 = new("Id", SortDirectionEnum.Ascending);
        OrderByItem<JoinRightTable> orderByItem2 = new("Id", SortDirectionEnum.Descending);
        OrderByItem<JoinLastTable> orderByItem3 = new("Id", SortDirectionEnum.Ascending);
        OrderByBase orderBy = new OrderBy(orderByItem1, orderByItem2, orderByItem3);
        OffsetNext offsetNext = new DefinePage(3, 50);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(null, joins, predicateId, orderBy, offsetNext);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Id_1) ORDER BY [Left].[Id] ASC, [Right].[Id] DESC, [Last].[Id] ASC OFFSET 100 ROWS FETCH NEXT 50 ROWS ONLY";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Id_1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void SqlSelect_WithLeftAndInnerJoin_WithPredicates_WithOrderByItem_AsOrderByClause_WithTableAttribute()
    {
        Predicates joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinLastTable>("Id"), new Parameter("Id", 3, SqlTypeDefinition.AsInt()));
        InnerJoin<JoinRightTable> join1 = new(joinId1);
        LeftJoin<JoinLastTable> join2 = new(joinId2);
        JoinsBase relation = new Joins<JoinLeftTable>(join1, join2);
        OrderByItem<JoinLeftTable> orderByItem = new("Id", SortDirectionEnum.Ascending);
        DefinePage offsetNext = new(3, 50);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.Select(null, relation, predicateId, orderByItem, offsetNext);

        string expectedSql = "SELECT [Left].* FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Id_1) ORDER BY [Left].[Id] ASC OFFSET 100 ROWS FETCH NEXT 50 ROWS ONLY";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Id_1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        int expectedValue = 3;
        int actualValue = (int)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }
}