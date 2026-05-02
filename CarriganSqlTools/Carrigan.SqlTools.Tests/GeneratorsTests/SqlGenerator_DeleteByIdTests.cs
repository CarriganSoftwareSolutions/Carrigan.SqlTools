using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

//ignore spelling: fbaa
public class SqlGenerator_DeleteByIdTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<CompositePrimaryKeyTable> _sqlGeneratorForCompositeKeyTable;
    private readonly SqlGenerator<Address> _sqlGeneratorForAddress;

    public SqlGenerator_DeleteByIdTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new(_mockEncrypter);
        _sqlGeneratorForCompositeKeyTable = new(_mockEncrypter);
        _sqlGeneratorForAddress = new();
    }

    [Fact]
    public void SqlDeleteAllByIds_GeneratesCorrectSql_WithTableAttribute()
    {
        IEnumerable<EntityWithTableAttribute> entities =
        [
            new() {
                Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")
            },
            new() {
                Id = new Guid("41369484-fbaa-4c90-aae3-8b2199afa50f")
            },
            new() {
                Id = new Guid("f98360bc-1f3d-466d-a1e7-f3a8184a7a15")
            }
        ];

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.DeleteById([.. entities]);

        string expectedSql = "DELETE FROM [Test] WHERE (([Test].[Id] = @Id_1) OR ([Test].[Id] = @Id_2) OR ([Test].[Id] = @Id_3))";
        Assert.Equal(expectedSql, query.QueryText);
    }


    [Fact]
    public void SqlDeleteById_Composite_Key()
    {
        CompositePrimaryKeyTable entity = new() { Id1 = 1, Id2 = 2, NotKey1 = 5, NotKey2 = 6, NotKey3 = 7 };
        SqlQuery query = _sqlGeneratorForCompositeKeyTable.DeleteById(entity);

        string expectedSql = $"DELETE FROM [Ck] WHERE (([Ck].[Id1] = @Id1_1) AND ([Ck].[Id2] = @Id2_2))";
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
    public void Throws_NoPrimaryKeyException()
    {
        IEnumerable<Address> entities =
        [
            new() { City = "Clarksville", PostalCode = "37043", Street = "Madison" }
        ];
        Assert.Throws<NoPrimaryKeyPropertyException<Address>>(() => _sqlGeneratorForAddress.DeleteById(entities));
    }
}