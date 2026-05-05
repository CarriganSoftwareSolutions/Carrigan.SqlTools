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
        ParameterTag parameterTag = new("ParameterName");
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnIndex()
    {
        ParameterTag parameterTag = new("ParameterName");
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnPrefix()
    {
        ParameterTag parameterTag = new("ParameterName");
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName", actual);
    }

    [Fact]
    public void ToString_ReturnsSameAsImplicitString()
    {
        ParameterTag parameterTag = new("ParameterName");
        string viaCast = (string)parameterTag;
        string viaToString = parameterTag.ToString();

        Assert.Equal(viaCast, viaToString);
    }

    [Fact]
    public void CompareTo_IsCaseInsensitive()
    {
        ParameterTag left = new("ParameterName");
        ParameterTag right = new("parameterName");

        int comparison = left.CompareTo(right);

        Assert.Equal(0, comparison);
    }

    [Fact]
    public void Equals_IsCaseInsensitive_AndOperatorsMatch()
    {
        ParameterTag first = new("ParameterName");
        ParameterTag second = new("PARAMETERNAME");

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_ObjectOverride_Works()
    {
        ParameterTag parameterTag = new("ParameterName");
        object boxed = new ParameterTag("parameterName");

        Assert.True(parameterTag.Equals(boxed));
    }

    [Fact]
    public void GetHashCode_IsCaseInsensitive_EqualObjectsHaveSameHash()
    {
        ParameterTag first = new("ParameterName");
        ParameterTag second = new("parameterName");

        int hashFirst = first.GetHashCode();
        int hashSecond = second.GetHashCode();

        Assert.Equal(hashFirst, hashSecond);
    }

    [Fact]
    public void IEqualityComparer_Equals_And_GetHashCode_Work()
    {
        ParameterTag comparer = new("ParameterName");
        ParameterTag x = new("ParameterName");
        ParameterTag y = new("parameterName");

        bool equalsResult = comparer.Equals(x, y);
        int hashX = comparer.GetHashCode(x);
        int hashY = comparer.GetHashCode(y);

        Assert.True(equalsResult);
        Assert.Equal(hashX, hashY);
    }

    [Fact]
    public void EqualityOperators_HandleNulls()
    {
        ParameterTag left = new("ParameterName");
        ParameterTag? right = null;

        Assert.False(left == right);
        Assert.True(left != right);
        Assert.True((ParameterTag?)null == (ParameterTag?)null);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_BecauseParameterNameIsRequired()
    {
        ParameterTag parameterTag = new("ParameterName");
        bool isEmpty = parameterTag.IsEmpty();

        Assert.False(isEmpty);
    }

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_2() =>
        // Empty parameter name
        Assert.Throws<InvalidParameterIdentifierException>(() => new ParameterTag(string.Empty));

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_3() =>
        // Whitespace parameter name
        Assert.Throws<InvalidParameterIdentifierException>(() => new ParameterTag("   "));

    [Fact]
    public void CompareTo_NullOther_ReturnsPositive()
    {
        ParameterTag parameterTag = new("ParameterName");
        int comparison = parameterTag.CompareTo(null);

        Assert.True(comparison > 0);
    }

    [Fact]
    public void ReferenceEquality_ShortCircuitInComparer()
    {
        ParameterTag parameterTag = new("ParameterName");
        bool equalsResult = parameterTag.Equals(parameterTag, parameterTag);

        Assert.True(equalsResult);
    }


    [Fact]
    public void ToString_OnlyParameterName()
    {
        ParameterTag tag = new("ParameterName");
        string actual = tag.ToString();

        Assert.Equal("ParameterName", actual);
    }


    [Theory]
    [InlineData("A", "B")]
    [InlineData("alpha", "beta")]
    public void Equals_DifferentParameterNames_False(string leftName, string rightName)
    {
        ParameterTag left = new(leftName);
        ParameterTag right = new(rightName);

        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }


    [Fact]
    public void CompareTo_DifferentValues_RespectsCaseInsensitiveOrdering()
    {
        ParameterTag a = new("Alpha");
        ParameterTag b = new("Beta");

        int comparison = a.CompareTo(b);
        int reversed = b.CompareTo(a);

        Assert.True(comparison < 0);
        Assert.True(reversed > 0);
    }

    [Fact]
    public void Equals_ObjectOverride_NonTagObject()
    {
        ParameterTag tag = new("2");
        object notATag = "1_2_3";

        Assert.False(tag.Equals(notATag));
    }

    [Fact]
    public void GetHashCode_DifferentLogicalValues()
    {
        ParameterTag first = new("B");
        ParameterTag second = new("B2");

        int hashFirst = first.GetHashCode();
        int hashSecond = second.GetHashCode();

        Assert.NotEqual(hashFirst, hashSecond);
    }

    [Fact]
    public void EqualityOperators_NullOnLeft()
    {
        ParameterTag? left = null;
        ParameterTag right = new("2");

        Assert.False(left == right);
        Assert.True(left != right);
    }


    [Fact]
    public void Constructor_NullPrefix_AndIndex_OnlyName()
    {
        ParameterTag tag = new("OnlyName");
        string actual = (string)tag;

        Assert.Equal("OnlyName", actual);
    }

    [Fact]
    public void Constructor_AllPartsPresent()
    {
        ParameterTag tag = new("Name");
        string actual = (string)tag;

        Assert.Equal("Name", actual);
    }

    [Fact]
    public void Equals_NullObject()
    {
        ParameterTag tag = new("2");

        Assert.False(tag.Equals((object?)null));
    }

    [Fact]
    public void CompareTo_EqualValues_ReturnsZero()
    {
        ParameterTag a = new("N");
        ParameterTag b = new("n");

        int compare = a.CompareTo(b);

        Assert.Equal(0, compare);
    }
}