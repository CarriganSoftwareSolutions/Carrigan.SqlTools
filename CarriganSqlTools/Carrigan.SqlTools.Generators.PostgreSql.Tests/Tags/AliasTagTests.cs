using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Tags;

public class AliasTagTests
{
    [Fact]
    public void Constructor()
    {
        AliasName aliasName = new("Alias");
        AliasTag aliasTag = new(aliasName);

        Assert.NotNull(aliasTag);
        Assert.Equal("Alias", aliasTag.ToString());
    }

    [Fact]
    public void Constructor_Null_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new AliasTag(null!));

    [Fact]
    public void New_Null()
    {
        AliasTag? aliasTag = AliasTag.New(null);

        Assert.Null(aliasTag);
    }

    [Fact]
    public void New_Empty()
    {
        AliasTag? aliasTag = AliasTag.New(new AliasName(string.Empty));

        Assert.Null(aliasTag);
    }

    [Fact]
    public void New_Whitespace()
    {
        AliasTag? aliasTag = AliasTag.New(new AliasName(" "));

        Assert.NotNull(aliasTag);
        Assert.Equal(" ", aliasTag.ToString());
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Alias", "Alias")]
    [InlineData(" Alias ", " Alias ")]
    public void ToString_Value(string? value, string expected)
    {
        AliasTag aliasTag = new(new AliasName(value));

        Assert.Equal(expected, aliasTag.ToString());
        Assert.Equal(expected, $"{aliasTag}");
    }

    [Fact]
    public void Equals_SameReference()
    {
        AliasTag aliasTag = new(new AliasName("Alias"));

        Assert.True(aliasTag.Equals(aliasTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(aliasTag == aliasTag);
        Assert.False(aliasTag != aliasTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        AliasTag left = new(new AliasName("Alias"));
        AliasTag right = new(new AliasName("Alias"));

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        AliasTag first = new(new AliasName("Alias"));
        AliasTag second = new(new AliasName("Alias"));
        AliasTag third = new(new AliasName("Alias"));

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_NullAndEmptyAliasName()
    {
        AliasTag left = new(new AliasName(null));
        AliasTag right = new(new AliasName(string.Empty));

        Assert.True(left.Equals(right));
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValue()
    {
        AliasTag left = new(new AliasName("AliasOne"));
        AliasTag right = new(new AliasName("AliasTwo"));

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_CaseSensitive()
    {
        AliasTag left = new(new AliasName("Alias"));
        AliasTag right = new(new AliasName("alias"));

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_PreservesWhitespace()
    {
        AliasTag left = new(new AliasName("Alias"));
        AliasTag right = new(new AliasName(" Alias "));

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_Null()
    {
        AliasTag aliasTag = new(new AliasName("Alias"));
        AliasTag? other = null;

        Assert.False(aliasTag.Equals(other));
        Assert.False(aliasTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        AliasTag aliasTag = new(new AliasName("Alias"));
        object other = new AliasTag(new AliasName("Alias"));

        Assert.True(aliasTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        AliasTag aliasTag = new(new AliasName("Alias"));
        object other = new AliasName("Alias");

        Assert.False(aliasTag.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        AliasTag left = new(new AliasName("Alias"));
        AliasTag right = new(new AliasName("Alias"));

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        AliasTag left = new(new AliasName("AliasOne"));
        AliasTag right = new(new AliasName("AliasTwo"));

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        AliasTag aliasTag = new(new AliasName("Alias"));
        AliasTag? nullAliasTag = null;

        Assert.False(aliasTag == nullAliasTag);
        Assert.False(nullAliasTag == aliasTag);
        Assert.True(aliasTag != nullAliasTag);
        Assert.True(nullAliasTag != aliasTag);
        Assert.Null(nullAliasTag);
        Assert.NotNull(aliasTag);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        AliasTag left = new(new AliasName("Alias"));
        AliasTag right = new(new AliasName("Alias"));

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        AliasTag storedKey = new(new AliasName("Alias"));
        AliasTag lookupKey = new(new AliasName("Alias"));
        Dictionary<AliasTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_NullAndEmptyAliasName()
    {
        AliasTag storedKey = new(new AliasName(null));
        AliasTag lookupKey = new(new AliasName(string.Empty));
        Dictionary<AliasTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentInstance()
    {
        AliasTag storedKey = new(new AliasName("AliasOne"));
        AliasTag lookupKey = new(new AliasName("AliasTwo"));
        Dictionary<AliasTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        AliasTag firstKey = new(new AliasName("Alias"));
        AliasTag secondKey = new(new AliasName("Alias"));
        Dictionary<AliasTag, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_CaseSensitive()
    {
        AliasTag storedKey = new(new AliasName("Alias"));
        AliasTag lookupKey = new(new AliasName("alias"));
        Dictionary<AliasTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }
}
