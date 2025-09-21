
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using Xunit;

namespace Carrigan.SqlTools.Tests.Tags;

public class ParameterTagTests
{
    [Fact]
    public void ImplicitString_CreatesExpectedTag_AllPartsPresent()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex");
        string actual = (string)parameterTag;

        Assert.Equal("FirstPrefix_ParameterName_FirstIndex", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnIndex()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", null);
        string actual = (string)parameterTag;

        Assert.Equal("FirstPrefix_ParameterName", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnPrefix()
    {
        ParameterTag parameterTag = new (null, "ParameterName", "FirstIndex");
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName_FirstIndex", actual);
    }

    [Fact]
    public void ToString_ReturnsSameAsImplicitString()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex");
        string viaCast = (string)parameterTag;
        string viaToString = parameterTag.ToString();

        Assert.Equal(viaCast, viaToString);
    }

    [Fact]
    public void CompareTo_IsCaseInsensitive()
    {
        ParameterTag left = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag right = new ("firstPrefix", "parameterName", "firstIndex");

        int comparison = left.CompareTo(right);

        Assert.Equal(0, comparison);
    }

    [Fact]
    public void Equals_IsCaseInsensitive_AndOperatorsMatch()
    {
        ParameterTag first = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag second = new ("FIRSTPREFIX", "PARAMETERNAME", "FIRSTINDEX");

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_ObjectOverride_Works()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex");
        object boxed = new ParameterTag("firstPrefix", "parameterName", "firstIndex");

        Assert.True(parameterTag.Equals(boxed));
    }

    [Fact]
    public void GetHashCode_IsCaseInsensitive_EqualObjectsHaveSameHash()
    {
        ParameterTag first = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag second = new ("firstPrefix", "parameterName", "firstIndex");

        int hashFirst = first.GetHashCode();
        int hashSecond = second.GetHashCode();

        Assert.Equal(hashFirst, hashSecond);
    }

    [Fact]
    public void IEqualityComparer_Equals_And_GetHashCode_Work()
    {
        ParameterTag comparer = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag x = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag y = new ("firstPrefix", "parameterName", "firstIndex");

        bool equalsResult = comparer.Equals(x, y);
        int hashX = comparer.GetHashCode(x);
        int hashY = comparer.GetHashCode(y);

        Assert.True(equalsResult);
        Assert.Equal(hashX, hashY);
    }

    [Fact]
    public void EqualityOperators_HandleNulls()
    {
        ParameterTag left = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag? right = null;

        Assert.False(left == right);
        Assert.True(left != right);
        Assert.True((ParameterTag?)null == (ParameterTag?)null);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_BecauseParameterNameIsRequired()
    {
        ParameterTag parameterTag = new (null, "ParameterName", null);
        bool isEmpty = parameterTag.IsEmpty();

        Assert.False(isEmpty);
    }

    [Fact]
    public void PrefixPrepend_WhenNoExistingPrefix_SetsPrefix()
    {
        ParameterTag original = new (null, "ParameterName", "FirstIndex");
        ParameterTag updated = original.PrefixPrepend("NewPrefix");

        Assert.Equal("NewPrefix_ParameterName_FirstIndex", (string)updated);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void PrefixPrepend_WhenExistingPrefix_PrependWithUnderscore()
    {
        ParameterTag original = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag updated = original.PrefixPrepend("NewPrefix");

        Assert.Equal("NewPrefix_FirstPrefix_ParameterName_FirstIndex", (string)updated);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void PrefixAppend_WhenNoExistingPrefix_SetsPrefix()
    {
        ParameterTag original = new (null, "ParameterName", "FirstIndex");
        ParameterTag updated = original.PrefixAppend("NewPrefix");

        Assert.Equal("NewPrefix_ParameterName_FirstIndex", (string)updated);
        Assert.Equal("ParameterName_FirstIndex", (string)original);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void PrefixAppend_WhenExistingPrefix_AppendsWithUnderscore()
    {
        ParameterTag original = new ("FirstPrefix", "ParameterName", "FirstIndex");
        ParameterTag updated = original.PrefixAppend("SecondPrefix");

        Assert.Equal("FirstPrefix_SecondPrefix_ParameterName_FirstIndex", (string)updated);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void AddIndex_WhenNoExistingIndex_AddsIndex()
    {
        ParameterTag original = new ("FirstPrefix", "ParameterName", null);
        ParameterTag updated = original.AddIndex("FirstIndex");

        Assert.Equal("FirstPrefix_ParameterName_FirstIndex", (string)updated);
        Assert.Equal("FirstPrefix_ParameterName", (string)original);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void AddIndex_WhenExistingIndex_ThrowsArgumentException()
    {
        ParameterTag original = new ("FirstPrefix", "ParameterName", "ExistingIndex");

        ArgumentException exception = Assert.Throws<ArgumentException>(() => original.AddIndex("NewIndex"));

        Assert.Contains("Index was already defined on the Parameter", exception.Message);
    }

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_1() =>
        // Null parameter name
        Assert.Throws<ArgumentNullException>(() => new ParameterTag("FirstPrefix", null!, null));

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_2() =>
        // Empty parameter name
        Assert.Throws<ArgumentNullException>(() => new ParameterTag("FirstPrefix", string.Empty, null));

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_3() =>
        // Whitespace parameter name
        Assert.Throws<ArgumentNullException>(() => new ParameterTag("FirstPrefix", "   ", null));

    [Fact]
    public void CompareTo_NullOther_ReturnsPositive()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex");
        int comparison = parameterTag.CompareTo(null);

        Assert.True(comparison > 0);
    }

    [Fact]
    public void ReferenceEquality_ShortCircuitInComparer()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex");
        bool equalsResult = parameterTag.Equals(parameterTag, parameterTag);

        Assert.True(equalsResult);
    }
}
