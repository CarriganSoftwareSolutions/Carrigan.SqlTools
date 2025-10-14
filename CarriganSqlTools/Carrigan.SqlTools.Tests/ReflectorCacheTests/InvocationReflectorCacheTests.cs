using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestComparers;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace Carrigan.SqlTools.Tests.ReflectorCacheTests;
public class InvocationReflectorCacheTests
{
    private const string _id = "Id";
    private const string _property = "Property";
    private const string _column = "Column";
    private const string _identifier = "Identifier";
    private const string _identifierOverride = "IdentifierOverride";
    private const string _alias = "Alias";
    private const string _aliasOverride = "AliasOverride";


    private const string _idName = nameof(SelectsEntity.Id);
    private const string _propertyName = nameof(SelectsEntity.Property);
    private const string _columnName = nameof(SelectsEntity.ColumnName);
    private const string _identifierName = nameof(SelectsEntity.IdentifierName);
    private const string _identifierOverrideName = nameof(SelectsEntity.IdentifierOverrideName);
    private const string _aliasName = nameof(SelectsEntity.AliasName);
    private const string _aliasOverrideName = nameof(SelectsEntity.AliasOverrideName);


    [Fact]
    public void TypeTest()
    {   //make sure the Type field works.
        Type expected = typeof(SelectsEntity);
        Type actual = InvocationReflectorCache<SelectsEntity>.Type;
        Assert.Equal(expected, actual);
    }


    [Theory]
    [InlineData(_id)]
    [InlineData(_property)]
    [InlineData(_column)]
    [InlineData(_identifier)]
    [InlineData(_identifierOverride)]
    [InlineData(_alias)]
    [InlineData(_aliasOverride)]
    [InlineData(_id, _column, _property, _identifier, _identifierOverride, _alias, _aliasOverride)]
    public void Exists(params string[] columns) =>
        Assert.True(InvocationReflectorCache<SelectsEntity>.PropertyInfoCache.Exists(columns.Select(column => new ResultColumnName(column))));


    [Theory]
    [InlineData(_id, _idName)]
    [InlineData(_property, _propertyName)]
    [InlineData(_column, _columnName)]
    [InlineData(_identifier, _identifierName)]
    [InlineData(_identifierOverride, _identifierOverrideName)]
    [InlineData(_alias, _aliasName)]
    [InlineData(_aliasOverride, _aliasOverrideName)]
    public void Get(string column, string expected) =>
        Assert.Equal(expected, InvocationReflectorCache<SelectsEntity>.PropertyInfoCache.Get(new(column)).Name);
}
