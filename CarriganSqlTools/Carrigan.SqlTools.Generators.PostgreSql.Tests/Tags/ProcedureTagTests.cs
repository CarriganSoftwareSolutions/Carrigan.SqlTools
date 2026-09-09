using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Tags;

public class ProcedureTagTests
{
    private static readonly PostgreSqlDialect Dialect = new();

    [Theory]
    [InlineData("Franks", "Pizza", "\"Franks\".\"Pizza\"")]
    [InlineData(null, "Pizza", "\"Pizza\"")]
    [InlineData("", "Pizza", "\"Pizza\"")]
    public void ToSql(string? schemaName, string procedureName, string expected)
    {
        ProcedureTag procedureTag = new(schemaName, procedureName);

        Assert.Equal(expected, procedureTag.ToSql(Dialect));
    }

    [Theory]
    [InlineData("Franks", "Pizza", "Franks.Pizza")]
    [InlineData(null, "Pizza", "Pizza")]
    [InlineData("", "Pizza", "Pizza")]
    [InlineData(null, "", "")]
    [InlineData(null, " ", " ")]
    public void ToString_Value(string? schemaName, string procedureName, string expected)
    {
        ProcedureTag procedureTag = new(schemaName, procedureName);

        Assert.Equal(expected, procedureTag.ToString());
        Assert.Equal(expected, $"{procedureTag}");
    }

    [Theory]
    [InlineData("Franks", "")]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Franks", null)]
    [InlineData(null, null)]
    [InlineData("", null)]
    public void Constructor_AllowsEmptyOrNullProcedureName(string? schemaName, string? procedureName) =>
        _ = new ProcedureTag(schemaName, procedureName!);

