using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

namespace Carrigan.SqlTools.Invocation;

//IGNORE SPELLING: datetime

//TODO: Update Documentation

/// <summary>
/// Invoke a class of type <see cref="T"/> using values defined <see cref="Dictionary{string, object?}"/> 
/// where the key represents a property in <see cref="T"/> and the value the value of that property.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class Invoker<T> where T : class?, new()
{
    /// <summary>
    /// Invoke a class of type <see cref="T"/> using values defined <see cref="Dictionary{string, object?}"/> 
    /// where the key represents a property in <see cref="T"/> and the value the value of that property.
    /// </summary>
    /// <param name="invocation">a <see cref="Dictionary{string, object?}"/>  where the key represents a property in <see cref="T"/> and the value the value of that property</param>
    /// <returns>A instance of the type <see cref="T"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
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
    public static object? ConvertValue(object? value, Type targetType)
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
