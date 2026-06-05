using Carrigan.SqlTools.Types;
using System.Reflection;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// A base attribute that allows a property to override the SQL field metadata used during SQL generation.
/// </summary>
/// <remarks>
/// This attribute defines metadata for the <b>property</b>, and that property represents an SQL column in the data model.
/// The derived attribute supplies <see cref="FieldProperties"/> consumed by the SQL generator when emitting SQL and materializing parameters.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class SqlTypeAttribute : Attribute
{
    /// <summary>
    /// The SQL field metadata used by the SQL generator for the decorated property.
    /// </summary>
    internal readonly FieldProperties FieldProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTypeAttribute"/> class.
    /// </summary>
    /// <param name="fieldProperties">The <see cref="FieldProperties"/> to associate with the property.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldProperties"/> is <c>null</c>.</exception>
    protected SqlTypeAttribute(FieldProperties fieldProperties)
    {
        ArgumentNullException.ThrowIfNull(fieldProperties, nameof(fieldProperties));
        FieldProperties = fieldProperties;
    }

    /// <summary>
    /// Retrieves the first attribute applied to the specified property that inherits from <see cref="SqlTypeAttribute"/>.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/> instance representing the property.</param>
    /// <returns>
    /// The first <see cref="SqlTypeAttribute"/> instance applied to the property, or <c>null</c> if no such attribute exists.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyInfo"/> is <c>null</c>.</exception>
    internal static SqlTypeAttribute? GetSqlTypeAttribute(PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        return propertyInfo
            .GetCustomAttributes(inherit: true)
            .OfType<SqlTypeAttribute>()
            .FirstOrDefault();
    }
}
