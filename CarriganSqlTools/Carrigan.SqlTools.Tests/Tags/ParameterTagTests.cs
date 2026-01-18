
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
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        string actual = (string)parameterTag;

        Assert.Equal("FirstPrefix_ParameterName_FirstIndex", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnIndex()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", null, null);
        string actual = (string)parameterTag;

        Assert.Equal("FirstPrefix_ParameterName", actual);
    }

    [Fact]
    public void ImplicitString_SkipsNullOrWhitespaceParts_OnPrefix()
    {
        ParameterTag parameterTag = new (null, "ParameterName", "FirstIndex", null);
        string actual = (string)parameterTag;

        Assert.Equal("ParameterName_FirstIndex", actual);
    }

    [Fact]
    public void ToString_ReturnsSameAsImplicitString()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        string viaCast = (string)parameterTag;
        string viaToString = parameterTag.ToString();

        Assert.Equal(viaCast, viaToString);
    }

    [Fact]
    public void CompareTo_IsCaseInsensitive()
    {
        ParameterTag left = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        ParameterTag right = new ("firstPrefix", "parameterName", "firstIndex", null);

        int comparison = left.CompareTo(right);

        Assert.Equal(0, comparison);
    }

    [Fact]
    public void Equals_IsCaseInsensitive_AndOperatorsMatch()
    {
        ParameterTag first = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        ParameterTag second = new ("FIRSTPREFIX", "PARAMETERNAME", "FIRSTINDEX", null);

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_ObjectOverride_Works()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        object boxed = new ParameterTag("firstPrefix", "parameterName", "firstIndex", null);

        Assert.True(parameterTag.Equals(boxed));
    }

    [Fact]
    public void GetHashCode_IsCaseInsensitive_EqualObjectsHaveSameHash()
    {
        ParameterTag first = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        ParameterTag second = new ("firstPrefix", "parameterName", "firstIndex", null);

        int hashFirst = first.GetHashCode();
        int hashSecond = second.GetHashCode();

        Assert.Equal(hashFirst, hashSecond);
    }

    [Fact]
    public void IEqualityComparer_Equals_And_GetHashCode_Work()
    {
        ParameterTag comparer = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        ParameterTag x = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        ParameterTag y = new ("firstPrefix", "parameterName", "firstIndex", null    );

        bool equalsResult = comparer.Equals(x, y);
        int hashX = comparer.GetHashCode(x);
        int hashY = comparer.GetHashCode(y);

        Assert.True(equalsResult);
        Assert.Equal(hashX, hashY);
    }

    [Fact]
    public void EqualityOperators_HandleNulls()
    {
        ParameterTag left = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        ParameterTag? right = null;

        Assert.False(left == right);
        Assert.True(left != right);
        Assert.True((ParameterTag?)null == (ParameterTag?)null);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_BecauseParameterNameIsRequired()
    {
        ParameterTag parameterTag = new (null, "ParameterName", null, null);
        bool isEmpty = parameterTag.IsEmpty();

        Assert.False(isEmpty);
    }

    [Fact]
    public void PrefixPrepend_WhenNoExistingPrefix_SetsPrefix()
    {
        ParameterTag original = new (null, "ParameterName", "FirstIndex", null);
        ParameterTag updated = original.PrefixPrepend("NewPrefix");

        Assert.Equal("NewPrefix_ParameterName_FirstIndex", (string)updated);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void PrefixPrepend_WhenExistingPrefix_PrependWithUnderscore()
    {
        ParameterTag original = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        ParameterTag updated = original.PrefixPrepend("NewPrefix");

        Assert.Equal("NewPrefix_FirstPrefix_ParameterName_FirstIndex", (string)updated);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void AddIndex_WhenNoExistingIndex_AddsIndex()
    {
        ParameterTag original = new ("FirstPrefix", "ParameterName", null, null);
        ParameterTag updated = original.AddIndex("FirstIndex");

        Assert.Equal("FirstPrefix_ParameterName_FirstIndex", (string)updated);
        Assert.Equal("FirstPrefix_ParameterName", (string)original);
        // Original remains unchanged
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void AddIndex_WhenExistingIndex_ThrowsArgumentException()
    {
        ParameterTag original = new ("FirstPrefix", "ParameterName", "ExistingIndex", null);

        ArgumentException exception = Assert.Throws<ArgumentException>(() => original.AddIndex("NewIndex"));

        Assert.Contains("Index was already defined on the Parameter", exception.Message);
    }

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_1() =>
        // Null parameter name
        Assert.Throws <InvalidParameterIdentifierException>(() => new ParameterTag("FirstPrefix", null!, null, null));

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_2() =>
        // Empty parameter name
        Assert.Throws<InvalidParameterIdentifierException>(() => new ParameterTag("FirstPrefix", string.Empty, null, null));

    [Fact]
    public void Constructor_WhenParameterNameIsNullOrEmpty_Throws_3() =>
        // Whitespace parameter name
        Assert.Throws<InvalidParameterIdentifierException>(() => new ParameterTag("FirstPrefix", "   ", null, null));

    [Fact]
    public void CompareTo_NullOther_ReturnsPositive()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex", null);
        int comparison = parameterTag.CompareTo(null);

        Assert.True(comparison > 0);
    }

    [Fact]
    public void ReferenceEquality_ShortCircuitInComparer()
    {
        ParameterTag parameterTag = new ("FirstPrefix", "ParameterName", "FirstIndex", null );
        bool equalsResult = parameterTag.Equals(parameterTag, parameterTag);

        Assert.True(equalsResult);
    }


    [Fact]
    public void ToString_OnlyParameterName()
    {
        ParameterTag tag = new(null, "ParameterName", null, null);
        string actual = tag.ToString();

        Assert.Equal("ParameterName", actual);
    }


    [Theory]
    [InlineData("A", "B")]
    [InlineData("alpha", "beta")]
    public void Equals_DifferentParameterNames_False(string leftName, string rightName)
    {
        ParameterTag left = new("P", leftName, "I", null);
        ParameterTag right = new("P", rightName, "I", null);

        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }


    [Fact]
    public void CompareTo_DifferentValues_RespectsCaseInsensitiveOrdering()
    {
        ParameterTag a = new("Prefix", "Alpha", "01", null);
        ParameterTag b = new("Prefix", "Beta", "01", null);

        int comparison = a.CompareTo(b);
        int reversed = b.CompareTo(a);

        Assert.True(comparison < 0);
        Assert.True(reversed > 0);
    }

    [Fact]
    public void PrefixPrepend_Chaining()
    {
        ParameterTag original = new("FirstPrefix", "ParameterName", "Idx", null);
        ParameterTag withOne = original.PrefixPrepend("New1");
        ParameterTag withTwo = withOne.PrefixPrepend("New2");

        Assert.Equal("New1_FirstPrefix_ParameterName_Idx", (string)withOne);
        Assert.Equal("New2_New1_FirstPrefix_ParameterName_Idx", (string)withTwo);
        Assert.NotEqual(original, withOne);
        Assert.NotEqual(withOne, withTwo);
        Assert.NotSame(original, withOne);
        Assert.NotSame(withOne, withTwo);
    }

    [Fact]
    public void Equals_ObjectOverride_NonTagObject()
    {
        ParameterTag tag = new("1", "2", "3", null);
        object notATag = "1_2_3";

        Assert.False(tag.Equals(notATag));
    }

    [Fact]
    public void GetHashCode_DifferentLogicalValues()
    {
        ParameterTag first = new("A", "B", "C", null);
        ParameterTag second = new("A", "B2", "C", null);

        int hashFirst = first.GetHashCode();
        int hashSecond = second.GetHashCode();

        Assert.NotEqual(hashFirst, hashSecond);
    }

    [Fact]
    public void EqualityOperators_NullOnLeft()
    {
        ParameterTag? left = null;
        ParameterTag right = new("1", "2", "3", null);

        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void AddIndex_Then_PrefixPrepend()
    {
        ParameterTag original = new(null, "Name", null, null);
        ParameterTag withIndex = original.AddIndex("01");
        ParameterTag withPrefix = withIndex.PrefixPrepend("P");

        Assert.Equal("Name_01", (string)withIndex);
        Assert.Equal("P_Name_01", (string)withPrefix);
        Assert.NotSame(original, withIndex);
        Assert.NotSame(withIndex, withPrefix);
    }

    [Fact]
    public void Constructor_NullPrefix_AndIndex_OnlyName()
    {
        ParameterTag tag = new(null, "OnlyName", null, null);
        string actual = (string)tag;

        Assert.Equal("OnlyName", actual);
    }

    [Fact]
    public void Constructor_AllPartsPresent()
    {
        ParameterTag tag = new("P1", "Name", "I1", null);
        string actual = (string)tag;

        Assert.Equal("P1_Name_I1", actual);
    }

    [Fact]
    public void Equals_NullObject()
    {
        ParameterTag tag = new("1", "2", "3", null);

        Assert.False(tag.Equals((object?)null));
    }

    [Fact]
    public void CompareTo_EqualValues_ReturnsZero()
    {
        ParameterTag a = new("P", "N", "I", null);
        ParameterTag b = new("p", "n", "i", null);

        int compare = a.CompareTo(b);

        Assert.Equal(0, compare);
    }


    [Fact]
    public void GetParameter_Value_SetsSqlTypeOnClone()
    {
        ParameterTag original = new(null, "IntParameter", null, null);

        KeyValuePair<ParameterTag, object> result = original.GetParameter(42);

        // Original should remain without a SqlType
        Assert.Null(original.SqlType);

        // Returned key should be a different instance with a non-null SqlType
        Assert.NotSame(original, result.Key);
        Assert.NotNull(result.Key.SqlType);
        Assert.Equal(SqlDbType.Int, result.Key.SqlType.Type);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void GetParameter_ValueNullUsesDBNullAndKeepsSqlTypeNull()
    {
        ParameterTag tag = new(null, "ParameterName", null, null);

        KeyValuePair<ParameterTag, object> result = tag.GetParameter(null);

        // Uses DBNull.Value for the parameter value
        Assert.Same(DBNull.Value, result.Value);

        Assert.Equal(tag, result.Key);

        Assert.NotNull(result.Key.SqlType);
        Assert.Equal(SqlDbType.Variant, result.Key.SqlType.Type);
    }

    [Fact]
    public void GetParameter_Value_DoesNotChangeExistingSqlTypeOnOriginal()
    {
        SqlTypeDefinition presetType = SqlTypeDefinition.AsInt();
        ParameterTag original = new(null, "IntParameter", null, presetType);

        KeyValuePair<ParameterTag, object> result = original.GetParameter(123);

        // Original should keep its preset type
        Assert.Same(presetType, original.SqlType);

        // Clone should also carry the same SqlTypeDefinition instance
        Assert.Same(presetType, result.Key.SqlType);
        Assert.Equal(123, result.Value);
        Assert.NotNull(result.Key.SqlType);
        Assert.Equal(SqlDbType.Int, result.Key.SqlType.Type);
    }

    [Fact]
    public void GetParameter_XDocument_ConvertsToString()
    {
        ParameterTag tag = new(null, "XmlParam", null, null);
        XDocument document = new(new XElement("Root", new XElement("Child", "Value")));

        KeyValuePair<ParameterTag, object> result = tag.GetParameter(document);

        Assert.IsType<string>(result.Value);
        Assert.Contains("<Root>", (string)result.Value);
    }

    [Fact]
    public void GetParameter_XmlDocument_ConvertsToOuterXml()
    {
        ParameterTag tag = new(null, "XmlParam", null, null);

        XmlDocument document = new();
        document.LoadXml("<Root><Child>Value</Child></Root>");

        KeyValuePair<ParameterTag, object> result = tag.GetParameter(document);

        Assert.IsType<string>(result.Value);
        Assert.Equal("<Root><Child>Value</Child></Root>", (string)result.Value);
    }
}
