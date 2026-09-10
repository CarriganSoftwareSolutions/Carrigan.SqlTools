using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: somecolumn somealias
namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Tags;

public class SelectTagBaseEqualityTests
{
    private static SelectTag<SomeTable> New(string columnName, string? aliasName = null) =>
        new (new PropertyName(columnName), AliasName.New(aliasName));

    private static SelectTag NewExpression(string columnName, string? aliasName = null)
    {
        ColumnTag columnTag = new(new TableTag(null, "SomeTable"), new ColumnName(columnName));
        ColumnTagExpression expression = new(columnTag);
        return new (expression, AliasTag.New(AliasName.New(aliasName)));
    }

    [Fact]
    public void Equals_SameReference()
    {
        SelectTagBase selectTag = New("SomeColumn", "SomeAlias");

        Assert.True(selectTag.Equals(selectTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(selectTag == selectTag);
        Assert.False(selectTag != selectTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        SelectTagBase left = New("SomeColumn", "SomeAlias");
        SelectTagBase right = New("SomeColumn", "SomeAlias");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        SelectTagBase first = New("SomeColumn", "SomeAlias");
        SelectTagBase second = New("SomeColumn", "SomeAlias");
        SelectTagBase third = New("SomeColumn", "SomeAlias");

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_DifferentExpression()
    {
        SelectTagBase left = New("SomeColumn", "SomeAlias");
        SelectTagBase right = New("OtherColumn", "SomeAlias");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_DifferentAlias()
    {
        SelectTagBase left = New("SomeColumn", "SomeAlias");
        SelectTagBase right = New("SomeColumn", "OtherAlias");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_AliasPresenceMatters()
    {
        SelectTagBase left = New("SomeColumn");
        SelectTagBase right = New("SomeColumn", "SomeAlias");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_ExpressionUsesColumnTagCaseInsensitiveIdentity()
    {
        SelectTagBase left = NewExpression("SomeColumn", "SomeAlias");
        SelectTagBase right = NewExpression("somecolumn", "SomeAlias");

        Assert.True(left.Equals(right));
        Assert.True(left == right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_AliasIsCaseSensitive()
    {
        SelectTagBase left = New("SomeColumn", "SomeAlias");
        SelectTagBase right = New("SomeColumn", "somealias");

        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_StructurallyEquivalentExpressionAcrossConcreteSelectTagTypes()
    {
        SelectTagBase reflected = New("SomeColumn", "SomeAlias");
        SelectTagBase expression = NewExpression("SomeColumn", "SomeAlias");

        Assert.NotEqual(reflected.GetType(), expression.GetType());
        Assert.True(reflected.Equals(expression));
        Assert.True(expression.Equals(reflected));
        Assert.True(reflected == expression);
        Assert.Equal(reflected.GetHashCode(), expression.GetHashCode());
    }

    [Fact]
    public void Equals_Null()
    {
        SelectTagBase selectTag = New("SomeColumn", "SomeAlias");
        SelectTagBase? other = null;

        Assert.False(selectTag.Equals(other));
        Assert.False(selectTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        SelectTagBase selectTag = New("SomeColumn", "SomeAlias");
        object other = New("SomeColumn", "SomeAlias");

        Assert.True(selectTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        SelectTagBase selectTag = New("SomeColumn", "SomeAlias");
        object other = new AliasName("SomeAlias");

        Assert.False(selectTag.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        SelectTagBase left = New("SomeColumn", "SomeAlias");
        SelectTagBase right = New("SomeColumn", "SomeAlias");

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        SelectTagBase left = New("SomeColumn", "SomeAlias");
        SelectTagBase right = New("OtherColumn", "SomeAlias");

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        SelectTagBase selectTag = New("SomeColumn", "SomeAlias");
        SelectTagBase? nullSelectTag = null;
        SelectTagBase? secondNullSelectTag = null;

        Assert.False(selectTag == nullSelectTag);
        Assert.False(nullSelectTag == selectTag);
        Assert.True(selectTag != nullSelectTag);
        Assert.True(nullSelectTag != selectTag);
        Assert.True(nullSelectTag == secondNullSelectTag);
        Assert.False(nullSelectTag != secondNullSelectTag);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        SelectTagBase left = New("SomeColumn", "SomeAlias");
        SelectTagBase right = New("SomeColumn", "SomeAlias");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        SelectTagBase storedKey = New("SomeColumn", "SomeAlias");
        SelectTagBase lookupKey = New("SomeColumn", "SomeAlias");
        Dictionary<SelectTagBase, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentExpression()
    {
        SelectTagBase storedKey = New("SomeColumn", "SomeAlias");
        SelectTagBase lookupKey = New("OtherColumn", "SomeAlias");
        Dictionary<SelectTagBase, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_DifferentAlias()
    {
        SelectTagBase storedKey = New("SomeColumn", "SomeAlias");
        SelectTagBase lookupKey = New("SomeColumn", "OtherAlias");
        Dictionary<SelectTagBase, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        SelectTagBase firstKey = New("SomeColumn", "SomeAlias");
        SelectTagBase secondKey = New("SomeColumn", "SomeAlias");
        Dictionary<SelectTagBase, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_StructurallyEquivalentCrossConcreteType()
    {
        SelectTagBase storedKey = New("SomeColumn", "SomeAlias");
        SelectTagBase lookupKey = NewExpression("SomeColumn", "SomeAlias");
        Dictionary<SelectTagBase, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void HashSet_EquivalentInstance()
    {
        SelectTagBase storedValue = New("SomeColumn", "SomeAlias");
        SelectTagBase lookupValue = New("SomeColumn", "SomeAlias");
        HashSet<SelectTagBase> set = [storedValue];

        Assert.Contains(lookupValue, set);
    }

    [Fact]
    public void WithNoAlias_EqualsEquivalentUnaliasedTag()
    {
        SelectTagBase withAlias = New("SomeColumn", "SomeAlias");
        SelectTagBase expected = New("SomeColumn");

        Assert.Equal(expected, withAlias.WithNoAlias());
    }

    private class SomeTable
    {
        public int SomeColumn { get; set; }
        public int OtherColumn { get; set; }
    }
}
