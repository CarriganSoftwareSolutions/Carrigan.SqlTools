using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tests.IdentifierTypes;
public class ResultColumnNameTests
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
        ResultColumnName nameWrapper = new (nul);
        Assert.NotNull(nameWrapper);
    }

    [Fact]
    public void New_Null()
    {
        ResultColumnName? nameWrapper = ResultColumnName.New(nul);
        Assert.Null(nameWrapper);
    }

    [Fact]
    public void New_Empty()
    {
        ResultColumnName? nameWrapper = ResultColumnName.New(empty);
        Assert.Null(nameWrapper);
    }

    [Fact]
    public void New_White()
    {
        ResultColumnName? nameWrapper = ResultColumnName.New(white);
        Assert.NotNull(nameWrapper);
    }


    [Fact]
    public void ToString_Null()
    {
        ResultColumnName nameWrapper = new(nul);
        Assert.Equal(string.Empty, nameWrapper.ToString());
    }

    [Fact]
    public void ToString_Empty()
    {
        ResultColumnName nameWrapper = new(empty);
        Assert.Equal(empty, nameWrapper.ToString());
    }

    [Fact]
    public void ToString_Text()
    {
        ResultColumnName nameWrapper = new(eStr);
        Assert.Equal(eStr, nameWrapper.ToString());
    }

    [Fact]
    public void Equal_Null()
    {
        string? name = null;

        ResultColumnName nameWrapper1 = new(name);
        ResultColumnName nameWrapper2 = new(name);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(string.Empty, nameWrapper2);
    }

    [Fact]
    public void Equal_Empty()
    {
        string? name = string.Empty;

        ResultColumnName nameWrapper1 = new(name);
        ResultColumnName nameWrapper2 = new(name);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(string.Empty, nameWrapper2);
    }

    [Fact]
    public void Equal_Default_Text()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(eStrAlt);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(eStr, nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Null()
    {
        ResultColumnName nameWrapper1 = new(nul);
        ResultColumnName nameWrapper2 = new(nul);
        Assert.True(nameWrapper1 == nameWrapper2);
        Assert.True(string.Empty == nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Empty()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(empty);
        Assert.True(nameWrapper1 == nameWrapper2);
        Assert.True(string.Empty == nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Text()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(eStrAlt);
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
        ResultColumnName nonNull = new(string.Empty);
        ResultColumnName? isNull = null;

        Assert.False(nonNull == isNull);
        Assert.False(isNull == nonNull);
    }

    [Fact]
    public void NotEqualEqual_NullComparisons()
    {
        ResultColumnName nonNull = new(string.Empty);
        ResultColumnName? isNull = null;
        Assert.True(nonNull != isNull);
        Assert.True(isNull != nonNull);
    }

    [Fact]
    public void DictionaryKey_Null()
    {
        string? name = null;
        Dictionary<ResultColumnName, decimal> dictionary= [];

        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(name);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Empty()
    {
        Dictionary<ResultColumnName, decimal> dictionary = [];

        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(empty);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Text()
    {
        string? name = eStr;
        Dictionary<ResultColumnName, decimal> dictionary = [];

        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(eStrAlt);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Multi()
    {
        string? name = eStr;
        Dictionary<ResultColumnName, decimal> dictionary = [];

        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(eStrAlt);
        ResultColumnName nameWrapper3 = new(tString);
        ResultColumnName nameWrapper4 = new(goldenString);
        ResultColumnName nameWrapper5 = new(piString);
        ResultColumnName nameWrapper6 = new(empty);
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
        ResultColumnName nameWrapper1 = new(nul);
        ResultColumnName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(empty, nameWrapper2);
    }

    [Fact]
    public void NotEqual_Empty()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(empty, nameWrapper2);
    }

    [Fact]
    public void NotEqual_Text()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(eStr, nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Null()
    {
        ResultColumnName nameWrapper1 = new(nul);
        ResultColumnName nameWrapper2 = new(eStr);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(empty != nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Empty()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(eStr);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(empty != nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Text()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(goldenString);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(nameWrapper1 != nameWrapper2);
    }

    [Fact]
    public void IsEmpty_True()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        Assert.True(nameWrapper1.IsEmpty());
        Assert.True(nameWrapper2.IsEmpty());
    }
    [Fact]
    public void IsNullOrEmpty_True()
    {
        ResultColumnName? nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        ResultColumnName? nameWrapper3 = null;
        ResultColumnName? nameWrapper4 = ResultColumnName.New(nul);
        ResultColumnName? nameWrapper5 = ResultColumnName.New(empty);
        Assert.True(nameWrapper1.IsNullOrEmpty());
        Assert.True(nameWrapper2.IsNullOrEmpty());
        Assert.True(nameWrapper3.IsNullOrEmpty());
        Assert.True(nameWrapper4.IsNullOrEmpty());
        Assert.True(nameWrapper5.IsNullOrEmpty());
    }

    [Fact]
    public void IsEmpty_False()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(white);
        Assert.False(nameWrapper1.IsEmpty());
        Assert.False(nameWrapper2.IsEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_False()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(white);
        ResultColumnName? nameWrapper3 = ResultColumnName.New(eStr);
        ResultColumnName? nameWrapper4 = ResultColumnName.New(white);
        Assert.False(nameWrapper1.IsNullOrEmpty());
        Assert.False(nameWrapper2.IsNullOrEmpty());
        Assert.False(nameWrapper3.IsNullOrEmpty());
        Assert.False(nameWrapper4.IsNullOrEmpty());
    }

    [Fact]
    public void IsNotEmpty_False()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        Assert.False(nameWrapper1.IsNotEmpty());
        Assert.False(nameWrapper2.IsNotEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_False()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        ResultColumnName? nameWrapper3 = null;
        ResultColumnName? nameWrapper4 = ResultColumnName.New(null);
        ResultColumnName? nameWrapper5 = ResultColumnName.New(nul);
        ResultColumnName? nameWrapper6 = ResultColumnName.New(empty);
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
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(white);
        Assert.True(nameWrapper1.IsNotEmpty());
        Assert.True(nameWrapper2.IsNotEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_True()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(white);
        ResultColumnName? nameWrapper3 = ResultColumnName.New(eStr);
        ResultColumnName? nameWrapper4 = ResultColumnName.New(white);
        Assert.True(nameWrapper1.IsNotNullOrEmpty());
        Assert.True(nameWrapper2.IsNotNullOrEmpty());
        Assert.True(nameWrapper3.IsNotNullOrEmpty());
        Assert.True(nameWrapper4.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsWhitespace_True()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        ResultColumnName nameWrapper3 = new(white);
        Assert.True(nameWrapper1.IsWhiteSpace());
        Assert.True(nameWrapper2.IsWhiteSpace());
        Assert.True(nameWrapper3.IsWhiteSpace());
    }

    [Fact]
    public void IsNullOrWhitespace_True()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        ResultColumnName nameWrapper3 = new(white);
        ResultColumnName? nameWrapper4 = null;
        ResultColumnName? nameWrapper5 = ResultColumnName.New(null);
        ResultColumnName? nameWrapper6 = ResultColumnName.New(nul);
        ResultColumnName? nameWrapper7 = ResultColumnName.New(empty);
        ResultColumnName? nameWrapper8 = ResultColumnName.New(white);
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
        ResultColumnName nameWrapper1 = new(eStr);
        Assert.False(nameWrapper1.IsWhiteSpace());
    }

    [Fact]
    public void IsNullOrWhitespace_False()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName? nameWrapper2 = ResultColumnName.New(eStr);
        Assert.False(nameWrapper1.IsNullOrWhiteSpace());
        Assert.False(nameWrapper2.IsNullOrWhiteSpace());
    }

    [Fact]
    public void IsNotWhitespace_False()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        ResultColumnName nameWrapper3 = new(white);
        Assert.False(nameWrapper1.IsNotWhiteSpace());
        Assert.False(nameWrapper2.IsNotWhiteSpace());
        Assert.False(nameWrapper3.IsNotWhiteSpace());
    }

    [Fact]
    public void IsNotNullOrWhitespace_False()
    {
        ResultColumnName nameWrapper1 = new(empty);
        ResultColumnName nameWrapper2 = new(nul);
        ResultColumnName nameWrapper3 = new(white);
        ResultColumnName? nameWrapper4 = null;
        ResultColumnName? nameWrapper5 = ResultColumnName.New(empty);
        ResultColumnName? nameWrapper6 = ResultColumnName.New(nul);
        ResultColumnName? nameWrapper7 = ResultColumnName.New(white);
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
        ResultColumnName nameWrapper1 = new(eStr);
        Assert.True(nameWrapper1.IsNotWhiteSpace());
    }

    [Fact]
    public void IsNotNullOrWhitespace_True()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        Assert.True(nameWrapper1.IsNotNullOrWhiteSpace());
    }

    [Fact]
    public void ImplicitConversion_ToString_AssignmentAndInterpolation()
    {
        ResultColumnName a = new(eStr);
        string assigned = eStr;
        string interpolated = $"{eStr}";

        Assert.Equal(eStr, assigned);
        Assert.Equal(eStr, interpolated);
    }

    [Fact]
    public void GetHashCode_EqualObjects_SameHash()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new(eStr);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(nameWrapper1.GetHashCode(), nameWrapper2.GetHashCode());
    }

    [Fact]
    public void Equality_PreservesWhitespace()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        ResultColumnName nameWrapper2 = new($" {eStr} ");
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.False(nameWrapper2.IsWhiteSpace()); 
    }

    [Fact]
    public void Equals_ObjectAndTyped_Agree()
    {
        ResultColumnName nameWrapper1 = new(eStr);
        object nameWrapper2 = new ResultColumnName(eStr);
        Assert.True(nameWrapper1.Equals((StringWrapper)nameWrapper2));
        Assert.True(nameWrapper1.Equals(nameWrapper2));
    }
}
