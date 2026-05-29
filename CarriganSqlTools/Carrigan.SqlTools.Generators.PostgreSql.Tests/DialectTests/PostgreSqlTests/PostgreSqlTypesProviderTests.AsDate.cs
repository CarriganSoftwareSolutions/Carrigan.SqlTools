using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsDate_Default_ReturnsDate()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsDate();

        AssertFieldProperties(actual, "DATE");
    }

    [Fact]
    public void AsDate_NullableTrue_ReturnsNullableDate()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsDate(true);

        AssertFieldProperties(actual, "DATE", isNullable: true);
    }
}
