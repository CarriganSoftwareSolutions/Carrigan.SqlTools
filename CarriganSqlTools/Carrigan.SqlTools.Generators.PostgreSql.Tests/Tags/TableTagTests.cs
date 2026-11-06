using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Tags;

public class TableTagTests
{    private readonly ITestOutputHelper _output;

    public TableTagTests(ITestOutputHelper output) => 
        _output = output;

    private static readonly PostgreSqlDialect Dialect = new();

    [Theory]
    [InlineData("Franks", "Pizza", "\"Franks\".\"Pizza\"")]
    [InlineData(null, "Pizza", "\"Pizza\"")]
    [InlineData("", "Pizza", "\"Pizza\"")]
    public void ToSql(string? schemaName, string tableName, string expected)
    {
        TableNameTag tableTag = new(schemaName, tableName);

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
        TableNameTag tableTag = new(schemaName, tableName);

        Assert.Equal(expected, tableTag.ToString());
        Assert.Equal(expected, $"{tableTag}");
    }

    [Fact]
    public void Equals_SameReference()
    {
        TableNameTag tableTag = new("Schema", "Table");

        Assert.True(tableTag.Equals(tableTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(tableTag == tableTag);
        Assert.False(tableTag != tableTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        TableNameTag left = new("Schema", "Table");
        TableNameTag right = new("Schema", "Table");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        TableNameTag first = new("Schema", "Table");
        TableNameTag second = new("Schema", "Table");
        TableNameTag third = new("Schema", "Table");

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_NullAndEmptySchema()
    {
        TableNameTag noSchema = new((SchemaName?)null, new TableName("Table"));
        TableNameTag emptySchema = new(new SchemaName(string.Empty), new TableName("Table"));

        Assert.True(noSchema.Equals(emptySchema));
        Assert.True(emptySchema.Equals(noSchema));
        Assert.True(noSchema == emptySchema);
        Assert.False(noSchema != emptySchema);
        Assert.Equal(noSchema.GetHashCode(), emptySchema.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentSchema()
    {
        TableNameTag left = new("SchemaOne", "Table");
        TableNameTag right = new("SchemaTwo", "Table");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_DifferentTable()
    {
        TableNameTag left = new("Schema", "TableOne");
        TableNameTag right = new("Schema", "TableTwo");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_SchemaPresenceMatters()
    {
        TableNameTag left = new(null, "Table");
        TableNameTag right = new("Schema", "Table");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_SchemaIsCaseSensitive()
    {
        TableNameTag left = new("Schema", "Table");
        TableNameTag right = new("schema", "Table");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_TableIsCaseSensitive()
    {
        TableNameTag left = new("Schema", "Table");
        TableNameTag right = new("Schema", "table");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_PreservesWhitespace()
    {
        TableNameTag left = new("Schema", "Table");
        TableNameTag right = new("Schema", " Table ");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_UsesStructuralIdentity_NotFormattedText()
    {
        TableNameTag left = new("A.B", "C");
        TableNameTag right = new("A", "B.C");

        Assert.Equal(left.ToString(), right.ToString());
        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_Null()
    {
        TableNameTag tableTag = new("Schema", "Table");
        TableNameTag? other = null;

        Assert.False(tableTag.Equals(other));
        Assert.False(tableTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        TableNameTag tableTag = new("Schema", "Table");
        object other = new TableNameTag("Schema", "Table");

        Assert.True(tableTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        TableNameTag tableTag = new("Schema", "Table");
        object other = new TableName("Table");

        Assert.False(tableTag.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        TableNameTag left = new("Schema", "Table");
        TableNameTag right = new("Schema", "Table");

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        TableNameTag left = new("SchemaOne", "Table");
        TableNameTag right = new("SchemaTwo", "Table");

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        TableNameTag tableTag = new("Schema", "Table");
        TableNameTag? nullTableTag = null;

        Assert.False(tableTag == nullTableTag);
        Assert.False(nullTableTag == tableTag);
        Assert.True(tableTag != nullTableTag);
        Assert.True(nullTableTag != tableTag);
        Assert.Null(nullTableTag);
        Assert.NotNull(tableTag);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        TableNameTag left = new("Schema", "Table");
        TableNameTag right = new("Schema", "Table");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        TableNameTag storedKey = new("Schema", "Table");
        TableNameTag lookupKey = new("Schema", "Table");
        Dictionary<TableNameTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_NullAndEmptySchema()
    {
        TableNameTag storedKey = new((SchemaName?)null, new TableName("Table"));
        TableNameTag lookupKey = new(new SchemaName(string.Empty), new TableName("Table"));
        Dictionary<TableNameTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentSchema()
    {
        TableNameTag storedKey = new("SchemaOne", "Table");
        TableNameTag lookupKey = new("SchemaTwo", "Table");
        Dictionary<TableNameTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_DifferentTable()
    {
        TableNameTag storedKey = new("Schema", "TableOne");
        TableNameTag lookupKey = new("Schema", "TableTwo");
        Dictionary<TableNameTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        TableNameTag firstKey = new("Schema", "Table");
        TableNameTag secondKey = new("Schema", "Table");
        Dictionary<TableNameTag, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_IsCaseSensitive()
    {
        TableNameTag storedKey = new("Schema", "Table");
        TableNameTag lookupKey = new("schema", "Table");
        Dictionary<TableNameTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void HashSet_EquivalentInstance()
    {
        TableNameTag storedValue = new("Schema", "Table");
        TableNameTag lookupValue = new("Schema", "Table");
        HashSet<TableNameTag> set = [storedValue];

        Assert.Contains(lookupValue, set);
    }

    [Theory]
    [InlineData(null, "", true, true)]
    [InlineData(null, " ", false, true)]
    [InlineData(null, "Table", false, false)]
    [InlineData("Schema", "", false, false)]
    public void EmptyAndWhiteSpaceContracts(string? schemaName, string tableName, bool expectedEmpty, bool expectedWhiteSpace)
    {
        TableNameTag tableTag = new(schemaName, tableName);

        Assert.Equal(expectedEmpty, tableTag.IsEmpty());
        Assert.Equal(!expectedEmpty, tableTag.IsNotEmpty());
        Assert.Equal(expectedWhiteSpace, tableTag.IsWhiteSpace());
        Assert.Equal(!expectedWhiteSpace, tableTag.IsNotWhiteSpace());
    }

    [Fact]
    public void Get_ShouldReturnExpectedTableTag_ForEntityWithSchema()
    {
        TableNameTag expected = new("myschema", "EntityWithSchema");
#pragma warning disable CA2263 // Prefer generic overload when type is known
        TableNameTag actual = TableNameTag.Get(typeof(EntityWithSchema));
#pragma warning restore CA2263 // Prefer generic overload when type is known

        Assert.Equal(expected, actual);
        Assert.Equal(expected.GetHashCode(), actual.GetHashCode());
    }

    [Fact]
    public void GetGeneric_ShouldReturnExpectedTableTag_ForEntityWithSchema()
    {
        TableNameTag expected = new("myschema", "EntityWithSchema");
        TableNameTag actual = TableNameTag.Get<EntityWithSchema>();

        Assert.Equal(expected, actual);
    }
}
