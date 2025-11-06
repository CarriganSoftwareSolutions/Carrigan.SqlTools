using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using System.Data;
using System.Reflection;
//IGNORE SPELLING: dbo
namespace Carrigan.SqlTools.Tests.ReflectorCacheTests;
public class ColumnInfoTests
{
    [Theory]
    [InlineData(null, "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "Id", new[] { "Id" },
        "[ColumnIdentifiersTable].[Id]", "Id", "Id", 
        "IdParameter",  null, "[ColumnIdentifiersTable].[Id]",
        true, false, false,  "[ColumnIdentifiersTable]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "Id", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Id]", "Id", "Id",
        "IdParameter", null, "[dbo].[ColumnIdentifiersTable].[Id]",
        true, false, false, "[dbo].[ColumnIdentifiersTable]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "Property", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Property]", "Property", "Property",
        "PropertyParameter", null, "[dbo].[ColumnIdentifiersTable].[Property]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "ColumnName", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Column]", "Column", "ColumnName",
        "ColumnParameter", null, "[dbo].[ColumnIdentifiersTable].[Column]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "IdentifierName", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[Identifier]", "Identifier", "IdentifierName",
        "IdentifierParameter", null, "[dbo].[ColumnIdentifiersTable].[Identifier]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "ColumnIdentifiersTable", typeof(ColumnIdentifiers), "IdentifierOverrideName", new[] { "Id" },
        "[dbo].[ColumnIdentifiersTable].[IdentifierOverride]", "IdentifierOverride", "IdentifierOverrideName",
        "IdentifierOverrideParameter", null, "[dbo].[ColumnIdentifiersTable].[IdentifierOverride]",
        false, false, false, "[dbo].[ColumnIdentifiersTable]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "Id", new[] { "Id" },
        "[dbo].[Test].[Id]", "Id", "Id",
        "Id", null, "[dbo].[Test].[Id]",
        true, false, false, "[dbo].[Test]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "NotSensitiveData", new[] { "Id" },
        "[dbo].[Test].[NotSensitiveData]", "NotSensitiveData", "NotSensitiveData",
        "NotSensitiveData", null, "[dbo].[Test].[NotSensitiveData]",
        false, false, false, "[dbo].[Test]", SqlDbType.NVarChar, "NVARCHAR")]

    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "SensitiveData", new[] { "Id" },
        "[dbo].[Test].[SensitiveData]", "SensitiveData", "SensitiveData",
        "SensitiveData", null, "[dbo].[Test].[SensitiveData]",
        false, true, false, "[dbo].[Test]", SqlDbType.NVarChar, "NVARCHAR")]

    [InlineData("dbo", "Test", typeof(EntityWithEncryption), "KeyVersion", new[] { "Id" },
        "[dbo].[Test].[KeyVersion]", "KeyVersion", "KeyVersion",
        "KeyVersion", null, "[dbo].[Test].[KeyVersion]",
        false, false, true, "[dbo].[Test]", SqlDbType.Int, "INT")]

    [InlineData("dbo", "TableWithAliases", typeof(TableWithAliases), "Id", new[] { "Id" },
        "[dbo].[TableWithAliases].[Id]", "Id", "Id",
        "Id", "TableId", "[dbo].[TableWithAliases].[Id] AS TableId",
        true, false, false, "[dbo].[TableWithAliases]", SqlDbType.Int, "INT")]


    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "Key", new[] { "Key" },
        "[dbo].[NullableTestEntity].[Key]", "Key", "Key",
        "Key", null, "[dbo].[NullableTestEntity].[Key]",
        true, false, false, 
        "[dbo].[NullableTestEntity]", SqlDbType.UniqueIdentifier, "UNIQUEIDENTIFIER")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "IntValue", new[] { "Key" },
        "[dbo].[NullableTestEntity].[IntValue]", "IntValue", "IntValue",
        "IntValue", null, "[dbo].[NullableTestEntity].[IntValue]",
        false, false, false,
        "[dbo].[NullableTestEntity]", SqlDbType.Int, "INT")]
    public void New(string? schemaName, string tableName, Type type, string propertyName, string[] keyProperties,
        string expectedColumnTag, string  expectedColumnName, string expectedPropertyName, 
        string parameterTag, string? expectedAliasName, string expectedSelectTag,
        bool expectedIsKeyPart, bool expectedIsEncrypted, bool expectedIsKeyVersionProperty,
        string expectedTableTag, SqlDbType expectedSqlType, string expectedSqlDeclarationType)
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

        Assert.Equal(expectedSelectTag, columnInfo.SelectTag);
        Assert.Equal(expectedIsKeyPart, columnInfo.IsKeyPart);
        Assert.Equal(expectedIsEncrypted, columnInfo.IsEncrypted);
        Assert.Equal(expectedIsKeyVersionProperty, columnInfo.IsKeyVersionProperty);

        Assert.Equal(expectedColumnTag, implicitString);
        Assert.Equal(expectedColumnTag, explicitString);
        Assert.Equal(expectedColumnTag.GetHashCode(), columnInfo.GetHashCode());

        Assert.False(columnInfo.IsEmpty());

        Assert.Equal(expectedSqlType, columnInfo.SqlType.Type);
        Assert.Equal(expectedSqlDeclarationType, columnInfo.SqlType.TypeDeclaration);
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
    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "LongValue", new[] { "Key" },
        SqlDbType.BigInt, "BIGINT")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "ShortValue", new[] { "Key" },
        SqlDbType.SmallInt, "SMALLINT")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "ByteValue", new[] { "Key" },
        SqlDbType.TinyInt, "TINYINT")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "BoolValue", new[] { "Key" },
        SqlDbType.Bit, "BIT")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DecimalValue", new[] { "Key" },
        SqlDbType.Decimal, "DECIMAL")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "FloatValue", new[] { "Key" },
        SqlDbType.Real, "REAL")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DoubleValue", new[] { "Key" },
        SqlDbType.Float, "FLOAT")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DateTimeValue", new[] { "Key" },
        SqlDbType.DateTime2, "DATETIME2")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "GuidValue", new[] { "Key" },
        SqlDbType.UniqueIdentifier, "UNIQUEIDENTIFIER")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "CharValue", new[] { "Key" },
        SqlDbType.NChar, "NCHAR")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "TimeOnlyValue", new[] { "Key" },
        SqlDbType.Time, "TIME")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DateOnlyValue", new[] { "Key" },
        SqlDbType.Date, "DATE")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "ByteArrayValue", new[] { "Key" },
        SqlDbType.VarBinary, "VARBINARY")]

    [InlineData("dbo", "NullableTestEntity", typeof(NullableTestEntity), "DateTimeOffsetValue", new[] { "Key" },
        SqlDbType.DateTimeOffset, "DATETIMEOFFSET")]


    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "IntValue", new[] { "Key" },
        SqlDbType.Int, "INT")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "LongValue", new[] { "Key" },
        SqlDbType.BigInt, "BIGINT")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "ShortValue", new[] { "Key" },
        SqlDbType.SmallInt, "SMALLINT")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "ByteValue", new[] { "Key" },
        SqlDbType.TinyInt, "TINYINT")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "BoolValue", new[] { "Key" },
        SqlDbType.Bit, "BIT")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DecimalValue", new[] { "Key" },
        SqlDbType.Decimal, "DECIMAL")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "FloatValue", new[] { "Key" },
        SqlDbType.Real, "REAL")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DoubleValue", new[] { "Key" },
        SqlDbType.Float, "FLOAT")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "StringValue", new[] { "Key" },
        SqlDbType.NVarChar, "NVARCHAR")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DateTimeValue", new[] { "Key" },
        SqlDbType.DateTime2, "DATETIME2")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "GuidValue", new[] { "Key" },
        SqlDbType.UniqueIdentifier, "UNIQUEIDENTIFIER")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "CharValue", new[] { "Key" },
        SqlDbType.NChar, "NCHAR")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "ByteArrayValue", new[] { "Key" },
        SqlDbType.VarBinary, "VARBINARY")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "TimeOnlyValue", new[] { "Key" },
        SqlDbType.Time, "TIME")]

    [InlineData("dbo", "StandardEntity", typeof(StandardEntity), "DateOnlyValue", new[] { "Key" },
        SqlDbType.Date, "DATE")]

    [InlineData("dbo", "StandardEntity", typeof(SpecialEntity), "EnumValueString", new[] { "Key" },
        SqlDbType.Int, "INT")]

    [InlineData("dbo", "StandardEntity", typeof(SpecialEntity), "EnumValueInt", new[] { "Key" },
        SqlDbType.Int, "INT")]

    [InlineData("dbo", "StandardEntity", typeof(SpecialEntity), "NullableEnumValue", new[] { "Key" },
        SqlDbType.Int, "INT")]
    public void SqlDbTyp(string? schemaName, string tableName, Type type, string propertyName, string[] keyProperties,
        SqlDbType expectedSqlType, string expectedSqlDeclarationType)
    {
        IEnumerable<PropertyInfo> keys = keyProperties.Select(property => type.GetProperty(property)).OfType<PropertyInfo>();

        SchemaName? schema = SchemaName.New(schemaName);
        TableName table = new(tableName);
        PropertyInfo? property = type.GetProperty(propertyName);
        Assert.NotNull(property);
        ColumnInfo columnInfo = new(schema, table, property, keys);

        Assert.Equal(expectedSqlType, columnInfo.SqlType.Type);
        Assert.Equal(expectedSqlDeclarationType, columnInfo.SqlType.TypeDeclaration);
    }


    [Theory]
    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "NChar", new string[] { },
        SqlDbType.NChar, "NCHAR", 4000, null, null, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "Text", new string[] { },
        SqlDbType.Text, "TEXT", null, null, null, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "VarChar", new string[] { },
        SqlDbType.VarChar, "VARCHAR", 8000, null, null, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "NVarChar", new string[] { },
        SqlDbType.NVarChar, "NVARCHAR", 4000, null, null, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "Binary", new string[] { },
        SqlDbType.Binary, "BINARY", 4000, null, null, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "VarBinary", new string[] { },
        SqlDbType.VarBinary, "VARBINARY", 4000, null, null, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "VarBinaryMax", new string[] { },
        SqlDbType.VarBinary, "VARBINARYMAX(MAX)", null, null, null, true)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "VarBinaryDefault", new string[] { },
        SqlDbType.VarBinary, "VARBINARYDEFAULT", null, null, null, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "Decimal", new string[] { },
        SqlDbType.Decimal, "DECIMAL", null, 12, 13, false)]

    [InlineData("dbo", "SqlTypeOverRiderEntity", typeof(SqlTypeOverRiderEntity), "DateTime2", new string[] { },
        SqlDbType.DateTime2, "DATETIME2", null, null, 7, false)]
    public void SqlDbTypWithValues(string? schemaName, string tableName, Type type, string propertyName, string[] keyProperties,
        SqlDbType expectedSqlType, string expectedSqlDeclarationType, int? expectedSize, byte? expectedPrecision, byte? expectedScale,
        bool expectedUseMax)
    {
        IEnumerable<PropertyInfo> keys = keyProperties.Select(property => type.GetProperty(property)).OfType<PropertyInfo>();

        SchemaName? schema = SchemaName.New(schemaName);
        TableName table = new(tableName);
        PropertyInfo? property = type.GetProperty(propertyName);
        Assert.NotNull(property);
        ColumnInfo columnInfo = new(schema, table, property, keys);

        Assert.Equal(expectedSqlType, columnInfo.SqlType.Type);
        Assert.Equal(expectedSqlDeclarationType, columnInfo.SqlType.TypeDeclaration);

        Assert.Equal(expectedSize, columnInfo.SqlType.Size);
        Assert.Equal(expectedPrecision, columnInfo.SqlType.Precision);
        Assert.Equal(expectedScale, columnInfo.SqlType.Scale);
        Assert.Equal(expectedUseMax, columnInfo.SqlType.UseMax);
    }
}
