

using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using System.Reflection;
//IGNORE SPELLING: dbo
namespace Carrigan.SqlTools.Tests.ReflectorCacheTests;
public class ColumnInfoTests
{
    private readonly SchemaName? nullSchema = null;
    private readonly SchemaName emptySchema = new (null);
    private readonly SchemaName dboSchema = new ("dbo");

    private readonly TableName emptyTable = new(null);
    private readonly TableName columnIdentifiersTable = new("ColumnIdentifiersTable");

    private readonly Type columnIdentifiersType = typeof(ColumnIdentifiers);


    [Theory]
    [InlineData(null, "ColumnIdentifiersTable", "Id",
        "[ColumnIdentifiersTable].[Id]", "Id", "Id", 
        "IdParameter",  null, "[ColumnIdentifiersTable].[Id]",
        true, false, false, "[ColumnIdentifiersTable]")]
    public void New(string? schemaName, string tableName, string propertyName, 
        string expectedColumnTag, string  expectedColumnName, string expectedPropertyName, 
        string parameterTag, string? expectedAliasName, string expectedSelectTag,
        bool expectedIsKeyPart, bool expectedIsEncrypted, bool expectedIsKeyVersionField, string expectedTableTag)
    {
        PropertyInfo? idProperty = columnIdentifiersType.GetProperty("Id");
        Assert.NotNull(idProperty);
        IEnumerable<PropertyInfo> columnIdentifiersKeys = [idProperty];

        SchemaName? schema =  SchemaName.New(schemaName);
        TableName table = new(tableName);
        PropertyInfo? property = columnIdentifiersType.GetProperty(propertyName);
        Assert.NotNull(property);
        ColumnInfo columnInfo = new(schema, table, property, columnIdentifiersKeys);

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
        Assert.Equal(expectedIsKeyVersionField, columnInfo.IsKeyVersionField);
    }
}
