using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsSmallInt_Default_ReturnsSmallInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsSmallInt();

        AssertFieldProperties(actual, "SMALLINT");
    }

    [Fact]
    public void AsSmallInt_NullableTrue_ReturnsNullableSmallInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsSmallInt(true);

        AssertFieldProperties(actual, "SMALLINT", isNullable: true);
    }
}
