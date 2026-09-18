using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Theory]
    [InlineData("A", 1)]
    [InlineData("#~", 2)]
    [InlineData("Test", 4)]
    public void FromClrValue_StringWithinBoundedLimit_ReturnsNVarCharWithValueLength(string value, int expectedLength)
    {
        FieldProperties actual = SqlServerTypesProvider.FromClrValue(value);

        AssertCharacter(actual, "NVARCHAR", expectedLength, false, true, false);
    }

    [Fact]
    public void FromClrValue_EmptyString_ReturnsNVarCharLengthOne() =>
        AssertCharacter(SqlServerTypesProvider.FromClrValue(string.Empty), "NVARCHAR", 1, false, true, false);

    [Fact]
    public void FromClrValue_StringAtBoundedLimit_ReturnsNVarChar() =>
        AssertCharacter(SqlServerTypesProvider.FromClrValue(new string('x', 4000)), "NVARCHAR", 4000, false, true, false);

    [Fact]
    public void FromClrValue_StringAboveBoundedLimit_ReturnsNVarCharMax() =>
        AssertCharacter(SqlServerTypesProvider.FromClrValue(new string('x', 4001)), "NVARCHAR", null, true, true, false);

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(100)]
    public void FromClrValue_ByteArrayWithinBoundedLimit_ReturnsVarBinaryWithValueLength(int length)
    {
        FieldProperties actual = SqlServerTypesProvider.FromClrValue(new byte[length]);

        AssertBinary(actual, "VARBINARY", length, false, false);
    }

    [Fact]
    public void FromClrValue_EmptyByteArray_ReturnsVarBinaryLengthOne() =>
        AssertBinary(SqlServerTypesProvider.FromClrValue(Array.Empty<byte>()), "VARBINARY", 1, false, false);

    [Fact]
    public void FromClrValue_ByteArrayAtBoundedLimit_ReturnsVarBinary() =>
        AssertBinary(SqlServerTypesProvider.FromClrValue(new byte[8000]), "VARBINARY", 8000, false, false);

    [Fact]
    public void FromClrValue_ByteArrayAboveBoundedLimit_ReturnsVarBinaryMax() =>
        AssertBinary(SqlServerTypesProvider.FromClrValue(new byte[8001]), "VARBINARY", null, true, false);
}
