using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsBytea_Default_ReturnsBytea()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBytea(false);

        AssertFieldProperties(actual, "BYTEA", isMax: true, isFixedLength: false);
    }

    [Fact]
    public void AsBytea_NullableTrue_ReturnsNullableBytea()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBytea(false, true);

        AssertFieldProperties(actual, "BYTEA", isMax: true, isFixedLength: false, isNullable: true);
    }
}
