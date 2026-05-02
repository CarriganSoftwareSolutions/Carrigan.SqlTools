using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_SelectByIdTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<CompositePrimaryKeyTable> _sqlGeneratorForCompositeKeyTable;
    private readonly SqlGenerator<Address> _sqlGeneratorForAddress;
    public SqlGenerator_SelectByIdTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForCompositeKeyTable = new SqlGenerator<CompositePrimaryKeyTable>(_mockEncrypter);
        _sqlGeneratorForAddress = new();
    }

    private readonly Guid _guid = new("711c4dff-6e8a-4e43-9eab-b83115244a57");
    private readonly Guid _guid2 = new("349cc712-281d-4abe-8470-6ed391391bd1");

    [Fact]
    public void SqlSelectById_WithTableAttribute()
    {
        EntityWithTableAttribute entity = new() { Id = _guid };
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.SelectById(entity);

        string expectedSql = $"SELECT [Test].* FROM [Test] WHERE ([Test].[Id] = @Id_1)";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 1;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        string expectedParameter = "@Id_1";
        string actualParameter = query.Parameters.AsEnumerable().Single().Key;

        Assert.Equal(expectedParameter, actualParameter);

        Guid expectedValue = _guid;
        Guid actualValue = (Guid)query.Parameters.AsEnumerable().Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelectById_Multiples_WithTableAttribute()
    {
        EntityWithTableAttribute[] entities = [new() { Id = _guid }, new() { Id = _guid2 }];
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.SelectById(entities);

        string expectedSql = $"SELECT [Test].* FROM [Test] WHERE (([Test].[Id] = @Id_1) OR ([Test].[Id] = @Id_2))";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 2;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        Guid expectedValue = _guid;
        Guid actualValue = (Guid)query.Parameters.Where(parameter => parameter.Key == "@Id_1").Single().Value;
        Assert.Equal(expectedValue, actualValue);

        expectedValue = _guid2;
        actualValue = (Guid)query.Parameters.Where(parameter => parameter.Key == "@Id_2").Single().Value;
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelectById_Composite_Key()
    {
        CompositePrimaryKeyTable entity = new() { Id1 = 1, Id2 = 2, NotKey1 = 5, NotKey2 = 6, NotKey3 = 7 };
        SqlQuery query = _sqlGeneratorForCompositeKeyTable.SelectById(entity);

        string expectedSql = $"SELECT [Ck].* FROM [Ck] WHERE (([Ck].[Id1] = @Id1_1) AND ([Ck].[Id2] = @Id2_2))";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 2;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        int expectedValue = 1;
        int actualValue = (int)query.Parameters.Where(parameter => parameter.Key == "@Id1_1").Single().Value;
        Assert.Equal(expectedValue, actualValue);

        expectedValue = 2;
        actualValue = (int)query.Parameters.Where(parameter => parameter.Key == "@Id2_2").Single().Value;
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlSelectById_Composite_Key_Multiple()
    {
        CompositePrimaryKeyTable entity1 = new() { Id1 = 1, Id2 = 2, NotKey1 = 5, NotKey2 = 6, NotKey3 = 7 };
        CompositePrimaryKeyTable entity2 = new() { Id1 = 3, Id2 = 4, NotKey1 = 5, NotKey2 = 6, NotKey3 = 7 };
        SqlQuery query = _sqlGeneratorForCompositeKeyTable.SelectById([entity1, entity2]);

        string expectedSql = $"SELECT [Ck].* FROM [Ck] WHERE ((([Ck].[Id1] = @Id1_1) AND ([Ck].[Id2] = @Id2_2)) OR (([Ck].[Id1] = @Id1_3) AND ([Ck].[Id2] = @Id2_4)))";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 4;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        int expectedValue = 1;
        int actualValue = (int)query.Parameters.Where(parameter => parameter.Key == "@Id1_1").Single().Value;
        Assert.Equal(expectedValue, actualValue);

        expectedValue = 2;
        actualValue = (int)query.Parameters.Where(parameter => parameter.Key == "@Id2_2").Single().Value;
        Assert.Equal(expectedValue, actualValue);

        expectedValue = 3;
        actualValue = (int)query.Parameters.Where(parameter => parameter.Key == "@Id1_3").Single().Value;
        Assert.Equal(expectedValue, actualValue);

        expectedValue = 4;
        actualValue = (int)query.Parameters.Where(parameter => parameter.Key == "@Id2_4").Single().Value;
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Throws_NoPrimaryKeyException()
    {
        IEnumerable<Address> entities =
        [
            new() { City = "Clarksville", PostalCode = "37043", Street = "Madison" }
        ];
        Assert.Throws<NoPrimaryKeyPropertyException<Address>>(() => _sqlGeneratorForAddress.SelectById(entities));
    }
}