using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestComparers;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.ReflectorCacheTests;
public class SqlToolsReflectorCacheTests
{
    #region test models

    private sealed class NoAliasModel
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }

    private sealed class AliasedModel
    {
        public int Id { get; init; }

        [Alias("FullName")]
        public string Name { get; init; } = string.Empty;
    }
    #endregion

    [Fact]
    public void TypeTest()
    {   //make sure the Type property works.
        Type expected = typeof(ColumnIdentifiers);
        Type actual = SqlToolsReflectorCache<ColumnIdentifiers>.Type;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SchemaNameNull()
    {
        //Ensure schema names that should be null are null.
        Assert.Null(SqlToolsReflectorCache<ColumnIdentifiers>.SchemaName);
        Assert.Null(SqlToolsReflectorCache<EntityName>.SchemaName);
        Assert.Null(SqlToolsReflectorCache<IdentifierName>.SchemaName);
        Assert.Null(SqlToolsReflectorCache<IdentifierNameOverride>.SchemaName);
        Assert.Null(SqlToolsReflectorCache<TableNameClass>.SchemaName);
    }

    [Fact]
    public void SchemaName()
    {
        //Ensure all the ways to specify schema names work.
        Assert.Equal(new SchemaName("Identifier"), SqlToolsReflectorCache<IdentifierNameOverrideSchema>.SchemaName);
        Assert.Equal(new SchemaName("Identifier"), SqlToolsReflectorCache<IdentifierNameOverrideSchemaOverride>.SchemaName);
        Assert.Equal(new SchemaName("Identifier"), SqlToolsReflectorCache<IdentifierNameSchema>.SchemaName);
        Assert.Equal(new SchemaName("Table"), SqlToolsReflectorCache<TableNameSchema>.SchemaName);
    }

    [Fact]
    public void TableName()
    {
        //Ensure all the ways to specify table names work.
        Assert.Equal(new TableName("ColumnIdentifiers"), SqlToolsReflectorCache<ColumnIdentifiers>.TableName);
        Assert.Equal(new TableName("EntityName"), SqlToolsReflectorCache<EntityName>.TableName);
        Assert.Equal(new TableName("TableNameSchemaTable"), SqlToolsReflectorCache<TableNameSchema>.TableName);
        Assert.Equal(new TableName("IdentifierNameOverrideTable"), SqlToolsReflectorCache<IdentifierNameOverride>.TableName);
        Assert.Equal(new TableName("IdentifierNameOverrideSchemaTable"), SqlToolsReflectorCache<IdentifierNameOverrideSchema>.TableName);
        Assert.Equal(new TableName("IdentifierNameOverrideSchemaOverrideTable"), SqlToolsReflectorCache<IdentifierNameOverrideSchemaOverride>.TableName);
        Assert.Equal(new TableName("IdentifierNameSchemaTable"), SqlToolsReflectorCache<IdentifierNameSchema>.TableName);
        Assert.Equal(new TableName("TableNameTable"), SqlToolsReflectorCache<TableNameClass>.TableName);
        Assert.Equal(new TableName("TableNameSchemaTable"), SqlToolsReflectorCache<TableNameSchema>.TableName);
    }

    [Fact]
    public void KeyColumnInfoColumnNames()
    {   //Ensure all the different way to specify key properties, generate column names correctly with all the ways to specify column names.
        //This ensures we get the columns we expect, KeyColumnInfoCompare ensures all the values are what we expect.
        IEnumerable<ColumnName> GetExpected(params IEnumerable<string> expectedNames) =>
            expectedNames.Select(name => new ColumnName(name));

        IEnumerable<ColumnName> GetActual<T>() =>
            SqlToolsReflectorCache<T>.KeyColumnInfo.Select(item => item.ColumnName);

        Assert.Equal(GetExpected("Id"), GetActual<ColumnIdentifiers>());

        Assert.Equal(GetExpected("Id"), GetActual<Customer>());

        Assert.Equal(GetExpected("Id"), GetActual<EntityWithSchema>());

        Assert.Equal(GetExpected("Id1", "Id2"), GetActual<CompositePrimaryKeyTable>());

        Assert.Equal(GetExpected("Id1", "Id2"), GetActual<CompositeKeyTable>());

        Assert.Equal(GetExpected("Id1", "IdTwo", "IdThree"), GetActual<KeysWithAttributes>());

        Assert.Equal(GetExpected("Id1", "IdTwo", "IdThree"), GetActual<PrimaryKeysWithAttributes>());
    }

    [Fact]
    public void KeyColumnInfoCompare()
    {   //Using the expected property names to compare results from GetColumnsFromProperties to the actual values of KeyColumnInfo
        //The comparer will ensure each critical part is equal.
        IEnumerable<ColumnInfo> GetExpected<T>(params IEnumerable<string> expectedPropertyNames) =>
            SqlToolsReflectorCache<T>.GetColumnsFromProperties(expectedPropertyNames.Select(name => new PropertyName(name)));

        IEnumerable<ColumnInfo> GetActual<T>() =>
            SqlToolsReflectorCache<T>.KeyColumnInfo;

        Assert.Equal(GetExpected<ColumnIdentifiers>("Id"), GetActual<ColumnIdentifiers>(), ColumnInfoAllPropertiesComparer.Instance);

        Assert.Equal(GetExpected<Customer>("Id"), GetActual<Customer>(), ColumnInfoAllPropertiesComparer.Instance);

        Assert.Equal(GetExpected<EntityWithSchema>("Id"), GetActual<EntityWithSchema>(), ColumnInfoAllPropertiesComparer.Instance);

        Assert.Equal(GetExpected<CompositePrimaryKeyTable>("Id1", "Id2"), GetActual<CompositePrimaryKeyTable>(), ColumnInfoAllPropertiesComparer.Instance);

        Assert.Equal(GetExpected<CompositeKeyTable>("Id1", "Id2"), GetActual<CompositeKeyTable>(), ColumnInfoAllPropertiesComparer.Instance);

        Assert.Equal(GetExpected<KeysWithAttributes>("Id1", "Id2", "Id3"), GetActual<KeysWithAttributes>(), ColumnInfoAllPropertiesComparer.Instance);

        Assert.Equal(GetExpected<PrimaryKeysWithAttributes>("Id1", "Id2", "Id3"), GetActual<PrimaryKeysWithAttributes>(), ColumnInfoAllPropertiesComparer.Instance);
    }

    [Fact]
    public void KeyColumnInfoSingleCompare()
    {   //This test makes sure that the KeyColumnInfo gets a correct ColunmInfo to a more manually built single item.
        ColumnInfo column = SqlToolsReflectorCache<ColumnIdentifiers>.KeyColumnInfo.Single();
        Assert.Equal(new ColumnTag(new TableTag(new SqlServerDialect(), null, new TableName("ColumnIdentifiers")), new ColumnName("Id")), column.ColumnTag);
        Assert.Equal(new ColumnName("Id"), column.ColumnName);
        Assert.Equal(new PropertyName("Id"), column.PropertyName);
        Assert.Equal("Id", column.PropertyInfo.Name);
        Assert.Equal(new ColumnTag(new TableTag(new SqlServerDialect(), null, new TableName("ColumnIdentifiers")), new ColumnName("Id")), column.SelectTag.ColumnTag);
        Assert.Null(column.SelectTag.AliasTag);
        Assert.True(column.IsKeyPart);
        Assert.False(column.IsEncrypted);
        Assert.False(column.IsKeyVersionProperty);
    }

    [Fact]
    public void ColumnInfoColumnNames()
    {   //Ensure we get all the expected columns
        IEnumerable<ColumnName> GetExpected(params IEnumerable<string> expectedNames) =>
            expectedNames.Select(name => new ColumnName(name));

        IEnumerable<ColumnName> GetActual<T>() =>
            SqlToolsReflectorCache<T>.ColumnInfo.Select(item => item.ColumnName);

        Assert.Equal(GetExpected("Id", "Property", "Column", "Identifier", "IdentifierOverride"), GetActual<ColumnIdentifiers>());
    }

    [Fact]
    public void ColumnInfoCompare()
    {   //Using the expected property names to compare results from GetColumnsFromProperties to the actual  column info
        IEnumerable<ColumnInfo> GetExpected<T>(params IEnumerable<string> expectedPropertyNames) =>
            SqlToolsReflectorCache<T>.GetColumnsFromProperties(expectedPropertyNames.Select(name => new PropertyName(name)));

        IEnumerable<ColumnInfo> GetActual<T>() =>
            SqlToolsReflectorCache<T>.ColumnInfo;

        Assert.Equal(GetExpected<ColumnIdentifiers>("Id", "Property", "ColumnName", "IdentifierName", "IdentifierOverrideName"), GetActual<ColumnIdentifiers>(), ColumnInfoAllPropertiesComparer.Instance);
    }

    [Fact]
    public void ColumnInfoLessKeysColumnNames()
    {   //Ensure we get all the expected columns
        IEnumerable<ColumnName> GetExpected(params IEnumerable<string> expectedNames) =>
            expectedNames.Select(name => new ColumnName(name));

        IEnumerable<ColumnName> GetActual<T>() =>
            SqlToolsReflectorCache<T>.ColumnInfoLessKeys.Select(item => item.ColumnName);

        Assert.Equal(GetExpected("Property", "Column", "Identifier", "IdentifierOverride"), GetActual<ColumnIdentifiers>());
    }

    [Fact]
    public void ColumnInfoLessKeysCompare()
    {   //Using the expected property names to compare results from GetColumnsFromProperties to the actual  column info
        IEnumerable<ColumnInfo> GetExpected<T>(params IEnumerable<string> expectedPropertyNames) =>
            SqlToolsReflectorCache<T>.GetColumnsFromProperties(expectedPropertyNames.Select(name => new PropertyName(name)));

        IEnumerable<ColumnInfo> GetActual<T>() =>
            SqlToolsReflectorCache<T>.ColumnInfoLessKeys;

        Assert.Equal(GetExpected<ColumnIdentifiers>("Property", "ColumnName", "IdentifierName", "IdentifierOverrideName"), GetActual<ColumnIdentifiers>(), ColumnInfoAllPropertiesComparer.Instance);
    }

    [Fact]
    public void TableTag()
    {
        Assert.Equal(new TableTag(new SqlServerDialect(), null, new TableName("ColumnIdentifiers")), SqlToolsReflectorCache<ColumnIdentifiers>.Table);
        Assert.Equal(new TableTag(new SqlServerDialect(), null, new TableName("EntityName")), SqlToolsReflectorCache<EntityName>.Table);
        Assert.Equal(new TableTag(new SqlServerDialect(), new SchemaName("Table"), new TableName("TableNameSchemaTable")), SqlToolsReflectorCache<TableNameSchema>.Table);
        Assert.Equal(new TableTag(new SqlServerDialect(), null, new TableName("IdentifierNameOverrideTable")), SqlToolsReflectorCache<IdentifierNameOverride>.Table);
        Assert.Equal(new TableTag(new SqlServerDialect(), new SchemaName("Identifier"), new TableName("IdentifierNameOverrideSchemaTable")), SqlToolsReflectorCache<IdentifierNameOverrideSchema>.Table);
        Assert.Equal(new TableTag(new SqlServerDialect(), new SchemaName("Identifier"), new TableName("IdentifierNameOverrideSchemaOverrideTable")), SqlToolsReflectorCache<IdentifierNameOverrideSchemaOverride>.Table);
        Assert.Equal(new TableTag(new SqlServerDialect(), new SchemaName("Identifier"), new TableName("IdentifierNameSchemaTable")), SqlToolsReflectorCache<IdentifierNameSchema>.Table);
        Assert.Equal(new TableTag(new SqlServerDialect(), null, new TableName("TableNameTable")), SqlToolsReflectorCache<TableNameClass>.Table);
    }

    [Fact]
    public void NullKeyVersion() =>
        Assert.Null(SqlToolsReflectorCache<ColumnIdentifiers>.KeyVersionColumnInfo);

    [Fact]
    public void KeyVersionNameNameCompare() =>
        Assert.Equal(new ColumnName("KeyVersion"), SqlToolsReflectorCache<EntityWithEncryption>.KeyVersionColumnInfo!.ColumnName);

    [Fact]
    public void KeyVersion()
    {
        ColumnInfo expected = SqlToolsReflectorCache<EntityWithEncryption>.GetColumnsFromProperties(new PropertyName("KeyVersion")).Single();
        ColumnInfo? actual = SqlToolsReflectorCache<EntityWithEncryption>.KeyVersionColumnInfo;
        Assert.NotNull(actual);
        Assert.Equal(expected, actual, ColumnInfoAllPropertiesComparer.Instance);
    }

    [Fact]
    public void ProcedureTagFromAttribute() =>
        Assert.Equal(new ProcedureTag("schema", "UpdateThing"), SqlToolsReflectorCache<ProcedureExec>.ProcedureTag);

    [Fact]
    public void ProcedureTagFromClass() =>
        Assert.Equal(new ProcedureTag(null, "ColumnIdentifiers"), SqlToolsReflectorCache<ColumnIdentifiers>.ProcedureTag);

    [Fact] 
    public void IsEncrypted_True()
    {
        ColumnInfo encrypted = SqlToolsReflectorCache<EntityWithEncryption>.GetColumnsFromProperties(new PropertyName("SensitiveData")).Single();
        Assert.True(SqlToolsReflectorCache<EntityWithEncryption> .IsEncrypted(encrypted));
        Assert.True(encrypted.IsEncrypted);
    }

    [Fact]
    public void IsEncrypted_False()
    {
        ColumnInfo encrypted = SqlToolsReflectorCache<EntityWithEncryption>.GetColumnsFromProperties(new PropertyName("NotSensitiveData")).Single();
        Assert.False(SqlToolsReflectorCache<EntityWithEncryption>.IsEncrypted(encrypted));
        Assert.False(encrypted.IsEncrypted);
    }

    [Fact]
    public void HasEncryptedColumns_True() =>
        Assert.True(SqlToolsReflectorCache<EntityWithEncryption>.HasEncryptedColumns());

    [Fact]
    public void HasEncryptedColumns_False() =>
        Assert.False(SqlToolsReflectorCache<ColumnIdentifiers>.HasEncryptedColumns());

    [Fact]
    public void HasKeyProperty_False() => 
        Assert.False(SqlToolsReflectorCache<Address>.HasKeyProperty);

    [Fact]
    public void HasKeyProperty_PrimaryKey_True() =>
        Assert.True(SqlToolsReflectorCache<Order>.HasKeyProperty);

    [Fact]
    public void HasKeyProperty_Key_True() =>
        Assert.True(SqlToolsReflectorCache<PhoneModel>.HasKeyProperty);

    [Fact]
    public void HasAliasedColumns_False()
    {
        bool value = SqlToolsReflectorCache<NoAliasModel>.HasAliasedColumns;
        Assert.False(value);
    }

    [Fact]
    public void HasAliasedColumns_True()
    {
        bool value = SqlToolsReflectorCache<AliasedModel>.HasAliasedColumns;
        Assert.True(value);
    }

    [Fact]
    public void SelectTags_AliasModel()
    {
        SelectTags selectTags = SqlToolsReflectorCache<AliasedModel>.SelectTags;

        Assert.Equal(2, selectTags.All().Count());
        Assert.Equal("[AliasedModel].[Id], [AliasedModel].[Name] AS FullName", selectTags.ToSql());
    }

    [Fact]
    public void SelectTags_NoAliasModel()
    {
        SelectTags selectTags = SqlToolsReflectorCache<NoAliasModel>.SelectTags;

        Assert.Equal(2, selectTags.All().Count());
        Assert.Equal("[NoAliasModel].[Id], [NoAliasModel].[Name]", selectTags.ToSql());
    }
}
