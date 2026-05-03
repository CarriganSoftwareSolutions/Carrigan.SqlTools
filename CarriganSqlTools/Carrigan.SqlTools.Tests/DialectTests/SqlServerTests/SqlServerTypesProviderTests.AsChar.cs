using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsChar_ReturnsCharWithLength() => 
        AssertCharacter(SqlServerTypesProvider.AsChar(10), "CHAR", 10, false, false, true);

    [Fact]
    public void AsChar_ReturnsNullable_WhenNullableTrue() => 
        AssertCharacter(SqlServerTypesProvider.AsChar(10, true), "CHAR", 10, false, false, true, true);

    [Fact]
    public void AsChar_Throws_WhenLengthIsZero() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsChar(0));

    [Fact]
    public void AsChar_Throws_WhenLengthIsTooLarge() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsChar(8001));
}
