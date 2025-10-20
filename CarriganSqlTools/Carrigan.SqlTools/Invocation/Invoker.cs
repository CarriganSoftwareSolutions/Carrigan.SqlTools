using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

//IGNORE SPELLING: datetime, Enums
namespace Carrigan.SqlTools.Invocation;

/// <summary>
/// Instantiates and populates an instance of <typeparamref name="T"/> from a set of
/// ADO.NET result values, mapping result column names (including aliases) to the
/// corresponding writable properties on <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The entity or model type to instantiate. Must provide a parameter less constructor.
/// </typeparam>
public static class Invoker<T> where T : class?, new()
{
    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> and assigns property values from
    /// the supplied result set values using a reflection cache. The dictionary keys are
    /// interpreted as SQL result column names (potentially including aliases) and are
    /// resolved to properties on <typeparamref name="T"/> via
    /// <see cref="InvocationReflectorCache{T}.PropertyInfoCache"/>.
    /// </summary>
    /// <param name="invocation">
    /// A <see cref="Dictionary{TKey, TValue}"/> where each key is a result column name
    /// (e.g., the column label returned by the query, potentially an alias) and each value
    /// is the raw value to assign to the corresponding property on <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// A newly created instance of <typeparamref name="T"/> populated with the provided values.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="Activator.CreateInstance(Type)"/> fails to create the instance
    /// or the result cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidResultColumnNameException{T}">
    /// Thrown when one or more keys in <paramref name="invocation"/> do not resolve to properties
    /// on <typeparamref name="T"/> via the <see cref="ResultColumnName"/> mapping.
    /// </exception>
    public static T Invoke(Dictionary<string, object?> invocation)
    {
        InvalidResultColumnNameException<T>? exception = null;
        if (Activator.CreateInstance(InvocationReflectorCache<T>.Type) is not T invoked)
        {
            throw new InvalidOperationException("Instance creation failed or the cast was invalid.");
        }
        exception =
            InvocationReflectorCache<T>
                .PropertyInfoCache
                .GetExceptionForInvalidProperties(invocation.Keys.Select(name => new ResultColumnName(name)));
        if (exception != null)
            throw exception;
        foreach (string key in invocation.Keys)
        {
            ResultColumnName columnName = new(key);
            PropertyInfo property = InvocationReflectorCache<T>.PropertyInfoCache.Get(columnName);
            object? rawValue = invocation[key];
            object? valueToSet = Invoker<T>.ConvertValue(rawValue, property.PropertyType);

            property.SetValue(invoked, valueToSet);
        }
        return invoked;
    }
    /// <summary>
    /// Converts a raw ADO.NET value to the specified target property type, handling common
    /// cases such as <see cref="DateTime"/> to <see cref="DateOnly"/>, <see cref="TimeOnly"/>,
    /// and <see cref="DateTimeOffset"/>; <see cref="TimeSpan"/> to <see cref="TimeOnly"/>;
    /// and enum conversions from either strings or underlying numeric values.
    /// </summary>
    /// <param name="value">The raw value to be converted (may be <c>null</c> or <see cref="DBNull.Value"/>).</param>
    /// <param name="targetType">The property type to convert to (nullable types are supported).</param>
    /// <returns>
    /// The converted value suitable for assignment to a property of type <paramref name="targetType"/>,
    /// or <c>null</c> if <paramref name="value"/> is <c>null</c> or <see cref="DBNull.Value"/>.
    /// </returns>
    /// <remarks>
    /// Conversion rules:
    /// <list type="bullet">
    /// <item><description><c>null</c> or <see cref="DBNull.Value"/> → <c>null</c></description></item>
    /// <item><description><see cref="DateTime"/> → <see cref="DateOnly"/>, <see cref="TimeOnly"/>, or <see cref="DateTimeOffset"/> (when requested)</description></item>
    /// <item><description><see cref="TimeSpan"/> → <see cref="TimeOnly"/></description></item>
    /// <item><description>Enums: string values parsed via <see cref="System.Enum.Parse(System.Type, string)"/>, otherwise constructed via <see cref="System.Enum.ToObject(System.Type, object)"/></description></item>
    /// <item><description>All other types are returned as-is</description></item>
    /// </list>
    /// </remarks>
    private static object? ConvertValue(object? value, Type targetType)
    {
        // If the value is null or DBNull, return null
        if (value == null || value == DBNull.Value)
            return null;
        else 
        {
            // Handle nullable types by getting the underlying type.
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Special case: SQL date (returned as DateTime) to DateOnly conversion.
            if (value is DateTime dateTime)
            {
                if (underlyingType == typeof(DateOnly))
                    return DateOnly.FromDateTime(dateTime);
                else if (underlyingType == typeof(TimeOnly))
                    return TimeOnly.FromDateTime(dateTime);
                else if (underlyingType == typeof(DateTimeOffset))
                    return new DateTimeOffset(dateTime);
                else
                    return value;
            }

            // Special case: SQL datetime to TimeOnly conversion.
            else if (underlyingType == typeof(TimeOnly) && value is TimeSpan timeSpan)
            {
                return TimeOnly.FromTimeSpan(timeSpan);
            }

            // Special case: Enum conversion.
            else if (underlyingType.IsEnum)
            {
                if (value is string s)
                {
                    return Enum.Parse(underlyingType, s);
                }
                else
                {
                    return Enum.ToObject(underlyingType, value);
                }
            }
            else
                return value;
        }
    }
}