    [Fact]
    public void Equals_SameReference()
    {
        ProcedureTag procedureTag = new("Schema", "Procedure");

        Assert.True(procedureTag.Equals(procedureTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(procedureTag == procedureTag);
        Assert.False(procedureTag != procedureTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        ProcedureTag left = new("Schema", "Procedure");
        ProcedureTag right = new("Schema", "Procedure");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        ProcedureTag first = new("Schema", "Procedure");
        ProcedureTag second = new("Schema", "Procedure");
        ProcedureTag third = new("Schema", "Procedure");

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_NullAndEmptySchema()
    {
        ProcedureTag noSchema = new((SchemaName?)null, new ProcedureName("Procedure"));
        ProcedureTag emptySchema = new(new SchemaName(string.Empty), new ProcedureName("Procedure"));

        Assert.True(noSchema.Equals(emptySchema));
        Assert.True(emptySchema.Equals(noSchema));
        Assert.True(noSchema == emptySchema);
        Assert.False(noSchema != emptySchema);
        Assert.Equal(noSchema.GetHashCode(), emptySchema.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentSchema()
    {
        ProcedureTag left = new("SchemaOne", "Procedure");
        ProcedureTag right = new("SchemaTwo", "Procedure");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_DifferentProcedure()
    {
        ProcedureTag left = new("Schema", "ProcedureOne");
        ProcedureTag right = new("Schema", "ProcedureTwo");

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_SchemaPresenceMatters()
    {
        ProcedureTag left = new(null, "Procedure");
        ProcedureTag right = new("Schema", "Procedure");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_SchemaIsCaseSensitive()
    {
        ProcedureTag left = new("Schema", "Procedure");
        ProcedureTag right = new("schema", "Procedure");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_ProcedureIsCaseSensitive()
    {
        ProcedureTag left = new("Schema", "Procedure");
        ProcedureTag right = new("Schema", "procedure");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_PreservesWhitespace()
    {
        ProcedureTag left = new("Schema", "Procedure");
        ProcedureTag right = new("Schema", " Procedure ");

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_UsesStructuralIdentity_NotFormattedText()
    {
        ProcedureTag left = new("A.B", "C");
        ProcedureTag right = new("A", "B.C");

        Assert.Equal(left.ToString(), right.ToString());
        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_Null()
    {
        ProcedureTag procedureTag = new("Schema", "Procedure");
        ProcedureTag? other = null;

        Assert.False(procedureTag.Equals(other));
        Assert.False(procedureTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        ProcedureTag procedureTag = new("Schema", "Procedure");
        object other = new ProcedureTag("Schema", "Procedure");

        Assert.True(procedureTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        ProcedureTag procedureTag = new("Schema", "Procedure");
        object other = new ProcedureName("Procedure");

        Assert.False(procedureTag.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        ProcedureTag left = new("Schema", "Procedure");
        ProcedureTag right = new("Schema", "Procedure");

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        ProcedureTag left = new("SchemaOne", "Procedure");
        ProcedureTag right = new("SchemaTwo", "Procedure");

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        ProcedureTag procedureTag = new("Schema", "Procedure");
        ProcedureTag? nullProcedureTag = null;
        ProcedureTag? secondNullProcedureTag = null;

        Assert.False(procedureTag == nullProcedureTag);
        Assert.False(nullProcedureTag == procedureTag);
        Assert.True(procedureTag != nullProcedureTag);
        Assert.True(nullProcedureTag != procedureTag);
        Assert.True(nullProcedureTag == secondNullProcedureTag);
        Assert.False(nullProcedureTag != secondNullProcedureTag);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        ProcedureTag left = new("Schema", "Procedure");
        ProcedureTag right = new("Schema", "Procedure");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_EquivalentInstance()
    {
        ProcedureTag storedKey = new("Schema", "Procedure");
        ProcedureTag lookupKey = new("Schema", "Procedure");
        Dictionary<ProcedureTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_NullAndEmptySchema()
    {
        ProcedureTag storedKey = new((SchemaName?)null, new ProcedureName("Procedure"));
        ProcedureTag lookupKey = new(new SchemaName(string.Empty), new ProcedureName("Procedure"));
        Dictionary<ProcedureTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.True(dictionary.ContainsKey(lookupKey));
        Assert.Equal(3.14159m, dictionary[lookupKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentSchema()
    {
        ProcedureTag storedKey = new("SchemaOne", "Procedure");
        ProcedureTag lookupKey = new("SchemaTwo", "Procedure");
        Dictionary<ProcedureTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_DifferentProcedure()
    {
        ProcedureTag storedKey = new("Schema", "ProcedureOne");
        ProcedureTag lookupKey = new("Schema", "ProcedureTwo");
        Dictionary<ProcedureTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        ProcedureTag firstKey = new("Schema", "Procedure");
        ProcedureTag secondKey = new("Schema", "Procedure");
        Dictionary<ProcedureTag, decimal> dictionary = [];
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_IsCaseSensitive()
    {
        ProcedureTag storedKey = new("Schema", "Procedure");
        ProcedureTag lookupKey = new("schema", "Procedure");
        Dictionary<ProcedureTag, decimal> dictionary = [];
        dictionary[storedKey] = 3.14159m;

        Assert.False(dictionary.ContainsKey(lookupKey));
    }

    [Fact]
    public void HashSet_EquivalentInstance()
    {
        ProcedureTag storedValue = new("Schema", "Procedure");
        ProcedureTag lookupValue = new("Schema", "Procedure");
        HashSet<ProcedureTag> set = [storedValue];

        Assert.Contains(lookupValue, set);
    }

    [Theory]
    [InlineData(null, "", true, true)]
    [InlineData(null, " ", false, true)]
    [InlineData(null, "Procedure", false, false)]
    [InlineData("Schema", "", false, false)]
    public void EmptyAndWhiteSpaceContracts(string? schemaName, string procedureName, bool expectedEmpty, bool expectedWhiteSpace)
    {
        ProcedureTag procedureTag = new(schemaName, procedureName);

        Assert.Equal(expectedEmpty, procedureTag.IsEmpty());
        Assert.Equal(!expectedEmpty, procedureTag.IsNotEmpty());
        Assert.Equal(expectedWhiteSpace, procedureTag.IsWhiteSpace());
        Assert.Equal(!expectedWhiteSpace, procedureTag.IsNotWhiteSpace());
    }

    [Theory]
    [InlineData("Invalid Procedure")]
    [InlineData("123Invalid")]
    [InlineData("Role;DROP")]
    // These tests document that identifier validation occurs in the SQL generator rather than ProcedureTag construction.
    public void Constructor_AllowsProcedureNamesValidatedLater(string procedureName)
    {
        ProcedureTag procedureTag = new("dbo", procedureName);

        Assert.Equal(procedureName, procedureTag.ProcedureName.ToString());
    }

    [Theory]
    [InlineData("Invalid Schema")]
    [InlineData("123Schema")]
    [InlineData("Sch;ema")]
    // These tests document that identifier validation occurs in the SQL generator rather than ProcedureTag construction.
    public void Constructor_AllowsSchemaNamesValidatedLater(string schemaName)
    {
        ProcedureTag procedureTag = new(schemaName, "ValidProcedure");

        Assert.Equal(schemaName, procedureTag.SchemaName!.ToString());
    }

    [Fact]
    public void Constructor_ValidWithoutSchema()
    {
        ProcedureTag procedureTag = new(null, "ValidProcedure");

        Assert.Equal("ValidProcedure", procedureTag.ToString());
        Assert.Equal("\"ValidProcedure\"", procedureTag.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_ValidWithSchema()
    {
        ProcedureTag procedureTag = new("dbo", "ValidProcedure");

        Assert.Equal("dbo.ValidProcedure", procedureTag.ToString());
        Assert.Equal("\"dbo\".\"ValidProcedure\"", procedureTag.ToSql(Dialect));
    }

    [Fact]
    public void Get_ShouldReturnExpectedProcedureTag_ForEntityWithSchema()
    {
        ProcedureTag expected = new("myschema", "EntityWithSchema");
#pragma warning disable CA2263 // Prefer generic overload when type is known
        ProcedureTag actual = ProcedureTag.Get(typeof(EntityWithSchema));
#pragma warning restore CA2263 // Prefer generic overload when type is known

        Assert.Equal(expected, actual);
        Assert.Equal(expected.GetHashCode(), actual.GetHashCode());
    }

    [Fact]
    public void ToString_FromGet_ReturnsExpectedString()
    {
#pragma warning disable CA2263 // Prefer generic overload when type is known
        ProcedureTag procedureTag = ProcedureTag.Get(typeof(EntityWithSchema));
#pragma warning restore CA2263 // Prefer generic overload when type is known

        Assert.Equal("myschema.EntityWithSchema", procedureTag.ToString());
    }
}
