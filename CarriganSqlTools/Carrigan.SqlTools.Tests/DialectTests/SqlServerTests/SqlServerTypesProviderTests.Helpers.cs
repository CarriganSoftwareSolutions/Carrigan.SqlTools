using System;
using Carrigan.SqlTools.Types;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    private static void AssertSimple(FieldProperties actual, string expectedProviderTypeName, bool expectedNullable = false)
    {
        Assert.NotNull(actual);
        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Null(actual.Length);
        Assert.Null(actual.IsMax);
        Assert.Null(actual.IsUnicode);
        Assert.Null(actual.IsFixedLength);
        Assert.Null(actual.Precision);
        Assert.Null(actual.Scale);
        Assert.Null(actual.FractionalSecondsPrecision);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }

    private static void AssertCharacter(FieldProperties actual, string expectedProviderTypeName, int? expectedLength, bool? expectedIsMax, bool? expectedIsUnicode, bool? expectedIsFixedLength, bool expectedNullable = false)
    {
        Assert.NotNull(actual);
        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Equal(expectedLength, actual.Length);
        Assert.Equal(expectedIsMax, actual.IsMax);
        Assert.Equal(expectedIsUnicode, actual.IsUnicode);
        Assert.Equal(expectedIsFixedLength, actual.IsFixedLength);
        Assert.Null(actual.Precision);
        Assert.Null(actual.Scale);
        Assert.Null(actual.FractionalSecondsPrecision);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }

    private static void AssertBinary(FieldProperties actual, string expectedProviderTypeName, int? expectedLength, bool? expectedIsMax, bool? expectedIsFixedLength, bool expectedNullable = false)
    {
        Assert.NotNull(actual);
        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Equal(expectedLength, actual.Length);
        Assert.Equal(expectedIsMax, actual.IsMax);
        Assert.Null(actual.IsUnicode);
        Assert.Equal(expectedIsFixedLength, actual.IsFixedLength);
        Assert.Null(actual.Precision);
        Assert.Null(actual.Scale);
        Assert.Null(actual.FractionalSecondsPrecision);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }

    private static void AssertDecimal(FieldProperties actual, string expectedProviderTypeName, byte? expectedPrecision, byte? expectedScale, bool expectedNullable = false)
    {
        Assert.NotNull(actual);
        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Null(actual.Length);
        Assert.Null(actual.IsMax);
        Assert.Null(actual.IsUnicode);
        Assert.Null(actual.IsFixedLength);
        Assert.Equal(expectedPrecision, actual.Precision);
        Assert.Equal(expectedScale, actual.Scale);
        Assert.Null(actual.FractionalSecondsPrecision);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }

    private static void AssertTemporal(FieldProperties actual, string expectedProviderTypeName, byte? expectedFractionalSecondsPrecision = null, bool expectedNullable = false)
    {
        Assert.NotNull(actual);
        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Null(actual.Length);
        Assert.Null(actual.IsMax);
        Assert.Null(actual.IsUnicode);
        Assert.Null(actual.IsFixedLength);
        Assert.Null(actual.Precision);
        Assert.Null(actual.Scale);
        Assert.Equal(expectedFractionalSecondsPrecision, actual.FractionalSecondsPrecision);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }

    private static void AssertProviderSpecific(FieldProperties actual, string expectedProviderTypeName, bool expectedNullable = false)
    {
        Assert.NotNull(actual);
        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }
}
