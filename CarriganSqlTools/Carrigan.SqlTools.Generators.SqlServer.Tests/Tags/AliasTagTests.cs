using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.Tags;

public class AliasTagTests
{
    [Fact]
    public void Constructor()
    {
        ColumnAlias aliasName = new("Alias");
        ColumnTag aliasTag = new(aliasName);

        Assert.NotNull(aliasTag);
        Assert.Equal("Alias", aliasTag.ToString());
    }

    [Fact]
    public void Constructor_Null_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new ColumnTag(null!));

    [Fact]
    public void New_Null()
    {
        ColumnTag? aliasTag = ColumnTag.New(null);

        Assert.Null(aliasTag);
    }

    [Fact]
    public void New_Empty()
    {
        ColumnTag? aliasTag = ColumnTag.New(new ColumnAlias(string.Empty));

        Assert.Null(aliasTag);
    }

    [Fact]
    public void New_Whitespace()
    {
        ColumnTag? aliasTag = ColumnTag.New(new ColumnAlias(" "));

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
        ColumnTag aliasTag = new(new ColumnAlias(value));

        Assert.Equal(expected, aliasTag.ToString());
        Assert.Equal(expected, $"{aliasTag}");
    }

    [Fact]
    public void Equals_SameReference()
    {
        ColumnTag aliasTag = new(new ColumnAlias("Alias"));

        Assert.True(aliasTag.Equals(aliasTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(aliasTag == aliasTag);
        Assert.False(aliasTag != aliasTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        ColumnTag left = new(new ColumnAlias("Alias"));
        ColumnTag right = new(new ColumnAlias("Alias"));

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        ColumnTag first = new(new ColumnAlias("Alias"));
        ColumnTag second = new(new ColumnAlias("Alias"));
        ColumnTag third = new(new ColumnAlias("Alias"));

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_NullAndEmptyAliasName()
    {
        ColumnTag left = new(new ColumnAlias(null));
        ColumnTag right = new(new ColumnAlias(string.Empty));

        Assert.True(left.Equals(right));
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValue()
    {
        ColumnTag left = new(new ColumnAlias("AliasOne"));
        ColumnTag right = new(new ColumnAlias("AliasTwo"));

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_CaseSensitive()
    {
        ColumnTag left = new(new ColumnAlias("Alias"));
        ColumnTag right = new(new ColumnAlias("alias"));

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_PreservesWhitespace()
    {
        ColumnTag left = new(new ColumnAlias("Alias"));
        ColumnTag right = new(new ColumnAlias(" Alias "));

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_Null()
    {
        ColumnTag aliasTag = new(new ColumnAlias("Alias"));
        ColumnTag? other = null;

        Assert.False(aliasTag.Equals(other));
        Assert.False(aliasTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        ColumnTag aliasTag = new(new ColumnAlias("Alias"));
        object other = new ColumnTag(new ColumnAlias("Alias"));

        Assert.True(aliasTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        ColumnTag aliasTag = new(new ColumnAlias("Alias"));
        object other = new ColumnAlias("Alias");

        Assert.False(aliasTag.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        ColumnTag left = new(new ColumnAlias("Alias"));
        ColumnTag right = new(new ColumnAlias("Alias"));

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        ColumnTag left = new(new ColumnAlias("AliasOne"));
        ColumnTag right = new(new ColumnAlias("AliasTwo"));

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        ColumnTag aliasTag = new(new ColumnAlias("Alias"));
        ColumnTag? nullAliasTag = null;
        bool nullIsNull;
        bool nullIsNotNull;

        Assert.False(aliasTag == nullAliasTag);
        Assert.False(nullAliasTag == aliasTag);
        Assert.True(aliasTag != nullAliasTag);
        Assert.True(nullAliasTag != aliasTag);
        nullIsNull = nullAliasTag == null;
        nullIsNotNull = nullAliasTag != null;
        Assert.True(nullIsNull);
        Assert.False(nullIsNotNull);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        ColumnTag left = new(new ColumnAlias("Alias"));
        ColumnTag right = new(new ColumnAlias("Alias"));

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        ColumnTag storedKey = new(new ColumnAlias("Alias"));
        ColumnTag lookupKey = new(new ColumnAlias("Alias"));
        Dictionary<ColumnTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_NullAndEmptyAliasName()
    {
        ColumnTag storedKey = new(new ColumnAlias(null));
        ColumnTag lookupKey = new(new ColumnAlias(string.Empty));
        Dictionary<ColumnTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentInstance()
    {
        ColumnTag storedKey = new(new ColumnAlias("AliasOne"));
        ColumnTag lookupKey = new(new ColumnAlias("AliasTwo"));
        Dictionary<ColumnTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        ColumnTag firstKey = new(new ColumnAlias("Alias"));
        ColumnTag secondKey = new(new ColumnAlias("Alias"));
        Dictionary<ColumnTag, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_CaseSensitive()
    {
        ColumnTag storedKey = new(new ColumnAlias("Alias"));
        ColumnTag lookupKey = new(new ColumnAlias("alias"));
        Dictionary<ColumnTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }
}
