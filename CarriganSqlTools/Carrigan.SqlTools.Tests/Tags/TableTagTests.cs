using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.Tags;

//IGNORE SPELLING: Za ema myschema dbo

public class TableTagTests
{
    [Theory]
    [InlineData("Franks", "Pizza", "[Franks].[Pizza]")]
    [InlineData(null, "Pizza", "[Pizza]")]
    [InlineData("", "Pizza", "[Pizza]")]
    public void Table_Tag_Tests(string? schemaName, string tableName, string expected)
    {
        string actual = new TableTag(schemaName, tableName);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Franks", "")]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Franks", null)]
    [InlineData(null, null)]
    [InlineData("", null)]
    //These unit tests originally enforced exceptions being throw when you create a table tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Table_Tag_Tests_Argument_Exception(string? schemaName, string? tableName) => 
        _ = new TableTag(schemaName, tableName!);

    [Theory]
    [InlineData("Franks", "Pizza", "Franks", "Pizza")]
    [InlineData("Planet", "Express", "Franks", "Pizza")]
    [InlineData("Franks", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Franks", "Pizza", null, null)]
    public void TableTag_Comparisons(string? schema1, string? table1, string? schema2, string? table2)
    {
        TableTag? tag1 = table1.IsNotNullOrEmpty() ? new TableTag(schema1, table1) : null;
        TableTag? tag2 = table2.IsNotNullOrEmpty() ? new TableTag(schema2, table2) : null;

        string? tagString1 = table1.IsNotNullOrEmpty() ? $"[{schema1}].[{table1}]" : null;
        string? tagString2 = table2.IsNotNullOrEmpty() ? $"[{schema2}].[{table2}]" : null;


        int expectedValue = string.Compare(tagString1, tagString2, StringComparison.Ordinal);
        int actualValue = tag1!.CompareTo(tag2);

        Assert.Equal(expectedValue, actualValue);
    }


    [Theory]
    [InlineData("Franks", "Pizza", "Franks", "Pizza")]
    [InlineData("Planet", "Express", "Franks", "Pizza")]
    [InlineData("Franks", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Franks", "Pizza", null, null)]
    public void TableTag_Equals(string? schema1, string? table1, string? schema2, string? table2)
    {
        TableTag? tag1 = table1.IsNotNullOrEmpty() ? new TableTag(schema1, table1) : null;
        TableTag? tag2 = table2.IsNotNullOrEmpty() ? new TableTag(schema2, table2) : null;

        string? tagString1 = table1.IsNotNullOrEmpty() ? $"[{schema1}].[{table1}]" : null;
        string? tagString2 = table2.IsNotNullOrEmpty() ? $"[{schema2}].[{table2}]" : null;


        bool expectedValue = tagString1!.Equals(tagString2);
        bool actualValue = tag1!.Equals(tag2);

        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("Franks", "Pizza", "Franks", "Pizza")]
    [InlineData("Planet", "Express", "Franks", "Pizza")]
    [InlineData("Franks", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Franks", "Pizza", null, null)]
    public void TableTag_EqualsObject(string? schema1, string? table1, string? schema2, string? table2)
    {
        TableTag? tag1 = table1.IsNotNullOrEmpty() ? new TableTag(schema1, table1) : null;
        TableTag? tag2 = table2.IsNotNullOrEmpty() ? new TableTag(schema2, table2) : null;

        string? tagString1 = table1.IsNotNullOrEmpty() ? $"[{schema1}].[{table1}]" : null;
        string? tagString2 = table2.IsNotNullOrEmpty() ? $"[{schema2}].[{table2}]" : null;


        bool expectedValue = tagString1!.Equals(tagString2);
        bool actualValue = tag1!.Equals((object?)tag2);

        Assert.Equal(expectedValue, actualValue);
    }


    [Theory]
    [InlineData("Franks", "Pizza", "Franks", "Pizza")]
    [InlineData("Planet", "Express", "Franks", "Pizza")]
    [InlineData("Franks", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Franks", "Pizza", null, null)]
    public void TableTag_EqualsEquals(string? schema1, string? table1, string? schema2, string? table2)
    {
        TableTag? tag1 = table1.IsNotNullOrEmpty() ? new TableTag(schema1, table1) : null;
        TableTag? tag2 = table2.IsNotNullOrEmpty() ? new TableTag(schema2, table2) : null;

        string? tagString1 = table1.IsNotNullOrEmpty() ? $"[{schema1}].[{table1}]" : null;
        string? tagString2 = table2.IsNotNullOrEmpty() ? $"[{schema2}].[{table2}]" : null;

        bool expectedValue = tagString1 == tagString2;
        bool actualValue = tag1 == tag2;

        Assert.Equal(expectedValue, actualValue);
    }


    [Theory]
    [InlineData("Franks", "Pizza", "Franks", "Pizza")]
    [InlineData("Planet", "Express", "Franks", "Pizza")]
    [InlineData("Franks", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Franks", "Pizza", null, null)]
    public void TableTag_NotEquals(string? schema1, string? table1, string? schema2, string? table2)
    {
        TableTag? tag1 = table1.IsNotNullOrEmpty() ? new TableTag(schema1, table1) : null;
        TableTag? tag2 = table2.IsNotNullOrEmpty() ? new TableTag(schema2, table2) : null;

        string? tagString1 = table1.IsNotNullOrEmpty() ? $"[{schema1}].[{table1}]" : null;
        string? tagString2 = table2.IsNotNullOrEmpty() ? $"[{schema2}].[{table2}]" : null;

        bool expectedValue = tagString1 != tagString2;
        bool actualValue = tag1 != tag2;

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void TableTag_Equals_BothNull_ReturnsTrue()
    {
        // Since the comparison methods are on TableTag itself, we just need an instance.
        TableTag comparer = new("Schema", "Table");

        bool result = comparer.Equals(null, null);

        Assert.True(result, "Both null references should be considered equal.");
    }

    [Fact]
    public void TableTag__Equals_OneNullOneNonNull_ReturnsFalse()
    {
        TableTag comparer = new("Schema", "Table");
        TableTag nonNullTag = new("Schema", "Table");

        bool result = comparer.Equals(null, nonNullTag);

        Assert.False(result, "Null and non-null should not be equal.");
    }

    [Fact]
    public void TableTag_Equals_SameValues_ReturnsTrue()
    {
        TableTag comparer = new("Schema", "Table");
        TableTag tag1 = new("Schema", "Table");
        TableTag tag2 = new("Schema", "Table");

        bool result = comparer.Equals(tag1, tag2);

        Assert.True(result, "Tags with the same schema/table strings should be equal.");
    }

    [Fact]
    public void TableTag_Equals_DifferentValues_ReturnsFalse()
    {
        TableTag comparer = new("Schema", "Table");
        TableTag tag1 = new("SchemaA", "TableA");
        TableTag tag2 = new("SchemaB", "TableB");

        bool result = comparer.Equals(tag1, tag2);

        Assert.False(result, "Tags with different schema/table strings should not be equal.");
    }

    [Fact]
    public void TableTag_GetHashCode_SameValues_ReturnsSameHash()
    {
        TableTag comparer = new("Schema", "Table");
        TableTag tag1 = new("MySchema", "MyTable");
        TableTag tag2 = new("MySchema", "MyTable");

        int hash1 = comparer.GetHashCode(tag1);
        int hash2 = comparer.GetHashCode(tag2);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void TableTag_GetHashCode_DifferentValues_ReturnsDifferentHash()
    {
        TableTag comparer = new("Schema", "Table");
        TableTag tag1 = new("Schema1", "Table1");
        TableTag tag2 = new("Schema2", "Table2");

        int hash1 = comparer.GetHashCode(tag1);
        int hash2 = comparer.GetHashCode(tag2);

        Assert.NotEqual(hash1, hash2);
    }


    [Fact]
    public void Constructor_ValidWithoutSchema_ShouldReturnFormattedTag()
    {
        // Arrange
        string tableName = "ValidTable"; // passes pattern e.g. "^[A-Za-z_@#][A-Za-z0-9_@$#]*$"
        string expected = "[ValidTable]";

        // Act
        TableTag tableTag = new(null, tableName);

        // Assert
        Assert.Equal(expected, tableTag.ToString());
        // Test implicit conversion to string.
        string implicitString = tableTag;
        Assert.Equal(expected, implicitString);
    }

    [Fact]
    public void Constructor_ValidWithSchema_ShouldReturnFormattedTag()
    {
        // Arrange
        string schemaName = "dbo";
        string tableName = "ValidTable";
        string expected = "[dbo].[ValidTable]";

        // Act
        TableTag tableTag = new(schemaName, tableName);

        // Assert
        Assert.Equal(expected, tableTag.ToString());
        string implicitString = tableTag;
        Assert.Equal(expected, implicitString);
    }

    [Theory]
    [InlineData("Invalid Table")]    // Contains space.
    [InlineData("123Invalid")]       // Starts with digit.
    [InlineData("Role;DROP")]        // Contains a semicolon.
    //These unit tests originally enforced exceptions being throw when you create a table tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Constructor_InvalidTableName_ShouldThrowSqlNamePatternException(string invalidTable)
    {
        // Arrange
        string schemaName = "dbo";

        // Act & Assert
        _ = new TableTag(schemaName, invalidTable);
    }

    [Theory]
    [InlineData("Invalid Schema")]   // Contains space.
    [InlineData("123Schema")]        // Starts with digit.
    [InlineData("Sch;ema")]          // Contains special characters.
    //These unit tests originally enforced exceptions being throw when you create a table tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Constructor_InvalidSchemaName_ShouldThrowSqlNamePatternException(string invalidSchema)
    {
        // Arrange
        string tableName = "ValidTable";

        // Act & Assert
        _ = new TableTag(invalidSchema, tableName);
    }

    [Fact]
    public void ToString_ReturnsFormattedTag()
    {
        // Arrange
        string schemaName = "dbo";
        string tableName = "ValidTable";
        TableTag tableTag = new(schemaName, tableName);
        string expected = "[dbo].[ValidTable]";

        // Act
        string result = tableTag.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ImplicitConversionToString_ReturnsFormattedTag()
    {
        // Arrange
        string schemaName = "dbo";
        string tableName = "ValidTable";
        TableTag tableTag = new(schemaName, tableName);
        string expected = "[dbo].[ValidTable]";

        // Act
        string result = tableTag;  // Implicit conversion to string

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Get_ShouldReturnExpectedTableTag_ForEntityWithSchema()
    {
        // Arrange
        Type entityType = typeof(EntityWithSchema);
        TableTag expectedTag = new("myschema", "EntityWithSchema");

        // Act
        TableTag actualTag = TableTag.Get(entityType);

        // Assert: compare string representations (implicit conversion and ToString())
        Assert.Equal(expectedTag.ToString(), actualTag.ToString());
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnSameString_AsToString()
    {
        // Arrange
        Type entityType = typeof(EntityWithSchema);
        TableTag tagFromGet = TableTag.Get(entityType);
        string tagAsStringFromToString = tagFromGet.ToString();
        string tagAsStringFromImplicit = tagFromGet;

        // Assert: both conversion methods yield the same result
        Assert.Equal(tagAsStringFromToString, tagAsStringFromImplicit);
    }
}
