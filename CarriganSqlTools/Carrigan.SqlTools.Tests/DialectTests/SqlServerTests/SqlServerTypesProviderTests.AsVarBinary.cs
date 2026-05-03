using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsVarBinary_ReturnsVarBinaryWithLength() => 
        AssertBinary(SqlServerTypesProvider.AsVarBinary(10), "VARBINARY", 10, false, false);

    [Fact]
    public void AsVarBinary_ReturnsNullable_WhenNullableTrue() => 
        AssertBinary(SqlServerTypesProvider.AsVarBinary(10, true), "VARBINARY", 10, false, false, true);

    [Fact]
    public void AsVarBinary_Throws_WhenLengthIsZero() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsVarBinary(0));

    [Fact]
    public void AsVarBinary_Throws_WhenLengthIsTooLarge() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsVarBinary(8001));
}
