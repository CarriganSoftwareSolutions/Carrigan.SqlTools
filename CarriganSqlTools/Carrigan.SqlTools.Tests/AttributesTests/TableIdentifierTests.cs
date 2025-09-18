using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.IdentifierTests;
public  class TableIdentifierTests
{
    private static readonly SqlGenerator<EntityName> _entityNameSqlGenerator = new();
    private static readonly SqlGenerator<IdentifierName> _identifierNameSqlGenerator = new();
    private static readonly SqlGenerator<IdentifierNameOverride> _identifierNameOverrideSqlGenerator = new();
    private static readonly SqlGenerator<IdentifierNameOverrideSchema> _identifierNameOverrideSchemaSqlGenerator = new();
    private static readonly SqlGenerator<IdentifierNameOverrideSchemaOverride> _identifierNameOverrideSchemaOverrideSqlGenerator = new();
    private static readonly SqlGenerator<IdentifierNameSchema> _identifierNameSchemaSqlGenerator = new();
    private static readonly SqlGenerator<TableName> _tableNameSqlGenerator = new();
    private static readonly SqlGenerator<TableNameSchema> _tableNameSchemaSqlGenerator = new();

    [Fact]
    public void EntityNameTest()
    {
        SqlQuery query = _entityNameSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [EntityName].* FROM [EntityName]", queryText);
    }

    [Fact]
    public void IdentifierNameNameTest()
    {
        SqlQuery query = _identifierNameSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [IdentifierNameTable].* FROM [IdentifierNameTable]", queryText);
    }

    [Fact]
    public void IdentifierNameOverrideNameTest()
    {
        SqlQuery query = _identifierNameOverrideSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [IdentifierNameOverrideTable].* FROM [IdentifierNameOverrideTable]", queryText);
    }

    [Fact]
    public void IdentifierNameOverrideSchemaNameTest()
    {
        SqlQuery query = _identifierNameOverrideSchemaSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [Identifier].[IdentifierNameOverrideSchemaTable].* FROM [Identifier].[IdentifierNameOverrideSchemaTable]", queryText);
    }

    [Fact]
    public void IdentifierNameOverrideSchemaOverrideNameTest()
    {
        SqlQuery query = _identifierNameOverrideSchemaOverrideSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [Identifier].[IdentifierNameOverrideSchemaOverrideTable].* FROM [Identifier].[IdentifierNameOverrideSchemaOverrideTable]", queryText);
    }

    [Fact]
    public void IdentifierNameSchemaNameTest()
    {
        SqlQuery query = _identifierNameSchemaSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [Identifier].[IdentifierNameSchemaTable].* FROM [Identifier].[IdentifierNameSchemaTable]", queryText);
    }

    [Fact]
    public void TableNameNameTest()
    {
        SqlQuery query = _tableNameSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [TableNameTable].* FROM [TableNameTable]", queryText);
    }

    [Fact]
    public void TableNameSchemaNameTest()
    {
        SqlQuery query = _tableNameSchemaSqlGenerator.SelectAll();
        string queryText = query.QueryText;
        Assert.Equal("SELECT [Table].[TableNameSchemaTable].* FROM [Table].[TableNameSchemaTable]", queryText);
    }
}
