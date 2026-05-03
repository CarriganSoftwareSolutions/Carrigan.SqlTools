using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsVarChar_ReturnsVarCharWithLength() => 
        AssertCharacter(SqlServerTypesProvider.AsVarChar(10), "VARCHAR", 10, false, false, false);

    [Fact]
    public void AsVarChar_ReturnsNullable_WhenNullableTrue() =>
        AssertCharacter(SqlServerTypesProvider.AsVarChar(10, true), "VARCHAR", 10, false, false, false, true);

    [Fact]
    public void AsVarChar_Throws_WhenLengthIsZero() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsVarChar(0));

    [Fact]
    public void AsVarChar_Throws_WhenLengthIsTooLarge() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsVarChar(8001));
}
