using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsInteger_Default_ReturnsInteger()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsInteger(false);

        AssertFieldProperties(actual, "INTEGER");
    }

    [Fact]
    public void AsInteger_NullableTrue_ReturnsNullableInteger()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsInteger(false, true);

        AssertFieldProperties(actual, "INTEGER", isNullable: true);
    }
}
