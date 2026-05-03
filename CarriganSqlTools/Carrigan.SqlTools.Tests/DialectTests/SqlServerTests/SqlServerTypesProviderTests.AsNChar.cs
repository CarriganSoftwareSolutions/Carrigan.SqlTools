using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsNChar_ReturnsNCharWithLength() =>
        AssertCharacter(SqlServerTypesProvider.AsNChar(10), "NCHAR", 10, false, true, true);

    [Fact]
    public void AsNChar_ReturnsNullable_WhenNullableTrue() =>
        AssertCharacter(SqlServerTypesProvider.AsNChar(10, true), "NCHAR", 10, false, true, true, true);

    [Fact]
    public void AsNChar_Throws_WhenLengthIsZero() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsNChar(0));

    [Fact]
    public void AsNChar_Throws_WhenLengthIsTooLarge() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsNChar(4001));
}
