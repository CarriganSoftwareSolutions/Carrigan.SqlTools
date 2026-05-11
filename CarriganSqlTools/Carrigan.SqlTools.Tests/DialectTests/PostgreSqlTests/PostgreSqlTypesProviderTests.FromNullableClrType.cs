using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void FromNullableClrType_NonNullableClrType_ReturnsNullableFieldProperties()
    {
        FieldProperties actual = PostgreSqlTypesProvider.FromNullableClrType(typeof(int));

        AssertFieldProperties(actual, "INTEGER", isNullable: true);
    }

    [Fact]
    public void FromNullableClrType_Generic_ReturnsNullableFieldProperties()
    {
        FieldProperties actual = PostgreSqlTypesProvider.FromNullableClrType<Guid>();

        AssertFieldProperties(actual, "UUID", isNullable: true);
    }

    [Fact]
    public void FromNullableClrType_NullType_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => PostgreSqlTypesProvider.FromNullableClrType(null!));
    }
}
