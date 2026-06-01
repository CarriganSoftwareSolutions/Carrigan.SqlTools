using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Types;
using System.Data;
using System.Reflection;
//IGNORE SPELLING: dbo
namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ReflectorCacheTests;
public class ColumnInfoTests
{
    private static readonly SqlServerDialect Dialect = new();

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
        "Id", "TableId", "[dbo].[TableWithAliases].[Id] AS [TableId]",
        true, false, false, "[dbo].[TableWithAliases]")]


    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "Key", new[] { "Key" },
        "[dbo].[NullableTestEntity].[Key]", "Key", "Key",
        "Key", null, "[dbo].[NullableTestEntity].[Key]",
        true, false, false, 
        "[dbo].[NullableTestEntity]")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "IntValue", new[] { "Key" },
        "[dbo].[NullableTestEntity].[IntValue]", "IntValue", "IntValue",
        "IntValue", null, "[dbo].[NullableTestEntity].[IntValue]",
        false, false, false,
        "[dbo].[NullableTestEntity]")]
    public void New(string? schemaName, string tableName, Type type, string propertyName, string[] keyProperties,
        string expectedColumnTag, string expectedColumnName, string expectedPropertyName,
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

        Assert.Equal(expectedColumnTag, columnInfo.ColumnTag.ToSql(Dialect));
        Assert.Equal(expectedColumnName, columnInfo.ColumnName);
        Assert.Equal(expectedTableTag, columnInfo.ColumnTag.TableTag.ToSql(Dialect));
        Assert.Equal(expectedPropertyName, columnInfo.PropertyInfo.Name);
        Assert.Equal(expectedPropertyName, columnInfo.PropertyName);
        Assert.Equal(parameterTag, columnInfo.ParameterTag);

        if(expectedAliasName is null)
        {
            Assert.Null(columnInfo.AliasName);
            Assert.Null(columnInfo.SelectTag.AliasTag);
        }
        else
        {
            Assert.NotNull(columnInfo.AliasName);
            Assert.NotNull(columnInfo.SelectTag.AliasTag);
            Assert.Equal(expectedAliasName, columnInfo.AliasName);
            Assert.Equal(expectedAliasName, columnInfo.SelectTag.AliasTag);
        }

        Assert.Equal(expectedSelectTag, columnInfo.SelectTag.ToSql(Dialect));
        Assert.Equal(expectedIsKeyPart, columnInfo.IsKeyPart);
        Assert.Equal(expectedIsEncrypted, columnInfo.IsEncrypted);
        Assert.Equal(expectedIsKeyVersionProperty, columnInfo.IsKeyVersionProperty);

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


    [Theory]
    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "LongValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "ShortValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "ByteValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "BoolValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DecimalValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "FloatValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DoubleValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DateTimeValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "GuidValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "CharValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "TimeOnlyValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DateOnlyValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "ByteArrayValue", new[] { "Key" })]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DateTimeOffsetValue", new[] { "Key" })]


    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "IntValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "LongValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "ShortValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "ByteValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "BoolValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DecimalValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "FloatValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DoubleValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "StringValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DateTimeValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "GuidValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "CharValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "ByteArrayValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "TimeOnlyValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DateOnlyValue", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(SpecialEntity), "EnumValueString", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(SpecialEntity), "EnumValueInt", new[] { "Key" })]

    [InlineData("dbo", "StandardEntity", typeof(SpecialEntity), "NullableEnumValue", new[] { "Key" })]
    public void SqlDbTyp(string? schemaName, string tableName, Type type, string propertyName, string[] keyProperties)
    {
        IEnumerable<PropertyInfo> keys = keyProperties.Select(property => type.GetProperty(property)).OfType<PropertyInfo>();

        SchemaName? schema = SchemaName.New(schemaName);
        TableName table = new(tableName);
        PropertyInfo? property = type.GetProperty(propertyName);
        Assert.NotNull(property);
        ColumnInfo columnInfo = new(schema, table, property, keys);

    }

    [Theory]
    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "NChar", new string[] { },
        "NCHAR", "NCHAR(4000)", 4000, false, true, true, null, null, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "Text", new string[] { },
        "TEXT", "TEXT", null, false, false, false, null, null, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "VarChar", new string[] { },
        "VARCHAR", "VARCHAR(8000)", 8000, false, false, false, null, null, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "NVarChar", new string[] { },
        "NVARCHAR", "NVARCHAR(4000)", 4000, false, true, false, null, null, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "Binary", new string[] { },
        "BINARY", "BINARY(4000)", 4000, false, null, true, null, null, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "VarBinary", new string[] { },
        "VARBINARY", "VARBINARY(4000)", 4000, false, null, false, null, null, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "VarBinaryMax", new string[] { },
        "VARBINARY", "VARBINARY(MAX)", null, true, null, false, null, null, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "Decimal", new string[] { },
        "DECIMAL", "DECIMAL(18, 4)", null, null, null, null, (byte)18, (byte)4, null)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "DateTime2", new string[] { },
        "DATETIME2", "DATETIME2(7)", null, null, null, null, null, null, (byte)7)]
    public void FieldProperties_WithOverrides(string? schemaName, string tableName, Type type, string propertyName, string[] keyProperties,
        string expectedProviderTypeName, string expectedDeclarationType, int? expectedLength, bool? expectedIsMax,
        bool? expectedIsUnicode, bool? expectedIsFixedLength, byte? expectedPrecision, byte? expectedScale,
        byte? expectedFractionalSecondsPrecision)
    {
        IEnumerable<PropertyInfo> keys =
            keyProperties
                .Select(property => type.GetProperty(property))
                .OfType<PropertyInfo>();

        SchemaName? schema = SchemaName.New(schemaName);
        TableName table = new(tableName);
        PropertyInfo? property = type.GetProperty(propertyName);
        Assert.NotNull(property);

        ColumnInfo columnInfo = new(schema, table, property, keys);

        Assert.NotNull(columnInfo.FieldProperties);
        FieldProperties fieldProperties = columnInfo.FieldProperties!;

        Assert.Equal(expectedProviderTypeName, fieldProperties.ProviderTypeName);
        Assert.Equal(expectedLength, fieldProperties.Length);
        Assert.Equal(expectedIsMax, fieldProperties.IsMax);
        Assert.Equal(expectedIsUnicode, fieldProperties.IsUnicode);
        Assert.Equal(expectedIsFixedLength, fieldProperties.IsFixedLength);
        Assert.Equal(expectedPrecision, fieldProperties.Precision);
        Assert.Equal(expectedScale, fieldProperties.Scale);
        Assert.Equal(expectedFractionalSecondsPrecision, fieldProperties.FractionalSecondsPrecision);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal($"{expectedDeclarationType} NOT NULL", Dialect.RenderFieldProperties(fieldProperties));
    }

    [Fact]
    public void New_TableNameNull_Exception()
    {
        Type type = typeof(ColumnIdentifiers);
        PropertyInfo property = type.GetProperty("Id")!;
        IEnumerable<PropertyInfo> keys = [property];

        Assert.Throws<ArgumentNullException>(() => new ColumnInfo(null, null!, property, keys));
    }

    [Fact]
    public void New_PropertyInfoNull_Exception()
    {
        Type type = typeof(ColumnIdentifiers);
        IEnumerable<PropertyInfo> keys = [type.GetProperty("Id")!];

        Assert.Throws<ArgumentNullException>(() => new ColumnInfo(null, new("ColumnIdentifiersTable"), null!, keys));
    }

    [Fact]
    public void New_KeysNull_Exception()
    {
        Type type = typeof(ColumnIdentifiers);
        PropertyInfo property = type.GetProperty("Id")!;

        Assert.Throws<ArgumentNullException>(() => new ColumnInfo(null, new("ColumnIdentifiersTable"), property, null!));
    }

    [Fact]
    public void ImplicitString_Null_Exception()
    {
        ColumnInfo? value = null;
        Assert.Throws<ArgumentNullException>(() => _ = (string)value!);
    }

}
