using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

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
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SqlSelect_WithTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.SelectCount(null, null, null, null);

        string expectedSql = "SELECT COUNT([Test].[Id]) FROM [Test]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SqlSelect_InnerInnerJoin_NoPredicates_WithTableAttribute()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        InnerJoin<JoinRightTable> join = new(id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(null, null, new Joins<JoinLeftTable>(join), null);

        string expectedSql = "SELECT COUNT([Left].[Id]) FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SqlSelect_InnerLeftJoin_NoPredicates_WithTableAttribute()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> join = new(id);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(null, null, join.AsJoins<JoinLeftTable>(), null);

        string expectedSql = "SELECT COUNT([Left].[Id]) FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SqlSelect_NoJoins_WithPredicates_WithTableAttribute()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<ColumnTable>("Col1"), new Parameter(3, "Col1"));
        SqlQuery query = _sqlGeneratorForColumnTable.SelectCount(null, null, null, id);

        string expectedSql = "SELECT COUNT([ColumnTable].[Col1]) FROM [ColumnTable] WHERE ([ColumnTable].[Col1] = @Col1_1)";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Col1_1", 3);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SqlSelectCount_WithInnerJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter(3, "Id"));
        Joins<JoinLeftTable> join = InnerJoin<JoinRightTable>.Joins<JoinLeftTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(null, null, join, predicateId);

        string expectedSql = "SELECT COUNT([Left].[Id]) FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Id_1)";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", 3);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SqlSelectCount_WithLeftJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinRightTable>("Id"), new Parameter(3, "Id"));
        Joins<JoinLeftTable> join = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(joinId);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(null, null, join, predicateId);

        string expectedSql = "SELECT COUNT([Left].[Id]) FROM [Left] LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Id] = @Id_1)";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query,"@Id_1", 3);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SqlSelectCount_WithLeftAndInnerJoin_WithPredicates_WithTableAttribute()
    {
        Predicates joinId1 = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Predicates joinId2 = new Equal(new Column<JoinRightTable>("LastId"), new Column<JoinLastTable>("Id"));
        Predicates predicateId = new Equal(new Column<JoinLastTable>("Id"), new Parameter(3, "Id"));
        InnerJoin<JoinRightTable> join1 = new(joinId1);
        LeftJoin<JoinLastTable> join2 = new(joinId2);
        SqlQuery query = _sqlGeneratorForJoinLeftTable.SelectCount(null, null, new Joins<JoinLeftTable>(join1, join2), predicateId);

        string expectedSql = "SELECT COUNT([Left].[Id]) FROM [Left] INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id]) WHERE ([Last].[Id] = @Id_1)";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", 3);
    }
}