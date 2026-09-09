using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Generators.SqlServer.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Types;
using System.Data;
using System.Reflection;
namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ReflectorCacheTests;
public class ColumnInfoTests
{
    private static readonly SqlServerDialect Dialect = new();

    private static ColumnInfo CreateColumnInfo(string propertyName, string? schemaName = "dbo", string tableName = "ColumnIdentifiersTable")
    {
        Type type = typeof(ColumnIdentifiers);
        IEnumerable<PropertyInfo> keys = [type.GetProperty("Id")!];
        PropertyInfo property = type.GetProperty(propertyName)!;

        return new(SchemaName.New(schemaName), new TableName(tableName), property, keys);
    }

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
        Assert.Equal(expectedColumnName, columnInfo.ColumnName.ToString());
        Assert.Equal(expectedTableTag, columnInfo.ColumnTag.TableTag.ToSql(Dialect));
        Assert.Equal(expectedPropertyName, columnInfo.PropertyInfo.Name);
        Assert.Equal(expectedPropertyName, columnInfo.PropertyName.ToString());
        Assert.Equal(parameterTag, columnInfo.ParameterTag.ToString());

        if(expectedAliasName is null)
        {
            Assert.Null(columnInfo.AliasName);
            Assert.Null(columnInfo.SelectTag.AliasTag);
        }
        else
        {
            Assert.NotNull(columnInfo.AliasName);
            Assert.NotNull(columnInfo.SelectTag.AliasTag);
            Assert.Equal(expectedAliasName, columnInfo.AliasName.ToString());
            Assert.Equal(expectedAliasName, columnInfo.SelectTag.AliasTag.ToString());
        }

        Assert.Equal(expectedSelectTag, columnInfo.SelectTag.ToSql(Dialect));
        Assert.Equal(expectedIsKeyPart, columnInfo.IsKeyPart);
        Assert.Equal(expectedIsEncrypted, columnInfo.IsEncrypted);
        Assert.Equal(expectedIsKeyVersionProperty, columnInfo.IsKeyVersionProperty);

        Assert.False(columnInfo.IsEmpty());

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
