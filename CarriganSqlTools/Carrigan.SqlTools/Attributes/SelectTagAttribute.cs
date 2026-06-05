using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Provides shared behavior for attributes that map a model property to a selected SQL column expression.
/// </summary>
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

/// <summary>
/// Provides shared behavior for attributes that map a model property to a selected SQL column expression.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SelectTagAttribute<T> : SelectTagAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTagAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    [ExternalOnly]
    public SelectTagAttribute(string propertyName, string? aliasName = null) :
        this(new(propertyName), AliasName.New(aliasName))
    {
    }

    internal SelectTagAttribute(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTag = SqlToolsReflectorCache<T>
            .GetSelectTag(propertyName, aliasName);
}
