using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsBigInt_Default_ReturnsBigInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBigInt();

        AssertFieldProperties(actual, "BIGINT");
    }

    [Fact]
    public void AsBigInt_NullableTrue_ReturnsNullableBigInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBigInt(true);

        AssertFieldProperties(actual, "BIGINT", isNullable: true);
    }
}
