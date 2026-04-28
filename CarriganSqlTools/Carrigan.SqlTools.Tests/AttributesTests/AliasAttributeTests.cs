using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using System.Reflection;

namespace Carrigan.SqlTools.Tests.AttributesTests;
public class AliasAttributeTests
{
    [Fact]
    public void GetCustomAttribute()
    {
        Type type = typeof(AliasEntity);
        PropertyInfo? property = type.GetProperty("TestColumn");
        Assert.NotNull(property);
        IEnumerable<AliasAttribute> aliasAttributes = property.GetCustomAttributes<AliasAttribute>();
        Assert.NotEmpty(aliasAttributes);
        Assert.Equal("AnAlias", aliasAttributes.Single().Name);
    }

    [Fact]
    public void GetCustomAttribute_Null()
    {
        Type type = typeof(AliasEntity);
        PropertyInfo? property = type.GetProperty("NoAlias");
        Assert.NotNull(property);
        IEnumerable<AliasAttribute> aliasAttributes = property.GetCustomAttributes<AliasAttribute>();
        Assert.Empty(aliasAttributes);
    }

    [Fact]
    public void SelectTag()
    {
        SelectTags tags = SelectTags.GetMany<AliasEntity>
        (
            nameof(AliasEntity.Id),
            nameof(AliasEntity.TestColumn),
            nameof(AliasEntity.NoAlias)
        );
        string expected = "[AliasEntity].[Id], [AliasEntity].[TestColumn] AS AnAlias, [AliasEntity].[NoAlias]";

        Assert.Equal(expected, tags.ToSql());
    }

    [Fact]
    public void SqlExample()
    {
        SelectTags tags = SelectTags.GetMany<AliasEntity>
        (
            nameof(AliasEntity.Id),
            nameof(AliasEntity.TestColumn),
            nameof(AliasEntity.NoAlias)
        );

        SqlGenerator<AliasEntity> generator = new();

        string expected = "SELECT [AliasEntity].[Id], [AliasEntity].[TestColumn] AS AnAlias, [AliasEntity].[NoAlias] FROM [AliasEntity]";
        SqlQuery query = generator.Select(tags, null, null, null, null);

        Assert.Equal(expected, query.QueryText);
    }

    [Fact]
    public void Constructor_Null_Exception() =>
    Assert.Throws<ArgumentNullException>(() => new AliasAttribute(null!));

}
