using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.Tags;

public class ColumnAliasTests
{
    [Fact]
    public void ToSql_QuotesColumnAliasForDialect()
    {
        ColumnAlias alias = new("Projected");

        Assert.Equal("[Projected]", alias.ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void Constructor()
    {
        ColumnAlias aliasName = new("Alias");
        ColumnAlias aliasTag = new(aliasName.ToString());

        Assert.NotNull(aliasTag);
        Assert.Equal("Alias", aliasTag.ToString());
    }

    [Fact]
    public void Constructor_Null_CreatesEmptyAlias() =>
        Assert.Equal(string.Empty, new ColumnAlias(null).ToString());

    [Fact]
    public void New_Null()
    {
        ColumnAlias? aliasTag = ColumnAlias.New(null);

        Assert.Null(aliasTag);
    }

    [Fact]
    public void New_Empty()
    {
        ColumnAlias? aliasTag = ColumnAlias.New(string.Empty);

        Assert.Null(aliasTag);
    }

    [Fact]
    public void New_Whitespace()
    {
        ColumnAlias? aliasTag = ColumnAlias.New(" ");

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
        ColumnAlias aliasTag = new(value);

        Assert.Equal(expected, aliasTag.ToString());
        Assert.Equal(expected, $"{aliasTag}");
    }

    [Fact]
    public void Equals_SameReference()
    {
        ColumnAlias aliasTag = new("Alias");

        Assert.True(aliasTag.Equals(aliasTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(aliasTag == aliasTag);
        Assert.False(aliasTag != aliasTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        ColumnAlias left = new("Alias");
        ColumnAlias right = new("Alias");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        ColumnAlias first = new("Alias");
        ColumnAlias second = new("Alias");
        ColumnAlias third = new("Alias");

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_NullAndEmptyAliasName()
    {
        ColumnAlias left = new(null);
        ColumnAlias right = new(string.Empty);

        Assert.True(left.Equals(right));
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValue()
    {
        ColumnAlias left = new("AliasOne");
        ColumnAlias right = new("AliasTwo");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_CaseInsensitive()
    {
        ColumnAlias left = new("Alias");
        ColumnAlias right = new("alias");

        Assert.True(left.Equals(right));
        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_PreservesWhitespace()
    {
        ColumnAlias left = new("Alias");
        ColumnAlias right = new(" Alias ");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_Null()
    {
        ColumnAlias aliasTag = new("Alias");
        ColumnAlias? other = null;

        Assert.False(aliasTag.Equals(other));
        Assert.False(aliasTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        ColumnAlias aliasTag = new("Alias");
        object other = new ColumnAlias("Alias");

        Assert.True(aliasTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        ColumnAlias aliasTag = new("Alias");
        object other = new object();

        Assert.False(aliasTag.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        ColumnAlias left = new("Alias");
        ColumnAlias right = new("Alias");

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        ColumnAlias left = new("AliasOne");
        ColumnAlias right = new("AliasTwo");

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        ColumnAlias aliasTag = new("Alias");
        ColumnAlias? nullAliasTag = null;
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
        ColumnAlias left = new("Alias");
        ColumnAlias right = new("Alias");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        ColumnAlias storedKey = new("Alias");
        ColumnAlias lookupKey = new("Alias");
        Dictionary<ColumnAlias, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_NullAndEmptyAliasName()
    {
        ColumnAlias storedKey = new(null);
        ColumnAlias lookupKey = new(string.Empty);
        Dictionary<ColumnAlias, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentInstance()
    {
        ColumnAlias storedKey = new("AliasOne");
        ColumnAlias lookupKey = new("AliasTwo");
        Dictionary<ColumnAlias, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        ColumnAlias firstKey = new("Alias");
        ColumnAlias secondKey = new("Alias");
        Dictionary<ColumnAlias, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_CaseInsensitive()
    {
        ColumnAlias storedKey = new("Alias");
        ColumnAlias lookupKey = new("alias");
        Dictionary<ColumnAlias, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }
}
