using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SelectTagAttributeTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    [Fact]
    public void GetAttribute_ReturnsAttribute_WhenPropertyHasGenericSelectTagAttribute()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributes), nameof(ModelWithSelectTagAttributes.Id));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);
    }

    [Fact]
    public void GetAttribute_ReturnsSelectTag_WhenPropertyHasGenericSelectTagAttribute()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributes), nameof(ModelWithSelectTagAttributes.Id));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);
        Assert.NotNull(attribute.SelectTag);
    }

    [Fact]
    public void GetAttribute_ReturnsAttribute_WhenGenericTypeIsNotKnownAtCallSite()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributes), nameof(ModelWithSelectTagAttributes.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);
        Assert.Equal(typeof(SelectTagAttribute<SelectSource>), attribute.GetType());
    }

    [Fact]
    public void GetAttribute_ReturnsAttribute_WithExpectedGenericArgument()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributes), nameof(ModelWithSelectTagAttributes.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);

        Type attributeType = attribute.GetType();

        Assert.True(attributeType.IsGenericType);
        Assert.Equal(typeof(SelectTagAttribute<>), attributeType.GetGenericTypeDefinition());
        Assert.Equal(typeof(SelectSource), attributeType.GetGenericArguments()[0]);
    }

    [Fact]
    public void GetAttribute_ReturnsNull_WhenPropertyDoesNotHaveSelectTagAttribute()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributes), nameof(ModelWithSelectTagAttributes.PlainValue));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.Null(attribute);
    }

    [Fact]
    public void GetAttribute_ReturnsNull_WhenPropertyHasDifferentAttribute()
    {
#pragma warning disable CS0612 // Type or member is obsolete
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributes), nameof(ModelWithSelectTagAttributes.ObsoleteValue));
#pragma warning restore CS0612 // Type or member is obsolete

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.Null(attribute);
    }

    [Fact]
    public void GetAttribute_ValidateTagNames()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributes), nameof(ModelWithSelectTagAttributes.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        SelectTagBase? selectTag = attribute?.SelectTag;

        Assert.NotNull(selectTag);

        Assert.NotNull(attribute?.ColumnTag);
        Assert.Equal("[SelectSource]", attribute.ColumnTag.TableTag.ToSql(Dialect));
        Assert.Equal("SelectSource", attribute.ColumnTag.TableTag);
        Assert.Equal("[SelectSource].[Name]", attribute.ColumnTag.ToSql(Dialect));
        Assert.Equal("SelectSource.Name", attribute.ColumnTag);
        Assert.NotNull(selectTag.AliasTag);
        Assert.Equal("[SelectSource].[Name] AS [SourceName]", selectTag.ToSql(Dialect));
        Assert.Equal("SelectSource.Name AS SourceName", selectTag);
    }

    [Fact]
    public void GetAttribute_WithIdentifiers_ValidateTagNames()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectTagAttributesIdentifier), nameof(ModelWithSelectTagAttributesIdentifier.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        SelectTagBase? selectTag = attribute?.SelectTag;

        Assert.NotNull(selectTag);

        Assert.NotNull(attribute?.ColumnTag);
        Assert.Equal("[TableIdentifier]", attribute.ColumnTag.TableTag.ToSql(Dialect));
        Assert.Equal("[TableIdentifier].[ColumnIdentifier]", attribute.ColumnTag.ToSql(Dialect));
        Assert.Equal("[TableIdentifier].[ColumnIdentifier] AS [SourceName]", selectTag.ToSql(Dialect));

        Assert.Equal("TableIdentifier", attribute.ColumnTag.TableTag);
        Assert.Equal("TableIdentifier.ColumnIdentifier", attribute.ColumnTag);
        Assert.NotNull(selectTag.AliasTag);
        Assert.Equal("TableIdentifier.ColumnIdentifier AS SourceName", selectTag);
    }

    private static PropertyInfo GetProperty(Type type, string propertyName) =>
        type.GetProperty(propertyName)
        ?? throw new InvalidOperationException($"Could not find property '{propertyName}'.");

    private sealed class SelectSource
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
    [Identifier("TableIdentifier")]
    private sealed class SelectSourceIdentifier
    {
        public Guid Id { get; set; }

        [Identifier("ColumnIdentifier")]
        [Alias("AnAlias")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class ModelWithSelectTagAttributes
    {
        [SelectTag<SelectSource>(nameof(SelectSource.Id))]
        public Guid Id { get; set; }

        [SelectTag<SelectSource>(nameof(SelectSource.Name), "SourceName")]
        public string Name { get; set; } = string.Empty;

        public string PlainValue { get; set; } = string.Empty;

        [Obsolete]
        public string ObsoleteValue { get; set; } = string.Empty;
    }

    private sealed class ModelWithSelectTagAttributesIdentifier
    {
        [SelectTag<SelectSourceIdentifier>(nameof(SelectSourceIdentifier.Id))]
        public Guid Id { get; set; }

        [SelectTag<SelectSourceIdentifier>(nameof(SelectSourceIdentifier.Name), "SourceName")]
        public string Name { get; set; } = string.Empty;
    }
}