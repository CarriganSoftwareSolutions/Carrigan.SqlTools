using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SelectAttributeTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    [Fact]
    public void GetAttribute_ReturnsAttribute_WhenPropertyHasGenericSelectAttribute()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributes), nameof(ModelWithSelectAttributes.Id));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);
    }

    [Fact]
    public void GetAttribute_ReturnsSelectTag_WhenPropertyHasGenericSelectAttribute()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributes), nameof(ModelWithSelectAttributes.Id));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);
        Assert.NotNull(attribute.SelectTag);
    }

    [Fact]
    public void GetAttribute_ReturnsAttribute_WhenGenericTypeIsNotKnownAtCallSite()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributes), nameof(ModelWithSelectAttributes.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);
        Assert.Equal(typeof(SelectAttribute<SelectSource>), attribute.GetType());
    }

    [Fact]
    public void GetAttribute_ReturnsAttribute_WithExpectedGenericArgument()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributes), nameof(ModelWithSelectAttributes.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.NotNull(attribute);

        Type attributeType = attribute.GetType();

        Assert.True(attributeType.IsGenericType);
        Assert.Equal(typeof(SelectAttribute<>), attributeType.GetGenericTypeDefinition());
        Assert.Equal(typeof(SelectSource), attributeType.GetGenericArguments()[0]);
    }

    [Fact]
    public void GetAttribute_ReturnsNull_WhenPropertyDoesNotHaveSelectAttribute()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributes), nameof(ModelWithSelectAttributes.PlainValue));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.Null(attribute);
    }

    [Fact]
    public void GetAttribute_ReturnsNull_WhenPropertyHasDifferentAttribute()
    {
#pragma warning disable CS0612 // Type or member is obsolete
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributes), nameof(ModelWithSelectAttributes.ObsoleteValue));
#pragma warning restore CS0612 // Type or member is obsolete

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        Assert.Null(attribute);
    }

    [Fact]
    public void GetAttribute_ValidateTagNames()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributes), nameof(ModelWithSelectAttributes.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        SelectTag? selectTag = attribute?.SelectTag;

        Assert.NotNull(selectTag);

        Assert.Equal("[SelectSource]", selectTag.ColumnTag.TableTag);
        Assert.Equal("[SelectSource].[Name]", selectTag.ColumnTag);
        Assert.NotNull(selectTag.AliasTag);
        Assert.Equal("[SelectSource].[Name] AS [SourceName]", selectTag.ToSql(Dialect));
    }

    [Fact]
    public void GetAttribute_WithIdentifiers_ValidateTagNames()
    {
        PropertyInfo propertyInfo = GetProperty(typeof(ModelWithSelectAttributesIdentifier), nameof(ModelWithSelectAttributesIdentifier.Name));

        SelectTagAttribute? attribute = SelectTagAttribute.GetAttribute(propertyInfo);

        SelectTag? selectTag = attribute?.SelectTag;

        Assert.NotNull(selectTag);

        Assert.Equal("[TableIdentifier]", selectTag.ColumnTag.TableTag);
        Assert.Equal("[TableIdentifier].[ColumnIdentifier]", selectTag.ColumnTag);
        Assert.NotNull(selectTag.AliasTag);
        Assert.Equal("[TableIdentifier].[ColumnIdentifier] AS [SourceName]", selectTag.ToSql(Dialect));
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

    private sealed class ModelWithSelectAttributes
    {
        [Select<SelectSource>(nameof(SelectSource.Id))]
        public Guid Id { get; set; }

        [Select<SelectSource>(nameof(SelectSource.Name), "SourceName")]
        public string Name { get; set; } = string.Empty;

        public string PlainValue { get; set; } = string.Empty;

        [Obsolete]
        public string ObsoleteValue { get; set; } = string.Empty;
    }

    private sealed class ModelWithSelectAttributesIdentifier
    {
        [Select<SelectSourceIdentifier>(nameof(SelectSourceIdentifier.Id))]
        public Guid Id { get; set; }

        [Select<SelectSourceIdentifier>(nameof(SelectSourceIdentifier.Name), "SourceName")]
        public string Name { get; set; } = string.Empty;
    }
}