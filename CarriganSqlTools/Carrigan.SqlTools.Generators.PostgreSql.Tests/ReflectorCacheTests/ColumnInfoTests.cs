using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ReflectorCacheTests;

public class ColumnInfoTests
{
    private static ColumnInfo CreateColumnInfo(string propertyName, string? schemaName = "dbo", string tableName = "ColumnIdentifiersTable")
    {
        Type type = typeof(ColumnIdentifiers);
        IEnumerable<PropertyInfo> keys = [type.GetProperty("Id")!];
        PropertyInfo property = type.GetProperty(propertyName)!;

        return new(SchemaName.New(schemaName), new TableName(tableName), property, keys);
    }

    [Fact]
    public void CompareTo_EquivalentInstances()
    {
        ColumnInfo left = CreateColumnInfo("Id");
        ColumnInfo right = CreateColumnInfo("Id");

        Assert.Equal(0, left.CompareTo(right));
        Assert.Equal(0, right.CompareTo(left));
    }

    [Fact]
    public void CompareTo_DifferentColumns()
    {
        ColumnInfo left = CreateColumnInfo("Id");
        ColumnInfo right = CreateColumnInfo("Property");

        Assert.NotEqual(0, left.CompareTo(right));
        Assert.NotEqual(0, right.CompareTo(left));
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        ColumnInfo left = CreateColumnInfo("Id");
        ColumnInfo right = CreateColumnInfo("Id");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
    }

    [Fact]
    public void Equals_IsCaseInsensitiveAcrossColumnTagComponents()
    {
        ColumnInfo left = CreateColumnInfo("Id", "Schema", "Table");
        ColumnInfo right = CreateColumnInfo("Id", "schema", "table");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
    }

    [Fact]
    public void Equals_DifferentColumn()
    {
        ColumnInfo left = CreateColumnInfo("Id");
        ColumnInfo right = CreateColumnInfo("Property");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_UsesColumnTagStructuralIdentity_NotFormattedText()
    {
        ColumnInfo left = CreateColumnInfo("Id", "A.B", "C");
        ColumnInfo right = CreateColumnInfo("Id", "A", "B.C");

        Assert.Equal(left.ToString(), right.ToString());
        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_Null()
    {
        ColumnInfo columnInfo = CreateColumnInfo("Id");
        ColumnInfo? other = null;

        Assert.False(columnInfo.Equals(other));
        Assert.False(columnInfo.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        ColumnInfo columnInfo = CreateColumnInfo("Id");
        object other = CreateColumnInfo("Id");

        Assert.True(columnInfo.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        ColumnInfo columnInfo = CreateColumnInfo("Id");
        object other = new ColumnName("Id");

        Assert.False(columnInfo.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        ColumnInfo left = CreateColumnInfo("Id");
        ColumnInfo right = CreateColumnInfo("Id");

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        ColumnInfo left = CreateColumnInfo("Id");
        ColumnInfo right = CreateColumnInfo("Property");

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        ColumnInfo columnInfo = CreateColumnInfo("Id");
        ColumnInfo? nullColumnInfo = null;
        bool flag;

        Assert.False(columnInfo == nullColumnInfo);
        Assert.False(nullColumnInfo == columnInfo);
        Assert.True(columnInfo != nullColumnInfo);
        Assert.True(nullColumnInfo != columnInfo);
        flag = nullColumnInfo == null;
        Assert.True(flag);
        flag = nullColumnInfo != null;
        Assert.False(flag);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        ColumnInfo left = CreateColumnInfo("Id");
        ColumnInfo right = CreateColumnInfo("Id");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void GetHashCode_CaseInsensitiveEquivalentInstances()
    {
        ColumnInfo left = CreateColumnInfo("Id", "Schema", "Table");
        ColumnInfo right = CreateColumnInfo("Id", "schema", "table");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        ColumnInfo storedKey = CreateColumnInfo("Id");
        ColumnInfo lookupKey = CreateColumnInfo("Id");
        Dictionary<ColumnInfo, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_CaseInsensitiveEquivalentInstance()
    {
        ColumnInfo storedKey = CreateColumnInfo("Id", "Schema", "Table");
        ColumnInfo lookupKey = CreateColumnInfo("Id", "schema", "table");
        Dictionary<ColumnInfo, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentColumn()
    {
        ColumnInfo storedKey = CreateColumnInfo("Id");
        ColumnInfo lookupKey = CreateColumnInfo("Property");
        Dictionary<ColumnInfo, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        ColumnInfo firstKey = CreateColumnInfo("Id", "Schema", "Table");
        ColumnInfo secondKey = CreateColumnInfo("Id", "schema", "table");
        Dictionary<ColumnInfo, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void HashSet_EquivalentInstance()
    {
        ColumnInfo storedValue = CreateColumnInfo("Id", "Schema", "Table");
        ColumnInfo lookupValue = CreateColumnInfo("Id", "schema", "table");
        HashSet<ColumnInfo> set = [storedValue];

        Assert.Contains(lookupValue, set);
    }
}
