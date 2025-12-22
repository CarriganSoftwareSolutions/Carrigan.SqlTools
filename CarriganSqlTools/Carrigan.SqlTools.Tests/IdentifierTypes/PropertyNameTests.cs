using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.IdentifierTypes;
public class PropertyNameTests
{
    private readonly string white = " ";
    private readonly string? nul = null;
    private readonly string empty = string.Empty;
    private readonly decimal pi = 3.14159m;
    private readonly decimal t = 6.28318m;
    private readonly decimal e = 2.71828m;
    private readonly decimal golden = 1.618m;
    private readonly decimal zero = 0m;
    private readonly string eStr = "E is 2.71828";
    private readonly string eStrAlt = "E is 2.71828";
    private readonly string goldenString = "Golden Ratio is 1.618";
    private readonly string tString = "t is 6.28318m";
    private readonly string piString = "Pi is 3.14159m";
    [Fact]
    public void Constructor_Null()
    {
        PropertyName nameWrapper = new (nul);
        Assert.NotNull(nameWrapper);
    }

    [Fact]
    public void New_Null()
    {
        PropertyName? nameWrapper = PropertyName.New(nul);
        Assert.Null(nameWrapper);
    }

    [Fact]
    public void New_Empty()
    {
        PropertyName? nameWrapper = PropertyName.New(empty);
        Assert.Null(nameWrapper);
    }

    [Fact]
    public void New_White()
    {
        PropertyName? nameWrapper = PropertyName.New(white);
        Assert.NotNull(nameWrapper);
    }


    [Fact]
    public void ToString_Null()
    {
        PropertyName nameWrapper = new(nul);
        Assert.Equal(string.Empty, nameWrapper.ToString());
    }

    [Fact]
    public void ToString_Empty()
    {
        PropertyName nameWrapper = new(empty);
        Assert.Equal(empty, nameWrapper.ToString());
    }

    [Fact]
    public void ToString_Text()
    {
        PropertyName nameWrapper = new(eStr);
        Assert.Equal(eStr, nameWrapper.ToString());
    }

    [Fact]
    public void Equal_Null()
    {
        string? name = null;

        PropertyName nameWrapper1 = new(name);
        PropertyName nameWrapper2 = new(name);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(string.Empty, nameWrapper2);
    }

    [Fact]
    public void Equal_Empty()
    {
        string? name = string.Empty;

        PropertyName nameWrapper1 = new(name);
        PropertyName nameWrapper2 = new(name);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(string.Empty, nameWrapper2);
    }

