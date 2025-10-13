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
public class ProcedureNameTests
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
        ProcedureName nameWrapper = new (nul);
        Assert.NotNull(nameWrapper);
    }

    [Fact]
    public void New_Null()
    {
        ProcedureName? nameWrapper = ProcedureName.New(nul);
        Assert.Null(nameWrapper);
    }

    [Fact]
    public void New_Empty()
    {
        ProcedureName? nameWrapper = ProcedureName.New(empty);
        Assert.Null(nameWrapper);
    }

    [Fact]
    public void New_White()
    {
        ProcedureName? nameWrapper = ProcedureName.New(white);
        Assert.NotNull(nameWrapper);
    }


    [Fact]
    public void ToString_Null()
    {
        ProcedureName nameWrapper = new(nul);
        Assert.Equal(string.Empty, nameWrapper.ToString());
    }

    [Fact]
    public void ToString_Empty()
    {
        ProcedureName nameWrapper = new(empty);
        Assert.Equal(empty, nameWrapper.ToString());
    }

    [Fact]
    public void ToString_Text()
    {
        ProcedureName nameWrapper = new(eStr);
        Assert.Equal(eStr, nameWrapper.ToString());
    }

    [Fact]
    public void Equal_Null()
    {
        string? name = null;

        ProcedureName nameWrapper1 = new(name);
        ProcedureName nameWrapper2 = new(name);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(string.Empty, nameWrapper2);
    }

    [Fact]
    public void Equal_Empty()
    {
        string? name = string.Empty;

        ProcedureName nameWrapper1 = new(name);
        ProcedureName nameWrapper2 = new(name);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(string.Empty, nameWrapper2);
    }

    [Fact]
    public void Equal_Default_Text()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(eStrAlt);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(eStr, nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Null()
    {
        ProcedureName nameWrapper1 = new(nul);
        ProcedureName nameWrapper2 = new(nul);
        Assert.True(nameWrapper1 == nameWrapper2);
        Assert.True(string.Empty == nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Empty()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(empty);
        Assert.True(nameWrapper1 == nameWrapper2);
        Assert.True(string.Empty == nameWrapper2);
    }

    [Fact]
    public void EqualEqual_Text()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(eStrAlt);
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
        ProcedureName nonNull = new(string.Empty);
        ProcedureName? isNull = null;

        Assert.False(nonNull == isNull);
        Assert.False(isNull == nonNull);
    }

    [Fact]
    public void NotEqualEqual_NullComparisons()
    {
        ProcedureName nonNull = new(string.Empty);
        ProcedureName? isNull = null;
        Assert.True(nonNull != isNull);
        Assert.True(isNull != nonNull);
    }

    [Fact]
    public void DictionaryKey_Null()
    {
        string? name = null;
        Dictionary<ProcedureName, decimal> dictionary= [];

        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(name);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Empty()
    {
        Dictionary<ProcedureName, decimal> dictionary = [];

        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(empty);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Text()
    {
        string? name = eStr;
        Dictionary<ProcedureName, decimal> dictionary = [];

        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(eStrAlt);
        dictionary[nameWrapper1] = pi;
        Assert.True(dictionary.ContainsKey(nameWrapper2));
        Assert.True(dictionary[nameWrapper1] == pi);
        Assert.True(dictionary[nameWrapper2] == pi);
    }

    [Fact]
    public void DictionaryKey_Multi()
    {
        string? name = eStr;
        Dictionary<ProcedureName, decimal> dictionary = [];

        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(eStrAlt);
        ProcedureName nameWrapper3 = new(tString);
        ProcedureName nameWrapper4 = new(goldenString);
        ProcedureName nameWrapper5 = new(piString);
        ProcedureName nameWrapper6 = new(empty);
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
        ProcedureName nameWrapper1 = new(nul);
        ProcedureName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(empty, nameWrapper2);
    }

    [Fact]
    public void NotEqual_Empty()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(empty, nameWrapper2);
    }

    [Fact]
    public void NotEqual_Text()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(goldenString);
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.NotEqual(eStr, nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Null()
    {
        ProcedureName nameWrapper1 = new(nul);
        ProcedureName nameWrapper2 = new(eStr);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(empty != nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Empty()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(eStr);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(empty != nameWrapper2);
    }

    [Fact]
    public void NotEqualEqual_Text()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(goldenString);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.True(nameWrapper1 != nameWrapper2);
    }

    [Fact]
    public void IsEmpty_True()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        Assert.True(nameWrapper1.IsEmpty());
        Assert.True(nameWrapper2.IsEmpty());
    }
    [Fact]
    public void IsNullOrEmpty_True()
    {
        ProcedureName? nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        ProcedureName? nameWrapper3 = null;
        ProcedureName? nameWrapper4 = ProcedureName.New(nul);
        ProcedureName? nameWrapper5 = ProcedureName.New(empty);
        Assert.True(nameWrapper1.IsNullOrEmpty());
        Assert.True(nameWrapper2.IsNullOrEmpty());
        Assert.True(nameWrapper3.IsNullOrEmpty());
        Assert.True(nameWrapper4.IsNullOrEmpty());
        Assert.True(nameWrapper5.IsNullOrEmpty());
    }

    [Fact]
    public void IsEmpty_False()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(white);
        Assert.False(nameWrapper1.IsEmpty());
        Assert.False(nameWrapper2.IsEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_False()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(white);
        ProcedureName? nameWrapper3 = ProcedureName.New(eStr);
        ProcedureName? nameWrapper4 = ProcedureName.New(white);
        Assert.False(nameWrapper1.IsNullOrEmpty());
        Assert.False(nameWrapper2.IsNullOrEmpty());
        Assert.False(nameWrapper3.IsNullOrEmpty());
        Assert.False(nameWrapper4.IsNullOrEmpty());
    }

    [Fact]
    public void IsNotEmpty_False()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        Assert.False(nameWrapper1.IsNotEmpty());
        Assert.False(nameWrapper2.IsNotEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_False()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        ProcedureName? nameWrapper3 = null;
        ProcedureName? nameWrapper4 = ProcedureName.New(null);
        ProcedureName? nameWrapper5 = ProcedureName.New(nul);
        ProcedureName? nameWrapper6 = ProcedureName.New(empty);
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
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(white);
        Assert.True(nameWrapper1.IsNotEmpty());
        Assert.True(nameWrapper2.IsNotEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_True()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(white);
        ProcedureName? nameWrapper3 = ProcedureName.New(eStr);
        ProcedureName? nameWrapper4 = ProcedureName.New(white);
        Assert.True(nameWrapper1.IsNotNullOrEmpty());
        Assert.True(nameWrapper2.IsNotNullOrEmpty());
        Assert.True(nameWrapper3.IsNotNullOrEmpty());
        Assert.True(nameWrapper4.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsWhitespace_True()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        ProcedureName nameWrapper3 = new(white);
        Assert.True(nameWrapper1.IsWhiteSpace());
        Assert.True(nameWrapper2.IsWhiteSpace());
        Assert.True(nameWrapper3.IsWhiteSpace());
    }

    [Fact]
    public void IsNullOrWhitespace_True()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        ProcedureName nameWrapper3 = new(white);
        ProcedureName? nameWrapper4 = null;
        ProcedureName? nameWrapper5 = ProcedureName.New(null);
        ProcedureName? nameWrapper6 = ProcedureName.New(nul);
        ProcedureName? nameWrapper7 = ProcedureName.New(empty);
        ProcedureName? nameWrapper8 = ProcedureName.New(white);
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
        ProcedureName nameWrapper1 = new(eStr);
        Assert.False(nameWrapper1.IsWhiteSpace());
    }

    [Fact]
    public void IsNullOrWhitespace_False()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName? nameWrapper2 = ProcedureName.New(eStr);
        Assert.False(nameWrapper1.IsNullOrWhiteSpace());
        Assert.False(nameWrapper2.IsNullOrWhiteSpace());
    }

    [Fact]
    public void IsNotWhitespace_False()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        ProcedureName nameWrapper3 = new(white);
        Assert.False(nameWrapper1.IsNotWhiteSpace());
        Assert.False(nameWrapper2.IsNotWhiteSpace());
        Assert.False(nameWrapper3.IsNotWhiteSpace());
    }

    [Fact]
    public void IsNotNullOrWhitespace_False()
    {
        ProcedureName nameWrapper1 = new(empty);
        ProcedureName nameWrapper2 = new(nul);
        ProcedureName nameWrapper3 = new(white);
        ProcedureName? nameWrapper4 = null;
        ProcedureName? nameWrapper5 = ProcedureName.New(empty);
        ProcedureName? nameWrapper6 = ProcedureName.New(nul);
        ProcedureName? nameWrapper7 = ProcedureName.New(white);
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
        ProcedureName nameWrapper1 = new(eStr);
        Assert.True(nameWrapper1.IsNotWhiteSpace());
    }

    [Fact]
    public void IsNotNullOrWhitespace_True()
    {
        ProcedureName nameWrapper1 = new(eStr);
        Assert.True(nameWrapper1.IsNotNullOrWhiteSpace());
    }

    [Fact]
    public void ImplicitConversion_ToString_AssignmentAndInterpolation()
    {
        ProcedureName a = new(eStr);
        string assigned = eStr;
        string interpolated = $"{eStr}";

        Assert.Equal(eStr, assigned);
        Assert.Equal(eStr, interpolated);
    }

    [Fact]
    public void GetHashCode_EqualObjects_SameHash()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new(eStr);
        Assert.Equal(nameWrapper1, nameWrapper2);
        Assert.Equal(nameWrapper1.GetHashCode(), nameWrapper2.GetHashCode());
    }

    [Fact]
    public void Equality_PreservesWhitespace()
    {
        ProcedureName nameWrapper1 = new(eStr);
        ProcedureName nameWrapper2 = new($" {eStr} ");
        Assert.NotEqual(nameWrapper1, nameWrapper2);
        Assert.True(nameWrapper1 != nameWrapper2);
        Assert.False(nameWrapper2.IsWhiteSpace()); 
    }

    [Fact]
    public void Equals_ObjectAndTyped_Agree()
    {
        ProcedureName nameWrapper1 = new(eStr);
        object nameWrapper2 = new ProcedureName(eStr);
        Assert.True(nameWrapper1.Equals((StringWrapper)nameWrapper2));
        Assert.True(nameWrapper1.Equals(nameWrapper2));
    }
}
