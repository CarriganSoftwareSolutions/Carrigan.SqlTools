using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.Tags;

//IGNORE SPELLING: Za ema myschema dbo

public class ProcedureTagTests
{
    [Theory]
    [InlineData("Poppies", "Pizza", "[Poppies].[Pizza]")]
    [InlineData(null, "Pizza", "[Pizza]")]
    [InlineData("", "Pizza", "[Pizza]")]
    public void Procedure_Tag_Tests(string? schemaName, string name, string expected)
    {
        string actual = new ProcedureTag(schemaName, name);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Poppies", "")]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Poppies", null)]
    [InlineData(null, null)]
    [InlineData("", null)]
    public void Procedure_Tag_Tests_Argument_Exception(string? schemaName, string? procedureName) => 
        Assert.Throws<ArgumentNullException>(() => new ProcedureTag(schemaName, procedureName!));

    [Theory]
    [InlineData("Poppies", "Pizza", "Poppies", "Pizza")]
    [InlineData("Planet", "Express", "Poppies", "Pizza")]
    [InlineData("Poppies", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Poppies", "Pizza", null, null)]
    public void ProcedureTag_Comparisons(string? schema1, string? procedure1, string? schema2, string? procedure2)
    {
        ProcedureTag? tag1 = procedure1.IsNotNullOrEmpty() ? new ProcedureTag(schema1, procedure1) : null;
        ProcedureTag? tag2 = procedure2.IsNotNullOrEmpty() ? new ProcedureTag(schema2, procedure2) : null;

        string? tagString1 = procedure1.IsNotNullOrEmpty() ? $"[{schema1}].[{procedure1}]" : null;
        string? tagString2 = procedure2.IsNotNullOrEmpty() ? $"[{schema2}].[{procedure2}]" : null;


        int expectedValue = string.Compare(tagString1, tagString2, StringComparison.Ordinal);
        int actualValue = tag1!.CompareTo(tag2);

        Assert.Equal(expectedValue, actualValue);
    }


    [Theory]
    [InlineData("Poppies", "Pizza", "Poppies", "Pizza")]
    [InlineData("Planet", "Express", "Poppies", "Pizza")]
    [InlineData("Poppies", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Poppies", "Pizza", null, null)]
    public void ProcedureTag_Equals(string? schema1, string? procedure1, string? schema2, string? procedure2)
    {
        ProcedureTag? tag1 = procedure1.IsNotNullOrEmpty() ? new ProcedureTag(schema1, procedure1) : null;
        ProcedureTag? tag2 = procedure2.IsNotNullOrEmpty() ? new ProcedureTag(schema2, procedure2) : null;

        string? tagString1 = procedure1.IsNotNullOrEmpty() ? $"[{schema1}].[{procedure1}]" : null;
        string? tagString2 = procedure2.IsNotNullOrEmpty() ? $"[{schema2}].[{procedure2}]" : null;


        bool expectedValue = tagString1!.Equals(tagString2);
        bool actualValue = tag1!.Equals(tag2);

        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("Poppies", "Pizza", "Poppies", "Pizza")]
    [InlineData("Planet", "Express", "Poppies", "Pizza")]
    [InlineData("Poppies", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Poppies", "Pizza", null, null)]
    public void ProcedureTag_EqualsObject(string? schema1, string? procedure1, string? schema2, string? procedure2)
    {
        ProcedureTag? tag1 = procedure1.IsNotNullOrEmpty() ? new ProcedureTag(schema1, procedure1) : null;
        ProcedureTag? tag2 = procedure2.IsNotNullOrEmpty() ? new ProcedureTag(schema2, procedure2) : null;

        string? tagString1 = procedure1.IsNotNullOrEmpty() ? $"[{schema1}].[{procedure1}]" : null;
        string? tagString2 = procedure2.IsNotNullOrEmpty() ? $"[{schema2}].[{procedure2}]" : null;


        bool expectedValue = tagString1!.Equals(tagString2);
        bool actualValue = tag1!.Equals((object?)tag2);

        Assert.Equal(expectedValue, actualValue);
    }


    [Theory]
    [InlineData("Poppies", "Pizza", "Poppies", "Pizza")]
    [InlineData("Planet", "Express", "Poppies", "Pizza")]
    [InlineData("Poppies", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Poppies", "Pizza", null, null)]
    public void ProcedureTag_EqualsEquals(string? schema1, string? procedure1, string? schema2, string? procedure2)
    {
        ProcedureTag? tag1 = procedure1.IsNotNullOrEmpty() ? new ProcedureTag(schema1, procedure1) : null;
        ProcedureTag? tag2 = procedure2.IsNotNullOrEmpty() ? new ProcedureTag(schema2, procedure2) : null;

        string? tagString1 = procedure1.IsNotNullOrEmpty() ? $"[{schema1}].[{procedure1}]" : null;
        string? tagString2 = procedure2.IsNotNullOrEmpty() ? $"[{schema2}].[{procedure2}]" : null;

        bool expectedValue = tagString1 == tagString2;
        bool actualValue = tag1 == tag2;

        Assert.Equal(expectedValue, actualValue);
    }


    [Theory]
    [InlineData("Poppies", "Pizza", "Poppies", "Pizza")]
    [InlineData("Planet", "Express", "Poppies", "Pizza")]
    [InlineData("Poppies", "Pizza", "Planet", "Express")]
    [InlineData("Planet", "Express", "Planet", "Express")]
    [InlineData("Poppies", "Pizza", null, null)]
    public void ProcedureTag_NotEquals(string? schema1, string? procedure1, string? schema2, string? procedure2)
    {
        ProcedureTag? tag1 = procedure1.IsNotNullOrEmpty() ? new ProcedureTag(schema1, procedure1) : null;
        ProcedureTag? tag2 = procedure2.IsNotNullOrEmpty() ? new ProcedureTag(schema2, procedure2) : null;

        string? tagString1 = procedure1.IsNotNullOrEmpty() ? $"[{schema1}].[{procedure1}]" : null;
        string? tagString2 = procedure2.IsNotNullOrEmpty() ? $"[{schema2}].[{procedure2}]" : null;

        bool expectedValue = tagString1 != tagString2;
        bool actualValue = tag1 != tag2;

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void ProcedureTag_Equals_BothNull_ReturnsTrue()
    {
        // Since the comparison methods are on ProcedureTag itself, we just need an instance.
        ProcedureTag comparer = new("Schema", "Procedure");

        bool result = comparer.Equals(null, null);

        Assert.True(result, "Both null references should be considered equal.");
    }

    [Fact]
    public void ProcedureTag__Equals_OneNullOneNonNull_ReturnsFalse()
    {
        ProcedureTag comparer = new("Schema", "Procedure");
        ProcedureTag nonNullTag = new("Schema", "Procedure");

        bool result = comparer.Equals(null, nonNullTag);

        Assert.False(result, "Null and non-null should not be equal.");
    }

    [Fact]
    public void ProcedureTag_Equals_SameValues_ReturnsTrue()
    {
        ProcedureTag comparer = new("Schema", "Procedure");
        ProcedureTag tag1 = new("Schema", "Procedure");
        ProcedureTag tag2 = new("Schema", "Procedure");

        bool result = comparer.Equals(tag1, tag2);

        Assert.True(result, "Tags with the same schema/Procedure strings should be equal.");
    }

    [Fact]
    public void ProcedureTag_Equals_DifferentValues_ReturnsFalse()
    {
        ProcedureTag comparer = new("Schema", "Procedure");
        ProcedureTag tag1 = new("SchemaA", "ProcedureA");
        ProcedureTag tag2 = new("SchemaB", "ProcedureB");

        bool result = comparer.Equals(tag1, tag2);

        Assert.False(result, "Tags with different schema/Procedure strings should not be equal.");
    }

    [Fact]
    public void ProcedureTag_GetHashCode_SameValues_ReturnsSameHash()
    {
        ProcedureTag comparer = new("Schema", "Procedure");
        ProcedureTag tag1 = new("MySchema", "MyProcedure");
        ProcedureTag tag2 = new("MySchema", "MyProcedure");

        int hash1 = comparer.GetHashCode(tag1);
        int hash2 = comparer.GetHashCode(tag2);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ProcedureTag_GetHashCode_DifferentValues_ReturnsDifferentHash()
    {
        ProcedureTag comparer = new("Schema", "Procedure");
        ProcedureTag tag1 = new("Schema1", "procedure1");
        ProcedureTag tag2 = new("Schema2", "procedure2");

        int hash1 = comparer.GetHashCode(tag1);
        int hash2 = comparer.GetHashCode(tag2);

        Assert.NotEqual(hash1, hash2);
    }


    [Fact]
    public void Constructor_ValidWithoutSchema_ShouldReturnFormattedTag()
    {
        // Arrange
        string ProcedureName = "ValidProcedure"; // passes pattern e.g. "^[A-Za-z_@#][A-Za-z0-9_@$#]*$"
        string expected = "[ValidProcedure]";

        // Act
        ProcedureTag ProcedureTag = new(null, ProcedureName);

        // Assert
        Assert.Equal(expected, ProcedureTag.ToString());
        // Test implicit conversion to string.
        string implicitString = ProcedureTag;
        Assert.Equal(expected, implicitString);
    }

    [Fact]
    public void Constructor_ValidWithSchema_ShouldReturnFormattedTag()
    {
        // Arrange
        string schemaName = "dbo";
        string ProcedureName = "ValidProcedure";
        string expected = "[dbo].[ValidProcedure]";

        // Act
        ProcedureTag ProcedureTag = new(schemaName, ProcedureName);

        // Assert
        Assert.Equal(expected, ProcedureTag.ToString());
        string implicitString = ProcedureTag;
        Assert.Equal(expected, implicitString);
    }

    [Theory]
    [InlineData("Invalid Procedure")]    // Contains space.
    [InlineData("123Invalid")]       // Starts with digit.
    [InlineData("Role;DROP")]        // Contains a semicolon.
    public void Constructor_InvalidProcedureName_ShouldThrowSqlNamePatternException(string invalidProcedure)
    {
        // Arrange
        string schemaName = "dbo";

        // Act & Assert
        Assert.Throws<SqlNamePatternException>(() => new ProcedureTag(schemaName, invalidProcedure));
    }

    [Theory]
    [InlineData("Invalid Schema")]   // Contains space.
    [InlineData("123Schema")]        // Starts with digit.
    [InlineData("Sch;ema")]          // Contains special characters.
    public void Constructor_InvalidSchemaName_ShouldThrowSqlNamePatternException(string invalidSchema)
    {
        // Arrange
        string ProcedureName = "ValidProcedure";

        // Act & Assert
        Assert.Throws<SqlNamePatternException>(() => new ProcedureTag(invalidSchema, ProcedureName));
    }

    [Fact]
    public void ToString_ReturnsFormattedTag()
    {
        // Arrange
        string schemaName = "dbo";
        string ProcedureName = "ValidProcedure";
        ProcedureTag ProcedureTag = new(schemaName, ProcedureName);
        string expected = "[dbo].[ValidProcedure]";

        // Act
        string result = ProcedureTag.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ImplicitConversionToString_ReturnsFormattedTag()
    {
        // Arrange
        string schemaName = "dbo";
        string ProcedureName = "ValidProcedure";
        ProcedureTag ProcedureTag = new(schemaName, ProcedureName);
        string expected = "[dbo].[ValidProcedure]";

        // Act
        string result = ProcedureTag;  // Implicit conversion to string

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Get_ShouldReturnExpectedProcedureTag_ForEntityWithSchema()
    {
        // Arrange
        Type entityType = typeof(EntityWithSchema);
        ProcedureTag expectedTag = new("myschema", "EntityWithSchema");

        // Act
        ProcedureTag actualTag = ProcedureTag.Get(entityType);

        // Assert: compare string representations (implicit conversion and ToString())
        Assert.Equal(expectedTag.ToString(), actualTag.ToString());
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnSameString_AsToString()
    {
        // Arrange
        Type entityType = typeof(EntityWithSchema);
        ProcedureTag tagFromGet = ProcedureTag.Get(entityType);
        string tagAsStringFromToString = tagFromGet.ToString();
        string tagAsStringFromImplicit = tagFromGet;

        // Assert: both conversion methods yield the same result
        Assert.Equal(tagAsStringFromToString, tagAsStringFromImplicit);
    }
}