    [Fact]
    public void Equal_Default_Text()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(eStrAlt);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(eStr, nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Null()
    {
        PropertyName nameWrapper1 = new(nul);
        PropertyName nameWrapper2 = new(nul);
        Assert.True(nameWrapper1 == nameWrapper2);
        Assert.True(string.Empty == nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Empty()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(empty);
        Assert.True(nameWrapper1 == nameWrapper2);
        Assert.True(string.Empty == nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Text()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(eStrAlt);
        bool test = eStr == nameWrapper2;
        bool test2 = eStr != nameWrapper2;
        Assert.True(nameWrapper1 == nameWrapper2);
        Assert.True(test);
        Assert.False(nameWrapper1 != nameWrapper2);
        Assert.False(test2);
    }

    [Fact]
    public void EqualEqual_NullComparisons()
    {
        PropertyName nonNull = new(string.Empty);
        PropertyName? isNull = null;

        Assert.False(nonNull == isNull);
        Assert.False(isNull == nonNull);
    }

    [Fact]
    public void NotEqualEqual_NullComparisons()
    {
        PropertyName nonNull = new(string.Empty);
        PropertyName? isNull = null;
        Assert.True(nonNull != isNull);
        Assert.True(isNull != nonNull);
    }

    [Fact]
    public void DictionaryKey_Null()
    {
        string? name = null;
        Dictionary<PropertyName, decimal> dictionary= [];

        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(name);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Empty()
    {
        Dictionary<PropertyName, decimal> dictionary = [];

        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(empty);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Text()
    {
        string? name = eStr;
        Dictionary<PropertyName, decimal> dictionary = [];

        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(eStrAlt);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Multi()
    {
        string? name = eStr;
        Dictionary<PropertyName, decimal> dictionary = [];

        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(eStrAlt);
        PropertyName nameWrapper3 = new(tString);
        PropertyName nameWrapper4 = new(goldenString);
        PropertyName nameWrapper5 = new(piString);
        PropertyName nameWrapper6 = new(empty);
        dictionary[nameWrapper1] = e;
        dictionary[nameWrapper3] = t;
        dictionary[nameWrapper4] = pi;
        dictionary[nameWrapper5] = golden;
        dictionary[nameWrapper6] = zero;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == e);
        Assert.True(dictionary[nameWrapper2] == e);
        Assert.True(dictionary[nameWrapper3] == t);
        Assert.True(dictionary[nameWrapper4] == pi);
        Assert.True(dictionary[nameWrapper5] == golden);
        Assert.True(dictionary[nameWrapper6] == zero);
        Assert.Equal(5, dictionary.Count);
    }



    [Fact]
    public void NotEqual_Null()
    {
        PropertyName nameWrapper1 = new(nul);
        PropertyName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(empty, nameWrapper2);
    }

    [Fact]
    public void NotEqual_Empty()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(empty, nameWrapper2);
    }

    [Fact]
    public void NotEqual_Text()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(eStr, nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Null()
    {
        PropertyName nameWrapper1 = new(nul);
        PropertyName nameWrapper2 = new(eStr);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(empty != nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Empty()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(eStr);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(empty != nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Text()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(goldenString);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(nameWrapper1 != nameWrapper2);
    }

    [Fact]
    public void IsEmpty_True()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        Assert.True(nameWrapper1.IsEmpty());
        Assert.True(nameWrapper2.IsEmpty());
    }
    [Fact]
    public void IsNullOrEmpty_True()
    {
        PropertyName? nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        PropertyName? nameWrapper3 = null;
        PropertyName? nameWrapper4 = PropertyName.New(nul);
        PropertyName? nameWrapper5 = PropertyName.New(empty);
        Assert.True(nameWrapper1.IsNullOrEmpty());
        Assert.True(nameWrapper2.IsNullOrEmpty());
        Assert.True(nameWrapper3.IsNullOrEmpty());
        Assert.True(nameWrapper4.IsNullOrEmpty());
        Assert.True(nameWrapper5.IsNullOrEmpty());
    }

    [Fact]
    public void IsEmpty_False()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(white);
        Assert.False(nameWrapper1.IsEmpty());
        Assert.False(nameWrapper2.IsEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_False()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(white);
        PropertyName? nameWrapper3 = PropertyName.New(eStr);
        PropertyName? nameWrapper4 = PropertyName.New(white);
        Assert.False(nameWrapper1.IsNullOrEmpty());
        Assert.False(nameWrapper2.IsNullOrEmpty());
        Assert.False(nameWrapper3.IsNullOrEmpty());
        Assert.False(nameWrapper4.IsNullOrEmpty());
    }

    [Fact]
    public void IsNotEmpty_False()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        Assert.False(nameWrapper1.IsNotEmpty());
        Assert.False(nameWrapper2.IsNotEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_False()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        PropertyName? nameWrapper3 = null;
        PropertyName? nameWrapper4 = PropertyName.New(null);
        PropertyName? nameWrapper5 = PropertyName.New(nul);
        PropertyName? nameWrapper6 = PropertyName.New(empty);
        Assert.False(nameWrapper1.IsNotNullOrEmpty());
        Assert.False(nameWrapper2.IsNotNullOrEmpty());
        Assert.False(nameWrapper3.IsNotNullOrEmpty());
        Assert.False(nameWrapper4.IsNotNullOrEmpty());
        Assert.False(nameWrapper5.IsNotNullOrEmpty());
        Assert.False(nameWrapper6.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsNotEmpty_True()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(white);
        Assert.True(nameWrapper1.IsNotEmpty());
        Assert.True(nameWrapper2.IsNotEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_True()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(white);
        PropertyName? nameWrapper3 = PropertyName.New(eStr);
        PropertyName? nameWrapper4 = PropertyName.New(white);
        Assert.True(nameWrapper1.IsNotNullOrEmpty());
        Assert.True(nameWrapper2.IsNotNullOrEmpty());
        Assert.True(nameWrapper3.IsNotNullOrEmpty());
        Assert.True(nameWrapper4.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsWhitespace_True()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        PropertyName nameWrapper3 = new(white);
        Assert.True(nameWrapper1.IsWhiteSpace());
        Assert.True(nameWrapper2.IsWhiteSpace());
        Assert.True(nameWrapper3.IsWhiteSpace());
    }

    [Fact]
    public void IsNullOrWhitespace_True()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        PropertyName nameWrapper3 = new(white);
        PropertyName? nameWrapper4 = null;
        PropertyName? nameWrapper5 = PropertyName.New(null);
        PropertyName? nameWrapper6 = PropertyName.New(nul);
        PropertyName? nameWrapper7 = PropertyName.New(empty);
        PropertyName? nameWrapper8 = PropertyName.New(white);
        Assert.True(nameWrapper1.IsNullOrWhiteSpace());
        Assert.True(nameWrapper2.IsNullOrWhiteSpace());
        Assert.True(nameWrapper3.IsNullOrWhiteSpace());
        Assert.True(nameWrapper4.IsNullOrWhiteSpace());
        Assert.True(nameWrapper5.IsNullOrWhiteSpace());
        Assert.True(nameWrapper6.IsNullOrWhiteSpace());
        Assert.True(nameWrapper7.IsNullOrWhiteSpace());
        Assert.True(nameWrapper8.IsNullOrWhiteSpace());
    }

    [Fact]
    public void IsWhitespace_False()
    {
        PropertyName nameWrapper1 = new(eStr);
        Assert.False(nameWrapper1.IsWhiteSpace());
    }

    [Fact]
    public void IsNullOrWhitespace_False()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName? nameWrapper2 = PropertyName.New(eStr);
        Assert.False(nameWrapper1.IsNullOrWhiteSpace());
        Assert.False(nameWrapper2.IsNullOrWhiteSpace());
    }

    [Fact]
    public void IsNotWhitespace_False()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        PropertyName nameWrapper3 = new(white);
        Assert.False(nameWrapper1.IsNotWhiteSpace());
        Assert.False(nameWrapper2.IsNotWhiteSpace());
        Assert.False(nameWrapper3.IsNotWhiteSpace());
    }

    [Fact]
    public void IsNotNullOrWhitespace_False()
    {
        PropertyName nameWrapper1 = new(empty);
        PropertyName nameWrapper2 = new(nul);
        PropertyName nameWrapper3 = new(white);
        PropertyName? nameWrapper4 = null;
        PropertyName? nameWrapper5 = PropertyName.New(empty);
        PropertyName? nameWrapper6 = PropertyName.New(nul);
        PropertyName? nameWrapper7 = PropertyName.New(white);
        Assert.False(nameWrapper1.IsNotNullOrWhiteSpace());
        Assert.False(nameWrapper2.IsNotNullOrWhiteSpace());
        Assert.False(nameWrapper3.IsNotNullOrWhiteSpace());
        Assert.False(nameWrapper4.IsNotNullOrWhiteSpace());
        Assert.False(nameWrapper5.IsNotNullOrWhiteSpace());
        Assert.False(nameWrapper6.IsNotNullOrWhiteSpace());
        Assert.False(nameWrapper7.IsNotNullOrWhiteSpace());
    }

    [Fact]
    public void IsNotWhitespace_True()
    {
        PropertyName nameWrapper1 = new(eStr);
        Assert.True(nameWrapper1.IsNotWhiteSpace());
    }

    [Fact]
    public void IsNotNullOrWhitespace_True()
    {
        PropertyName nameWrapper1 = new(eStr);
        Assert.True(nameWrapper1.IsNotNullOrWhiteSpace());
    }

    [Fact]
    public void ImplicitConversion_ToString_AssignmentAndInterpolation()
    {
        PropertyName a = new(eStr);
        string assigned = a;
        string interpolated = $"{a}";

        Assert.Equal(eStr, assigned);
        Assert.Equal(eStr, interpolated);
    }

    [Fact]
    public void GetHashCode_EqualObjects_SameHash()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new(eStr);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(nameWrapper1.GetHashCode(), nameWrapper2.GetHashCode());
    }

    [Fact]
    public void Equality_PreservesWhitespace()
    {
        PropertyName nameWrapper1 = new(eStr);
        PropertyName nameWrapper2 = new($" {eStr} ");
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.False(nameWrapper2.IsWhiteSpace()); 
    }

    [Fact]
    public void Equals_ObjectAndTyped_Agree()
    {
        PropertyName nameWrapper1 = new(eStr);
        object nameWrapper2 = new PropertyName(eStr);
        Assert.True(nameWrapper1.Equals((StringWrapper)nameWrapper2));
        Assert.True(nameWrapper1.Equals(nameWrapper2));
    }

    [Fact]
    public void New_White_PreservesValue()
    {
        PropertyName? nameWrapper = PropertyName.New(white);
        Assert.NotNull(nameWrapper);
        Assert.Equal(white, nameWrapper!.ToString());
    }



    [Fact]
    public void CompareTo_Null_Returns1()
    {
        PropertyName nameWrapper = new(eStr);

        Assert.Equal(1, nameWrapper.CompareTo(null));
    }

    [Fact]
    public void CompareTo_EqualObjects_Returns0()
    {
        PropertyName left = new("ABC");
        PropertyName right = new("ABC");

        Assert.Equal(0, left.CompareTo(right));
    }

    [Fact]
    public void CompareTo_OrdersByUnderlyingString()
    {
        PropertyName left = new("A");
        PropertyName right = new("B");

        Assert.True(left.CompareTo(right) < 0);
        Assert.True(right.CompareTo(left) > 0);
    }
}
