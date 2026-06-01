using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Attributes;

public abstract class SelectTagAttribute : Attribute
{
    internal SelectTagBase? SelectTag { get; init; }

    internal static SelectTagAttribute? GetAttribute(PropertyInfo propertyInfo) =>
        propertyInfo
            .GetCustomAttributes(inherit: true)
            .OfType<SelectTagAttribute>()
            .FirstOrDefault(attribute =>
                attribute.GetType().IsGenericType &&
                attribute.GetType().GetGenericTypeDefinition() == typeof(SelectTagAttribute<>));
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SelectTagAttribute<T> : SelectTagAttribute
{
    [ExternalOnly]
    public SelectTagAttribute(string propertyName, string? aliasName = null) :
        this(new(propertyName), AliasName.New(aliasName))
    {
    }

    internal SelectTagAttribute(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTag = SqlToolsReflectorCache<T>
            .GetSelectTag(propertyName, aliasName);
}
