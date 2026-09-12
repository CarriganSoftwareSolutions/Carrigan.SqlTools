using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.Tags;

public class TableTagTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Theory]
    [InlineData("Franks", "Pizza", "[Franks].[Pizza]")]
    [InlineData(null, "Pizza", "[Pizza]")]
    [InlineData("", "Pizza", "[Pizza]")]
    public void ToSql(string? schemaName, string tableName, string expected)
    {
        TableTag tableTag = new(schemaName, tableName);

        Assert.Equal(expected, tableTag.ToSql(Dialect));
    }

    [Theory]
    [InlineData("Franks", "Pizza", "Franks.Pizza")]
    [InlineData(null, "Pizza", "Pizza")]
    [InlineData("", "Pizza", "Pizza")]
    [InlineData(null, "", "")]
    [InlineData(null, " ", " ")]
    public void ToString_Value(string? schemaName, string tableName, string expected)
    {
        TableTag tableTag = new(schemaName, tableName);

        Assert.Equal(expected, tableTag.ToString());
        Assert.Equal(expected, $"{tableTag}");
    }

    [Fact]
    public void Equals_SameReference()
    {
        TableTag tableTag = new("Schema", "Table");

        Assert.True(tableTag.Equals(tableTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(tableTag == tableTag);
        Assert.False(tableTag != tableTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        TableTag left = new("Schema", "Table");
        TableTag right = new("Schema", "Table");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        TableTag first = new("Schema", "Table");
        TableTag second = new("Schema", "Table");
        TableTag third = new("Schema", "Table");

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_NullAndEmptySchema()
    {
        TableTag noSchema = new((SchemaName?)null, new TableName("Table"));
        TableTag emptySchema = new(new SchemaName(string.Empty), new TableName("Table"));

        Assert.True(noSchema.Equals(emptySchema));
        Assert.True(emptySchema.Equals(noSchema));
        Assert.True(noSchema == emptySchema);
        Assert.False(noSchema != emptySchema);
        Assert.Equal(noSchema.GetHashCode(), emptySchema.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentSchema()
    {
        TableTag left = new("SchemaOne", "Table");
        TableTag right = new("SchemaTwo", "Table");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_DifferentTable()
    {
        TableTag left = new("Schema", "TableOne");
        TableTag right = new("Schema", "TableTwo");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_SchemaPresenceMatters()
    {
        TableTag left = new(null, "Table");
        TableTag right = new("Schema", "Table");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_SchemaIsCaseSensitive()
    {
        TableTag left = new("Schema", "Table");
        TableTag right = new("schema", "Table");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_TableIsCaseSensitive()
    {
        TableTag left = new("Schema", "Table");
        TableTag right = new("Schema", "table");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_PreservesWhitespace()
    {
        TableTag left = new("Schema", "Table");
        TableTag right = new("Schema", " Table ");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_UsesStructuralIdentity_NotFormattedText()
    {
        TableTag left = new("A.B", "C");
        TableTag right = new("A", "B.C");

        Assert.Equal(left.ToString(), right.ToString());
        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_Null()
    {
        TableTag tableTag = new("Schema", "Table");
        TableTag? other = null;

        Assert.False(tableTag.Equals(other));
        Assert.False(tableTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        TableTag tableTag = new("Schema", "Table");
        object other = new TableTag("Schema", "Table");

        Assert.True(tableTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        TableTag tableTag = new("Schema", "Table");
        object other = new TableName("Table");

        Assert.False(tableTag.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        TableTag left = new("Schema", "Table");
        TableTag right = new("Schema", "Table");

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        TableTag left = new("SchemaOne", "Table");
        TableTag right = new("SchemaTwo", "Table");

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        TableTag tableTag = new("Schema", "Table");
        TableTag? nullTableTag = null;
        bool flag;
        Assert.False(tableTag == nullTableTag);
        Assert.False(nullTableTag == tableTag);
        Assert.True(tableTag != nullTableTag);
        Assert.True(nullTableTag != tableTag);
        flag = nullTableTag == null;
        Assert.True(flag);
        flag = nullTableTag != null;
        Assert.False(flag);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        TableTag left = new("Schema", "Table");
        TableTag right = new("Schema", "Table");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        TableTag storedKey = new("Schema", "Table");
        TableTag lookupKey = new("Schema", "Table");
        Dictionary<TableTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_NullAndEmptySchema()
    {
        TableTag storedKey = new((SchemaName?)null, new TableName("Table"));
        TableTag lookupKey = new(new SchemaName(string.Empty), new TableName("Table"));
        Dictionary<TableTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentSchema()
    {
        TableTag storedKey = new("SchemaOne", "Table");
        TableTag lookupKey = new("SchemaTwo", "Table");
        Dictionary<TableTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_DifferentTable()
    {
        TableTag storedKey = new("Schema", "TableOne");
        TableTag lookupKey = new("Schema", "TableTwo");
        Dictionary<TableTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        TableTag firstKey = new("Schema", "Table");
        TableTag secondKey = new("Schema", "Table");
        Dictionary<TableTag, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_IsCaseSensitive()
    {
        TableTag storedKey = new("Schema", "Table");
        TableTag lookupKey = new("schema", "Table");
        Dictionary<TableTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void HashSet_EquivalentInstance()
    {
        TableTag storedValue = new("Schema", "Table");
        TableTag lookupValue = new("Schema", "Table");
        HashSet<TableTag> set = [storedValue];

        Assert.Contains(lookupValue, set);
    }

    [Theory]
    [InlineData(null, "", true, true)]
    [InlineData(null, " ", false, true)]
    [InlineData(null, "Table", false, false)]
    [InlineData("Schema", "", false, false)]
    public void EmptyAndWhiteSpaceContracts(string? schemaName, string tableName, bool expectedEmpty, bool expectedWhiteSpace)
    {
        TableTag tableTag = new(schemaName, tableName);

        Assert.Equal(expectedEmpty, tableTag.IsEmpty());
        Assert.Equal(!expectedEmpty, tableTag.IsNotEmpty());
        Assert.Equal(expectedWhiteSpace, tableTag.IsWhiteSpace());
        Assert.Equal(!expectedWhiteSpace, tableTag.IsNotWhiteSpace());
    }

    [Fact]
    public void Get_ShouldReturnExpectedTableTag_ForEntityWithSchema()
    {
        TableTag expected = new("myschema", "EntityWithSchema");
#pragma warning disable CA2263 // Prefer generic overload when type is known
        TableTag actual = TableTag.Get(typeof(EntityWithSchema));
#pragma warning restore CA2263 // Prefer generic overload when type is known

        Assert.Equal(expected, actual);
        Assert.Equal(expected.GetHashCode(), actual.GetHashCode());
    }

    [Fact]
    public void GetGeneric_ShouldReturnExpectedTableTag_ForEntityWithSchema()
    {
        TableTag expected = new("myschema", "EntityWithSchema");
        TableTag actual = TableTag.Get<EntityWithSchema>();

        Assert.Equal(expected, actual);
    }
}
