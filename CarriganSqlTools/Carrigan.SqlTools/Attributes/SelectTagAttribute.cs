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
    /// <summary>
    /// Gets or initializes the fully qualified source column selected by this attribute.
    /// </summary>
    internal ColumnTag? ColumnTag { get; init; }

    /// <summary>
    /// Gets or initializes the alias applied by this attribute.
    /// </summary>
    internal AliasTag? AliasTag { get; init; }

    /// <summary>
    /// Gets or initializes the reflected SELECT projection metadata created by the concrete generic attribute.
    /// </summary>
    internal SelectTagBase? SelectTag { get; init; }

    /// <summary>
    /// Gets the first generic SELECT projection attribute applied to a model property.
    /// </summary>
    /// <param name="propertyInfo">The reflected property to inspect for SELECT projection metadata.</param>
    /// <returns>
    /// The applied <see cref="SelectTagAttribute{T}"/> instance when one is present; otherwise, <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown by reflection when <paramref name="propertyInfo"/> is <see langword="null"/>.
    /// </exception>
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
    /// Initializes a new instance of the <see cref="SelectTagAttribute{T}"/> class from a model property name and optional SQL result alias.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column selected from <typeparamref name="T"/>.</param>
    /// <param name="aliasName">The optional SQL result-column alias to apply to the selected property.</param>
    [ExternalOnly]
    public SelectTagAttribute(string propertyName, string? aliasName = null) :
        this(new(propertyName), AliasName.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTagAttribute{T}"/> class from already wrapped identifier values.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column selected from <typeparamref name="T"/>.</param>
    /// <param name="aliasName">The optional SQL result-column alias to apply.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid selected property on <typeparamref name="T"/>.
    /// </exception>
    internal SelectTagAttribute(PropertyName propertyName, AliasName? aliasName = null)
    {
        ColumnInfo columnInfo = SqlToolsReflectorCache<T>.GetSelectColumnInfo(propertyName);
        ColumnTag columnTag = columnInfo.SelectColumnTag;
        AliasTag? aliasTag = aliasName is null
            ? columnInfo.SelectAliasTag
            : AliasTag.New(aliasName);

        ColumnTag = columnTag;
        AliasTag = aliasTag;
        SelectTag = new ReflectedSelectTag(columnTag, aliasTag);
    }
}
