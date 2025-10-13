

using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tests.TestEntities;
using System.Reflection;

namespace Carrigan.SqlTools.Tests.RegularExpressionTests;
public class PropertyInfoCacheTests
{

    private readonly List<Tuple<PropertyInfo, PropertyName>> _properties;
    private readonly PropertyInfoCache<Address, PropertyName> _cache;

    private readonly PropertyName _streetPropertyName;
    private readonly PropertyName _cityPropertyName;
    private readonly PropertyName _postalCodePropertyName;

    private readonly PropertyName _invalid;
    public PropertyInfoCacheTests()
    {
        Type addressType = typeof(Address);
        _properties = [];
        _streetPropertyName = new(nameof(Address.Street));
        _cityPropertyName = new(nameof(Address.City));
        _postalCodePropertyName = new(nameof(Address.PostalCode));
        _invalid = new("INVALID_PROPERTY");

        _properties.Add(new(addressType.GetProperty(nameof(Address.Street))!, new PropertyName(nameof(Address.Street))));
        _properties.Add(new(addressType.GetProperty(nameof(Address.City))!, new PropertyName(nameof(Address.City))));
        _properties.Add(new(addressType.GetProperty(nameof(Address.PostalCode))!, new PropertyName(nameof(Address.PostalCode))));

        _cache = new(_properties);
    }

    [Fact]
    public void Exists()
    {
        Assert.True(_cache.Exists(_streetPropertyName));
        Assert.True(_cache.Exists(_cityPropertyName));
        Assert.True(_cache.Exists(_postalCodePropertyName));
        Assert.True(_cache.Exists(_streetPropertyName, _cityPropertyName));
        Assert.True(_cache.Exists(_cityPropertyName, _postalCodePropertyName));
        Assert.True(_cache.Exists(_streetPropertyName, _postalCodePropertyName));
        Assert.True(_cache.Exists(_streetPropertyName, _cityPropertyName, _postalCodePropertyName));
    }
    [Fact]
    public void DoesNotExist() => 
        Assert.False(_cache.Exists(_invalid));

    [Fact]
    public void Get()
    {
        Assert.Equal(_streetPropertyName, _cache.Get(_streetPropertyName));
        Assert.Equal(_cityPropertyName, _cache.Get(_cityPropertyName));
        Assert.Equal(_postalCodePropertyName, _cache.Get(_postalCodePropertyName));
    }

    [Fact]
    public void Get_InvalidException() =>
        Assert.Throws<InvalidPropertyException<Address>>(() =>  _cache.Get(_invalid));

    [Fact]
    public void GetMany()
    {
        Assert.Equal([_streetPropertyName], _cache.GetMany(_streetPropertyName));
        Assert.Equal([_cityPropertyName], _cache.GetMany(_cityPropertyName));
        Assert.Equal([_postalCodePropertyName], _cache.GetMany(_postalCodePropertyName));
        Assert.Equal([_streetPropertyName, _cityPropertyName], _cache.GetMany(_streetPropertyName, _cityPropertyName));
        Assert.Equal([_streetPropertyName, _postalCodePropertyName], _cache.GetMany(_streetPropertyName, _postalCodePropertyName));
        Assert.Equal([_cityPropertyName, _postalCodePropertyName], _cache.GetMany(_cityPropertyName, _postalCodePropertyName));
        Assert.Equal([_streetPropertyName, _cityPropertyName, _postalCodePropertyName], _cache.GetMany(_streetPropertyName, _cityPropertyName, _postalCodePropertyName));
    }

    [Fact]
    public void GetMany_InvalidException() =>
        Assert.Throws<InvalidPropertyException<Address>>(() => _cache.GetMany(_invalid));

    [Fact]
    public void GetExceptionForInvalidProperties_Null() => 
        Assert.Null(_cache.GetExceptionForInvalidProperties(_streetPropertyName));

    [Fact]
    public void GetExceptionForInvalidProperties_NotNull() =>
        Assert.NotNull(_cache.GetExceptionForInvalidProperties(_invalid));
}
