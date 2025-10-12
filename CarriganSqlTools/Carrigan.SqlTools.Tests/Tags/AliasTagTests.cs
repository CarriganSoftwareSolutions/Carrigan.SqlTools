using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.Tags;
public class AliasTagTests
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
        AliasName nameWrapper = new(nul);
        Assert.NotNull(nameWrapper);
        AliasTag tag = new(nameWrapper);
        Assert.NotNull(tag);
    }

    [Fact]
    public void New_Null()
    {
        AliasName? nameWrapper = AliasName.New(nul);
        Assert.Null(nameWrapper);
        AliasTag? tag = AliasTag.New(nameWrapper);
        Assert.Null(tag);
    }

    [Fact]
    public void New_White()
    {
        AliasName? nameWrapper = AliasName.New(white);
        Assert.NotNull(nameWrapper);
        AliasTag? tag = AliasTag.New(nameWrapper);
        Assert.NotNull(tag);
    }


    [Fact]
    public void ToString_Null()
    {
        AliasName nameWrapper = new(nul);
        AliasTag tag = new(nameWrapper);
        Assert.Equal(string.Empty, tag.ToString());
    }

    [Fact]
    public void ToString_Empty()
    {
        AliasName nameWrapper = new(empty);
        AliasTag tag = new(nameWrapper);
        Assert.Equal(string.Empty, tag.ToString());
    }

    [Fact]
    public void ToString_Text()
    {
        AliasName nameWrapper = new(eStr);
        AliasTag tag = new(nameWrapper);
        Assert.Equal(eStr, tag.ToString());
    }

    [Fact]
    public void Equal_Null()
    {
        string? name = null;

        AliasName nameWrapper1 = new(name);
        AliasName nameWrapper2 = new(name);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.Equal(tag1, tag2);
        Assert.Equal(tag2, tag1);
        Assert.Equal(string.Empty, tag1);
        Assert.Equal(string.Empty, tag2);
    }

    [Fact]
    public void Equal_Empty()
    {
        string? name = string.Empty;

        AliasName nameWrapper1 = new(name);
        AliasName nameWrapper2 = new(name);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.Equal(tag1, tag2);
        Assert.Equal(tag2, tag1);
        Assert.Equal(string.Empty, tag1);
        Assert.Equal(string.Empty, tag2);
    }

    [Fact]
    public void Equal_Default_Text()
    {
        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new(eStrAlt);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.Equal(tag1, tag2);
        Assert.Equal(tag2, tag1);
        Assert.Equal(eStr, tag1);
        Assert.Equal(eStr, tag2);
    }

    [Fact]
    public void EqualEqual_Null()
    {
        AliasName nameWrapper1 = new(nul);
        AliasName nameWrapper2 = new(nul);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.True(tag1 == tag2);
        Assert.True(tag2 == tag1);
        Assert.True(string.Empty == tag1);
        Assert.True(string.Empty == tag2);
    }

    [Fact]
    public void EqualEqual_Empty()
    {
        AliasName nameWrapper1 = new(empty);
        AliasName nameWrapper2 = new(empty);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.True(tag1 == tag2);
        Assert.True(tag2 == tag1);
        Assert.True(string.Empty == tag1);
        Assert.True(string.Empty == tag2);
    }

    [Fact]
    public void EqualEqual_Text()
    {
        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new(eStrAlt);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.True(tag1 == tag2);
        Assert.True(tag2 == tag1);

        bool test = eStr == tag1;
        bool test2 = eStr != tag1;
        bool test3 = eStr == tag1;
        bool test4 = eStr != tag2;

        Assert.True(tag1 == tag2);
        Assert.True(tag2 == tag1);
        Assert.True(test);
        Assert.True(test3);
        Assert.False(tag1 != nameWrapper2);
        Assert.False(nameWrapper2 != tag1);
        Assert.False(test2);
        Assert.False(test4);
    }

    [Fact]
    public void EqualEqual_NullComparisons()
    {
        AliasName nonNull = new(string.Empty);
        AliasTag tag1 = new(nonNull);
        AliasTag? tag2 = null;

        Assert.False(tag1 == tag2);
        Assert.False(tag2 == tag1);
    }

    [Fact]
    public void NotEqualEqual_NullComparisons()
    {
        AliasName nonNull = new(string.Empty);

        AliasTag tag1 = new(nonNull);
        AliasTag? tag2 = null;

        Assert.True(tag1 != tag2);
        Assert.True(tag2 != tag1);
    }

    [Fact]
    public void DictionaryKey_Null()
    {
        string? name = null;
        Dictionary<AliasTag, decimal> dictionary= [];

        AliasName nameWrapper1 = new(empty);
        AliasName nameWrapper2 = new(name);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        dictionary[tag1] = pi;
        Assert.True(dictionary.ContainsKey(tag2));
        Assert.True(dictionary[tag1] == pi);
        Assert.True(dictionary[tag1] == pi);
    }

    [Fact]
    public void DictionaryKey_Empty()
    {
        Dictionary<AliasTag, decimal> dictionary = [];

        AliasName nameWrapper1 = new(empty);
        AliasName nameWrapper2 = new(empty);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        dictionary[tag1] = pi;
        Assert.True(dictionary.ContainsKey(tag2));
        Assert.True(dictionary[tag1] == pi);
        Assert.True(dictionary[tag1] == pi);
    }

    [Fact]
    public void DictionaryKey_Text()
    {
        string? name = eStr;
        Dictionary<AliasTag, decimal> dictionary = [];

        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new(eStrAlt);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        dictionary[tag1] = pi;
        Assert.True(dictionary.ContainsKey(tag2));
        Assert.True(dictionary[tag1] == pi);
        Assert.True(dictionary[tag2] == pi);
    }

    [Fact]
    public void DictionaryKey_Multi()
    {
        string? name = eStr;
        Dictionary<AliasTag, decimal> dictionary = [];

        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new(eStrAlt);
        AliasName nameWrapper3 = new(tString);
        AliasName nameWrapper4 = new(goldenString);
        AliasName nameWrapper5 = new(piString);
        AliasName nameWrapper6 = new(empty);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        AliasTag tag3 = new(nameWrapper3);
        AliasTag tag4 = new(nameWrapper4);
        AliasTag tag5 = new(nameWrapper5);
        AliasTag tag6 = new(nameWrapper6);
        dictionary[tag1] = e;
        dictionary[tag3] = t;
        dictionary[tag4] = pi;
        dictionary[tag5] = golden;
        dictionary[tag6] = zero;
        Assert.True(dictionary.ContainsKey(tag2));
        Assert.True(dictionary[tag1] == e);
        Assert.True(dictionary[tag2] == e);
        Assert.True(dictionary[tag3] == t);
        Assert.True(dictionary[tag4] == pi);
        Assert.True(dictionary[tag5] == golden);
        Assert.True(dictionary[tag6] == zero);
        Assert.Equal(5, dictionary.Count);
    }



    [Fact]
    public void NotEqual_Null()
    {
        AliasName nameWrapper1 = new(nul);
        AliasName nameWrapper2 = new(goldenString);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.NotEqual(tag1, tag2);
        Assert.NotEqual(empty, tag2);
    }

    [Fact]
    public void NotEqual_Empty()
    {
        AliasName nameWrapper1 = new(empty);
        AliasName nameWrapper2 = new(goldenString);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.NotEqual(tag1, tag2);
        Assert.NotEqual(empty, tag2);
    }

    [Fact]
    public void NotEqual_Text()
    {
        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new(goldenString);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.NotEqual(tag1, tag2);
        Assert.NotEqual(eStr, tag2);
    }

    [Fact]
    public void NotEqualEqual_Null()
    {
        AliasName nameWrapper1 = new(nul);
        AliasName nameWrapper2 = new(eStr);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.True(tag1 != tag2);
        Assert.True(empty != tag2);
    }

    [Fact]
    public void NotEqualEqual_Empty()
    {
        AliasName nameWrapper1 = new(empty);
        AliasName nameWrapper2 = new(eStr);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.True(tag1 != tag2);
        Assert.True(empty != tag2);
    }

    [Fact]
    public void NotEqualEqual_Text()
    {
        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new(goldenString);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.True(tag1 != tag2);
        Assert.True(tag1 != tag2);
    }

    [Fact]
    public void ImplicitConversion_ToString_AssignmentAndInterpolation()
    {
        AliasName a = new(eStr);
        AliasTag aTag = new(a);
        string assigned = aTag;
        string interpolated = $"{aTag}";

        Assert.Equal(eStr, assigned);
        Assert.Equal(eStr, interpolated);
    }

    [Fact]
    public void GetHashCode_EqualObjects_SameHash()
    {
        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new(eStr);
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.Equal(tag1, tag2);
        Assert.Equal(tag1.GetHashCode(), tag2.GetHashCode());
    }

    [Fact]
    public void Equality_PreservesWhitespace()
    {
        AliasName nameWrapper1 = new(eStr);
        AliasName nameWrapper2 = new($" {eStr} ");
        AliasTag tag1 = new(nameWrapper1);
        AliasTag tag2 = new(nameWrapper2);
        Assert.NotEqual(tag1, tag2);
        Assert.True(tag1 != tag2);
    }
}
