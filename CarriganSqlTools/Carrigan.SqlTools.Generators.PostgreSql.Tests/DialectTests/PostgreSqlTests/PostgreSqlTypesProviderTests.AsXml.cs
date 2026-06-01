using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsXml_Default_ReturnsXml()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsXml();

        AssertFieldProperties(actual, "XML");
    }

    [Fact]
    public void AsXml_NullableTrue_ReturnsNullableXml()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsXml(true);

        AssertFieldProperties(actual, "XML", isNullable: true);
    }
}
