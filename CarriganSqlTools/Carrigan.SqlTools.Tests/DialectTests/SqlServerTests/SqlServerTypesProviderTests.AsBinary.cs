using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsBinary_ReturnsBinaryWithLength() => 
        AssertBinary(SqlServerTypesProvider.AsBinary(10), "BINARY", 10, false, true);

    [Fact]
    public void AsBinary_ReturnsNullable_WhenNullableTrue() => 
        AssertBinary(SqlServerTypesProvider.AsBinary(10, true), "BINARY", 10, false, true, true);

    [Fact]
    public void AsBinary_Throws_WhenLengthIsZero() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            SqlServerTypesProvider.AsBinary(0));

    [Fact]
    public void AsBinary_Throws_WhenLengthIsTooLarge() => 
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SqlServerTypesProvider.AsBinary(8001));
}
