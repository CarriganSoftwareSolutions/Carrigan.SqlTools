using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using System.Reflection;
//IGNORE SPELLING: dbo
namespace Carrigan.SqlTools.Tests.ReflectorCacheTests;
public class ColumnInfoTests
{
    [Theory]
    [InlineData(null, "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "Id", new[] { "Id" },
        "[ColumnIdentifiersTable].[Id]", "Id", "Id", 
        "IdParameter",  null, "[ColumnIdentifiersTable].[Id]",
        true, false, false,  "[ColumnIdentifiersTable]")]
    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "Id", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Id]", "Id", "Id",
        "IdParameter", null, "[dbo].[ColumnIdentifiersTable].[Id]",
        true, false, false, "[dbo].[ColumnIdentifiersTable]")]
    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "Property", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Property]", "Property", "Property",
        "PropertyParameter", null, "[dbo].[ColumnIdentifiersTable].[Property]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]")]
    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "ColumnName", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Column]", "Column", "ColumnName",
        "ColumnParameter", null, "[dbo].[ColumnIdentifiersTable].[Column]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]")]
    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "IdentifierName", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Identifier]", "Identifier", "IdentifierName",
        "IdentifierParameter", null, "[dbo].[ColumnIdentifiersTable].[Identifier]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]")]
    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "IdentifierOverrideName", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[IdentifierOverride]", "IdentifierOverride", "IdentifierOverrideName",
        "IdentifierOverrideParameter", null, "[dbo].[ColumnIdentifiersTable].[IdentifierOverride]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]")]
    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "Id", new[] { "Id" },
        "[dbo].[Test].[Id]", "Id", "Id",
        "Id", null, "[dbo].[Test].[Id]",
        true, false, false, "[dbo].[Test]")]
    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "NotSensitiveData", new[] { "Id" },
        "[dbo].[Test].[NotSensitiveData]", "NotSensitiveData", "NotSensitiveData",
        "NotSensitiveData", null, "[dbo].[Test].[NotSensitiveData]",
        false, false, false, "[dbo].[Test]")]
    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "SensitiveData", new[] { "Id" },
        "[dbo].[Test].[SensitiveData]", "SensitiveData", "SensitiveData",
        "SensitiveData", null, "[dbo].[Test].[SensitiveData]",
        false, true, false, "[dbo].[Test]")]
    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "KeyVersion", new[] { "Id" },
        "[dbo].[Test].[KeyVersion]", "KeyVersion", "KeyVersion",
        "KeyVersion", null, "[dbo].[Test].[KeyVersion]",
        false, false, true, "[dbo].[Test]")]
    [InlineData("dbo", "TableWithAliases", typeof(TableWithAliases), "Id", new[] { "Id" },
        "[dbo].[TableWithAliases].[Id]", "Id", "Id",
        "Id", "TableId", "[dbo].[TableWithAliases].[Id] AS TableId",
        true, false, false, "[dbo].[TableWithAliases]")]
    public void New(string? schemaName, string tableName, Type type, string propertyName, string[] keyProperties,
        string expectedColumnTag, string  expectedColumnName, string expectedPropertyName, 
        string parameterTag, string? expectedAliasName, string expectedSelectTag,
        bool expectedIsKeyPart, bool expectedIsEncrypted, bool expectedIsKeyVersionProperty,
        string expectedTableTag)
    {
        IEnumerable<PropertyInfo> keys = keyProperties.Select(property => type.GetProperty(property)).OfType<PropertyInfo>();

        SchemaName? schema =  SchemaName.New(schemaName);
        TableName table = new(tableName);
        PropertyInfo? property = type.GetProperty(propertyName);
        Assert.NotNull(property);
        ColumnInfo columnInfo = new(schema, table, property, keys);
        string implicitString = columnInfo;
        string explicitString = columnInfo.ToString();
        int hashCode = columnInfo.GetHashCode();

        Assert.Equal(expectedColumnTag, columnInfo.ColumnTag);
        Assert.Equal(expectedColumnTag, columnInfo.ColumnTag);
        Assert.Equal(expectedColumnTag, columnInfo.ColumnTag);
        Assert.Equal(expectedColumnName, columnInfo.ColumnName);
        Assert.Equal(expectedTableTag, columnInfo.ColumnTag.TableTag);
        Assert.Equal(expectedPropertyName, columnInfo.PropertyInfo.Name);
        Assert.Equal(expectedPropertyName, columnInfo.PropertyName);
        Assert.Equal(parameterTag, columnInfo.ParameterTag);

        if (columnInfo.AliasName is not null)
            Assert.Equal(expectedAliasName, columnInfo.AliasName);
        else
            Assert.Null(expectedAliasName);

        if (columnInfo.SelectTag.AliasTag is not null)
            Assert.Equal(expectedAliasName, columnInfo.SelectTag.AliasTag);
        else
            Assert.Null(expectedAliasName);

        Assert.Equal(expectedSelectTag, columnInfo.SelectTag);
        Assert.Equal(expectedIsKeyPart, columnInfo.IsKeyPart);
        Assert.Equal(expectedIsEncrypted, columnInfo.IsEncrypted);
        Assert.Equal(expectedIsKeyVersionProperty, columnInfo.IsKeyVersionProperty);

        Assert.Equal(expectedColumnTag, implicitString);
        Assert.Equal(expectedColumnTag, explicitString);
        Assert.Equal(expectedColumnTag.GetHashCode(), columnInfo.GetHashCode());

        Assert.False(columnInfo.IsEmpty());
    }

    [Fact]
    public void Compare()
    {
        ColumnInfo New(string propertyName)
        {
            Type type = typeof(ColumnIdentifiers);
            string[] keyProperties = ["Id"];
            IEnumerable<PropertyInfo> keys = keyProperties.Select(property => type.GetProperty(property)).OfType<PropertyInfo>();

            SchemaName? schema = SchemaName.New("dbo");
            TableName table = new("ColumnIdentifiersTable");
            PropertyInfo? property = type.GetProperty(propertyName);
            Assert.NotNull(property);
            return new(schema, table, property, keys);
        }
        ColumnInfo a = New("Id");
        ColumnInfo aAlt = New("Id");
        ColumnInfo b = New("Property");
        ColumnInfo bAlt = New("Property");

        Assert.Equal(0, a.CompareTo(aAlt));
        Assert.Equal(0, b.CompareTo(bAlt));
        Assert.NotEqual(0, b.CompareTo(a));
        Assert.Equal(a, aAlt);
        Assert.Equal(b, bAlt);
        Assert.NotEqual(a, b);
        Assert.True(a.Equals(aAlt));
        Assert.True(b.Equals(bAlt));
        Assert.False(a.Equals(b));
        Assert.True(a == aAlt);
        Assert.True(b == bAlt);
        Assert.False(a == b);
        Assert.False(a != aAlt);
        Assert.False(b != bAlt);
        Assert.True(a != b);
    }

    [Fact]
    public void Dictionary()
    {
        ColumnInfo New(string propertyName)
        {
            Type type = typeof(ColumnIdentifiers);
            string[] keyProperties = ["Id"];
            IEnumerable<PropertyInfo> keys = keyProperties.Select(property => type.GetProperty(property)).OfType<PropertyInfo>();

            SchemaName? schema = SchemaName.New("dbo");
            TableName table = new("ColumnIdentifiersTable");
            PropertyInfo? property = type.GetProperty(propertyName);
            Assert.NotNull(property);
            return new(schema, table, property, keys);
        }
        ColumnInfo a = New("Id");
        ColumnInfo aAlt = New("Id");
        ColumnInfo b = New("Property");
        ColumnInfo bAlt = New("Property");

        Dictionary<ColumnInfo, int> dictionary = [];

        dictionary.Add(a, 1);
        dictionary.Add(b, 2);

        Assert.Equal(1, dictionary[aAlt]);
        Assert.Equal(2, dictionary[bAlt]);
    }
}
