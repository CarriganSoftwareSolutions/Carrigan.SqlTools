using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsText_Default_ReturnsText()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsText();

        AssertFieldProperties(actual, "TEXT", isMax: true, isUnicode: true, isFixedLength: false);
    }

    [Fact]
    public void AsText_NullableTrue_ReturnsNullableText()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsText(true);

        AssertFieldProperties(actual, "TEXT", isMax: true, isUnicode: true, isFixedLength: false, isNullable: true);
    }
}
