using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using System.Reflection;

namespace Carrigan.SqlTools.Tests.ReflectorCacheTests;
public class PropertyInfoCacheTests
{
    private readonly Type _type = typeof(SelectsEntity);

    private readonly IEnumerable<Tuple<ResultColumnName, PropertyInfo>> _properties;

    private readonly PropertyInfoCache<Address> _cache;

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

    private const string _invalid = "INVALID_PROPERTY";
     PropertyInfo GetPropertyInfo(string propertyName) =>
            (_type.GetProperty(propertyName) ?? throw new InvalidPropertyException<SelectsEntity>(new PropertyName(propertyName)));
    public PropertyInfoCacheTests()
    {
        _properties =
        [
            new(new(_id),  GetPropertyInfo(nameof(SelectsEntity.Id))),
            new(new(_property),  GetPropertyInfo(nameof(SelectsEntity.Property))),
            new(new(_column),  GetPropertyInfo(nameof(SelectsEntity.ColumnName))),
            new(new(_identifier),  GetPropertyInfo(nameof(SelectsEntity.IdentifierName))),
            new(new (_identifierOverride),  GetPropertyInfo(nameof(SelectsEntity.IdentifierOverrideName))),
            new(new (_alias),  GetPropertyInfo(nameof(SelectsEntity.AliasName))),
            new(new (_aliasOverride),  GetPropertyInfo(nameof(SelectsEntity.AliasOverrideName)))
        ];
        _cache = new(_properties);
    }

    [Theory]
    [InlineData(_id, _idName)]
    [InlineData(_property, _propertyName)]
    [InlineData(_column, _columnName)]
    [InlineData(_identifier, _identifierName)]
    [InlineData(_identifierOverride, _identifierOverrideName)]
    [InlineData(_alias, _aliasName)]
    [InlineData(_aliasOverride, _aliasOverrideName)]
    public void Get(string column, string expected) => 
        Assert.Equal(expected, _cache.Get(new(column)).Name);

    [Fact]
    public void Get_InvalidException() =>
        Assert.Throws<InvalidResultColumnNameException<Address>>(() =>  _cache.Get(new(_invalid)));

    [Theory]
    [InlineData(_id, _idName)]
    [InlineData(_property, _propertyName)]
    [InlineData(_column, _columnName)]
    [InlineData(_identifier, _identifierName)]
    [InlineData(_identifierOverride, _identifierOverrideName)]
    [InlineData(_alias, _aliasName)]
    [InlineData(_aliasOverride, _aliasOverrideName)]
    public void GetMany_Single(string column, string expected) => 
        Assert.Equal(expected, _cache.GetMany(new ResultColumnName(column)).Single().Name);


    [Theory]
    [InlineData(new[] { _id }, new[] { _idName })]
    [InlineData(new[] { _property }, new[] { _propertyName })]
    [InlineData(new[] { _column }, new[] { _columnName })]
    [InlineData(new[] { _identifier }, new[] { _identifierName })]
    [InlineData(new[] { _identifierOverride }, new[] { _identifierOverrideName })]
    [InlineData(new[] { _alias }, new[] { _aliasName })]
    [InlineData(new[] { _aliasOverride }, new[] { _aliasOverrideName })]
    [InlineData(new[] { _id, _property, _column, _identifier, _identifierOverride, _alias, _aliasOverride },
        new[] { _idName, _propertyName, _columnName, _identifierName, _identifierOverrideName, _aliasName, _aliasOverrideName })]
    public void GetMany(string[] columns, string[] expected)
    {
        string[] actual = [.. _cache.GetMany(columns.Select(column => new ResultColumnName(column))).Select(property => property.Name)];
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetMany_InvalidException() =>
        Assert.Throws<InvalidResultColumnNameException<Address>>(() => _cache.GetMany(new ResultColumnName(_invalid)));

    [Fact]
    public void GetExceptionForInvalidProperties_Null() => 
        Assert.Null(_cache.GetExceptionForInvalidProperties(new ResultColumnName(_id)));

    [Fact]
    public void GetExceptionForInvalidProperties_NotNull() =>
        Assert.NotNull(_cache.GetExceptionForInvalidProperties(new ResultColumnName(_invalid)));

    [Fact]
    public void GetMany_MultipleInvalidException()
    {
        string invalid1 = "INVALID_PROPERTY_1";
        string invalid2 = "INVALID_PROPERTY_2";

        InvalidResultColumnNameException<Address> exception =
            Assert.Throws<InvalidResultColumnNameException<Address>>
            (
                () => _cache.GetMany
                (
                    [
                        new ResultColumnName(invalid1),
                    new ResultColumnName(invalid2)
                    ]
                )
            );

        Assert.Contains(invalid1, exception.Message);
        Assert.Contains(invalid2, exception.Message);
    }

    [Fact]
    public void Get_NullKey_ThrowsArgumentNullException() =>
    Assert.Throws<ArgumentNullException>(() => _cache.Get(null!));

    [Fact]
    public void GetMany_NullParamEnumerable_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => _cache.GetMany(null!));
}
