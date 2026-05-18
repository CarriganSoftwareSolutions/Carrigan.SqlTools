using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Attributes;

internal abstract class SelectTagAttribute : Attribute
{
    internal SelectTag? SelectTag { get; init; }

    internal static SelectTagAttribute? GetAttribute(PropertyInfo propertyInfo) =>
        propertyInfo
            .GetCustomAttributes(inherit: true)
            .OfType<SelectTagAttribute>()
            .FirstOrDefault(attribute =>
                attribute.GetType().IsGenericType &&
                attribute.GetType().GetGenericTypeDefinition() == typeof(SelectAttribute<>));
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class SelectAttribute<T> : SelectTagAttribute
{
    [ExternalOnly]
    public SelectAttribute(string propertyName, string? aliasName = null) :
        this(new(propertyName), AliasName.New(aliasName))
    {
    }

    internal SelectAttribute(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTag = SqlToolsReflectorCache<T>
            .GetSelectTag(propertyName, aliasName);
}