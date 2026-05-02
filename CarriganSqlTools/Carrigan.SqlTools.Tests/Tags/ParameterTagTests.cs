using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Data;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Tests.Tags;

public class ParameterTagTests
{

    [Fact]
    public void ImplicitString_CreatesExpectedTag_AllPartsPresent()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnIndex()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnPrefix()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName", actual);
    }

    [Fact]
    public void ToString_ReturnsSameAsImplicitString()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        string viaCast = (string)parameterTag;
        string viaToString = parameterTag.ToString();

        Assert.Equal(viaCast, viaToString);
    }

    [Fact]
    public void CompareTo_IsCaseInsensitive()
    {
        ParameterTag left = new("ParameterName", null);
        ParameterTag right = new("parameterName", null);

        int comparison = left.CompareTo(right);

        Assert.Equal(0, comparison);
    }

    [Fact]
    public void Equals_IsCaseInsensitive_AndOperatorsMatch()
    {
        ParameterTag first = new("ParameterName", null);
        ParameterTag second = new("PARAMETERNAME", null);

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_ObjectOverride_Works()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        object boxed = new ParameterTag("parameterName", null);

        Assert.True(parameterTag.Equals(boxed));
    }

    [Fact]
    public void GetHashCode_IsCaseInsensitive_EqualObjectsHaveSameHash()
    {
        ParameterTag first = new("ParameterName", null);
        ParameterTag second = new("parameterName", null);

        int hashFirst = first.GetHashCode();
        int hashSecond = second.GetHashCode();

        Assert.Equal(hashFirst, hashSecond);
    }

    [Fact]
    public void IEqualityComparer_Equals_And_GetHashCode_Work()
    {
        ParameterTag comparer = new("ParameterName", null);
        ParameterTag x = new("ParameterName", null);
        ParameterTag y = new("parameterName", null);

        bool equalsResult = comparer.Equals(x, y);
        int hashX = comparer.GetHashCode(x);
        int hashY = comparer.GetHashCode(y);

        Assert.True(equalsResult);
        Assert.Equal(hashX, hashY);
    }

    [Fact]
    public void EqualityOperators_HandleNulls()
    {
        ParameterTag left = new("ParameterName", null);
        ParameterTag? right = null;

        Assert.False(left == right);
        Assert.True(left != right);
        Assert.True((ParameterTag?)null == (ParameterTag?)null);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_BecauseParameterNameIsRequired()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        bool isEmpty = parameterTag.IsEmpty();

        Assert.False(isEmpty);
    }

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_1() =>
        // Null parameter name
        Assert.Throws<InvalidParameterIdentifierException>(() => new ParameterTag(null!, (object)null!));

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_2() =>
        // Empty parameter name
        Assert.Throws<InvalidParameterIdentifierException>(() => new ParameterTag(string.Empty, null));

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_3() =>
        // Whitespace parameter name
        Assert.Throws<InvalidParameterIdentifierException>(() => new ParameterTag("   ", null));

    [Fact]
    public void CompareTo_NullOther_ReturnsPositive()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        int comparison = parameterTag.CompareTo(null);

        Assert.True(comparison > 0);
    }

    [Fact]
    public void ReferenceEquality_ShortCircuitInComparer()
    {
        ParameterTag parameterTag = new("ParameterName", null);
        bool equalsResult = parameterTag.Equals(parameterTag, parameterTag);

        Assert.True(equalsResult);
    }


    [Fact]
    public void ToString_OnlyParameterName()
    {
        ParameterTag tag = new("ParameterName", null);
        string actual = tag.ToString();

        Assert.Equal("ParameterName", actual);
    }


    [Theory]
    [InlineData("A", "B")]
    [InlineData("alpha", "beta")]
    public void Equals_DifferentParameterNames_False(string leftName, string rightName)
    {
        ParameterTag left = new(leftName, null);
        ParameterTag right = new(rightName, null);

        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }


    [Fact]
    public void CompareTo_DifferentValues_RespectsCaseInsensitiveOrdering()
    {
        ParameterTag a = new("Alpha", null);
        ParameterTag b = new("Beta", null);

        int comparison = a.CompareTo(b);
        int reversed = b.CompareTo(a);

        Assert.True(comparison < 0);
        Assert.True(reversed > 0);
    }

    [Fact]
    public void Equals_ObjectOverride_NonTagObject()
    {
        ParameterTag tag = new("2", null);
        object notATag = "1_2_3";

        Assert.False(tag.Equals(notATag));
    }

    [Fact]
    public void GetHashCode_DifferentLogicalValues()
    {
        ParameterTag first = new("B", null);
        ParameterTag second = new("B2", null);

        int hashFirst = first.GetHashCode();
        int hashSecond = second.GetHashCode();

        Assert.NotEqual(hashFirst, hashSecond);
    }

    [Fact]
    public void EqualityOperators_NullOnLeft()
    {
        ParameterTag? left = null;
        ParameterTag right = new("2", null);

        Assert.False(left == right);
        Assert.True(left != right);
    }


    [Fact]
    public void Constructor_NullPrefix_AndIndex_OnlyName()
    {
        ParameterTag tag = new("OnlyName", null);
        string actual = (string)tag;

        Assert.Equal("OnlyName", actual);
    }

    [Fact]
    public void Constructor_AllPartsPresent()
    {
        ParameterTag tag = new("Name", null);
        string actual = (string)tag;

        Assert.Equal("Name", actual);
    }

    [Fact]
    public void Equals_NullObject()
    {
        ParameterTag tag = new("2", null);

        Assert.False(tag.Equals((object?)null));
    }

    [Fact]
    public void CompareTo_EqualValues_ReturnsZero()
    {
        ParameterTag a = new("N", null);
        ParameterTag b = new("n", null);

        int compare = a.CompareTo(b);

        Assert.Equal(0, compare);
    }
}