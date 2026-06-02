using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsSmallInt_Default_ReturnsSmallInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsSmallInt(false);

        AssertFieldProperties(actual, "SMALLINT");
    }

    [Fact]
    public void AsSmallInt_NullableTrue_ReturnsNullableSmallInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsSmallInt(false, true);

        AssertFieldProperties(actual, "SMALLINT", isNullable: true);
    }
}
