using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

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
    {   //make sure the Type property works.
        Type expected = typeof(SelectsEntity);
        Type actual = InvocationReflectorCache<SelectsEntity>.Type;
        Assert.Equal(expected, actual);
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
        Assert.Equal(expected, InvocationReflectorCache<SelectsEntity>.PropertyInfoCache.Get(new(column)).Name);


    [Fact]
    public void DuplicateResultColumnNames_ThrowsTypeInitializationException()
    {
        TypeInitializationException exception =
            Assert.Throws<TypeInitializationException>(() =>
                InvocationReflectorCache<InvocationDuplicateColumnsEntity>.PropertyInfoCache.Values.ToList());

        Assert.IsType<ArgumentException>(exception.InnerException);
    }

    internal class InvocationDuplicateColumnsEntity
    {
        [Column("Dup")]
        public int A { get; set; }

        [Alias("Dup")]
        public int B { get; set; }
    }


}
