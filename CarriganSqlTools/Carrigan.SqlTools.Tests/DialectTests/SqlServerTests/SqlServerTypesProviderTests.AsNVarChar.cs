using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsNVarChar_ReturnsNVarCharWithLength() => 
        AssertCharacter(SqlServerTypesProvider.AsNVarChar(10), "NVARCHAR", 10, false, true, false);

    [Fact]
    public void AsNVarChar_ReturnsNullable_WhenNullableTrue() => 
        AssertCharacter(SqlServerTypesProvider.AsNVarChar(10, true), "NVARCHAR", 10, false, true, false, true);

    [Fact]
    public void AsNVarChar_Throws_WhenLengthIsZero() => Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsNVarChar(0));

    [Fact]
    public void AsNVarChar_Throws_WhenLengthIsTooLarge() => Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsNVarChar(4001));
}
