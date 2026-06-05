using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Data.SqlTypes;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Invocation;

/// <summary>
/// Instantiates and populates an instance of <typeparamref name="T"/> from a set of
/// ADO.NET result values, mapping result column names (including aliases) to the
/// corresponding writable properties on <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The entity or model type to instantiate. Must provide a parameterless constructor.
/// </typeparam>
public static class Invoker<T> where T : class, new()
{
    /// <summary>
    /// Provides nullable reference type metadata for reflected members.
    /// </summary>
    private static readonly NullabilityInfoContext NullabilityContext = new();

    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> and assigns property values from
    /// the supplied result set values using a reflection cache. The dictionary keys are
    /// interpreted as SQL result column names (potentially including aliases) and are
    /// resolved to properties on <typeparamref name="T"/> via
    /// <see cref="InvocationReflectorCache{T}.PropertyInfoCache"/>.
    /// </summary>
    /// <param name="invocation">
    /// The key/value pairs representing a single result row. Keys are result column names,
    /// values are raw ADO.NET values (including <see cref="DBNull.Value"/>).
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="T"/> populated with values from <paramref name="invocation"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="invocation"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidResultColumnNameException{T}">
    /// Thrown when one or more result column names do not map to a writable property on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="XmlException">
    /// Thrown when XML parsing fails while converting <see cref="SqlXml"/> to <see cref="XDocument"/> or <see cref="XmlDocument"/>.
    /// </exception>
    /// <exception cref="TargetException">
    /// Thrown when reflection-based assignment fails due to an invalid target instance.
    /// </exception>
    /// <exception cref="TargetInvocationException">
    /// Thrown when the property setter throws while assigning a value.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when enum parsing fails, or when the converted value cannot be assigned to the target property type.
    /// </exception>
    public static T Invoke(Dictionary<string, object?> invocation)
    {
        ArgumentNullException.ThrowIfNull(invocation);

        PropertyInfoCache<T> propertyInfoCache = InvocationReflectorCache<T>.PropertyInfoCache;
        InvalidResultColumnNameException<T>? invalidException =
            propertyInfoCache.GetExceptionForInvalidProperties(invocation.Keys.Select(static key => new ResultColumnName(key)));

        if (invalidException is not null)
            throw invalidException;

        T invoked = new();

        foreach (KeyValuePair<string, object?> pair in invocation)
        {
            ResultColumnName columnName = new(pair.Key);
            PropertyInfo propertyInfo = propertyInfoCache.Get(columnName);
            object? valueToSet = ConvertValue(pair.Value, propertyInfo);
            propertyInfo.SetValue(invoked, valueToSet);
        }

        return invoked;
    }

    /// <summary>
    /// Converts a raw ADO.NET value to a value assignable to the given property.
    /// </summary>
    /// <param name="databaseValue">
    /// The raw value to convert (may be <c>null</c> or <see cref="DBNull.Value"/>).
    /// </param>
    /// <param name="propertyInfo">
    /// The property receiving the converted value.
    /// </param>
    /// <returns>
    /// A value suitable for assignment to <paramref name="propertyInfo"/>. When the input is <c>null</c> /
    /// <see cref="DBNull.Value"/>, this method returns <c>null</c> even if the property type is non-nullable,
    /// except that non-nullable <see cref="string"/> and <see cref="byte"/>[] properties are normalized to
    /// <see cref="string.Empty"/> and <see cref="Array.Empty{T}"/> respectively.
    /// </returns>
    private static object? ConvertValue(object? databaseValue, PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        Type targetType = propertyInfo.PropertyType;

        if (targetType.IsArray && targetType != typeof(byte[]) && databaseValue is Array databaseArray)
            return ConvertArrayValue(databaseArray, targetType);

        return ConvertScalarValue(databaseValue, targetType, IsNullable(propertyInfo));
    }

    /// <summary>
    /// Converts a database array into the CLR array type required by the destination model property.
    /// </summary>
    /// <param name="databaseArray">The array value returned by the database provider.</param>
    /// <param name="targetArrayType">The CLR array type declared by the model property.</param>
    /// <returns>An array with each element converted to the target element type.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="targetArrayType"/> is not an array type with an element type.
    /// </exception>
    private static Array ConvertArrayValue(Array databaseArray, Type targetArrayType)
    {
        Type targetElementType = targetArrayType.GetElementType()
            ?? throw new ArgumentException("Array target types must have an element type.", nameof(targetArrayType));

        Array convertedArray = Array.CreateInstance(targetElementType, databaseArray.Length);

        for (int index = 0; index < databaseArray.Length; index++)
        {
            object? databaseValue = databaseArray.GetValue(index);
            object? convertedValue = ConvertScalarValue(databaseValue, targetElementType, targetIsNullable: true);
            convertedArray.SetValue(convertedValue, index);
        }

        return convertedArray;
    }

    /// <summary>
    /// Converts a single database value into the CLR type required by the destination model property or array element.
    /// </summary>
    /// <param name="databaseValue">The value returned by the database provider.</param>
    /// <param name="targetType">The CLR type expected by the destination model member.</param>
    /// <param name="targetIsNullable">Whether the destination member can accept <see langword="null"/>.</param>
    /// <returns>The converted value, the original value when no conversion is needed, or <see langword="null"/> for database nulls.</returns>
    /// <exception cref="XmlException">
    /// Thrown when XML text cannot be parsed as <see cref="XDocument"/> or <see cref="XmlDocument"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when enum or numeric conversion fails for the destination type.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/> cannot convert the supplied value.
    /// </exception>
    private static object? ConvertScalarValue(object? databaseValue, Type targetType, bool targetIsNullable)
    {
        Type underlyingTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (databaseValue is null || databaseValue == DBNull.Value)
        {
            if (underlyingTargetType == typeof(string))
                return targetIsNullable ? null : string.Empty;
            else if (underlyingTargetType == typeof(byte[]))
                return targetIsNullable ? null : Array.Empty<byte>();
            else
                return null;
        }

        if (underlyingTargetType.IsInstanceOfType(databaseValue))
            return databaseValue;

        if (databaseValue is SqlXml sqlXmlValue)
        {
            if (sqlXmlValue.IsNull)
                return null;
            else if (underlyingTargetType == typeof(XDocument))
                return XDocument.Parse(sqlXmlValue.Value);
            else if (underlyingTargetType == typeof(XmlDocument))
            {
                XmlDocument xmlDocument = new();
                xmlDocument.LoadXml(sqlXmlValue.Value);
                return xmlDocument;
            }

            return sqlXmlValue;
        }
        else if (underlyingTargetType == typeof(XDocument) && databaseValue is string xDocumentXml)
            return XDocument.Parse(xDocumentXml);
        else if (underlyingTargetType == typeof(XmlDocument) && databaseValue is string xmlDocumentXml)
        {
            XmlDocument xmlDocument = new();
            xmlDocument.LoadXml(xmlDocumentXml);
            return xmlDocument;
        }
        // Special case: SQL date (returned as DateTime) to DateOnly conversion.
        else if (databaseValue is DateTime dateTime)
        {
            if (underlyingTargetType == typeof(DateOnly))
                return DateOnly.FromDateTime(dateTime);
            else if (underlyingTargetType == typeof(TimeOnly))
                return TimeOnly.FromDateTime(dateTime);
            else if (underlyingTargetType == typeof(DateTimeOffset))
                return new DateTimeOffset(dateTime);
            else
                return databaseValue;
        }
        // Special case: SQL datetime to TimeOnly conversion.
        else if (underlyingTargetType == typeof(TimeOnly) && databaseValue is TimeSpan timeSpan)
            return TimeOnly.FromTimeSpan(timeSpan);
        else if (underlyingTargetType == typeof(char) && databaseValue is string charAsString)
            return charAsString.Length == 0 ? null : charAsString[0];
        else if (underlyingTargetType.IsEnum)
        {
            if (databaseValue is string s)
                return Enum.Parse(underlyingTargetType, s);
            else
                return Enum.ToObject(underlyingTargetType, databaseValue);
        }
        else if (databaseValue is IConvertible && IsConvertibleNumericTarget(underlyingTargetType))
            return Convert.ChangeType(databaseValue, underlyingTargetType, CultureInfo.InvariantCulture);

        return databaseValue;
    }

    /// <summary>
    /// Determines whether a CLR type can be populated through invariant-culture numeric conversion.
    /// </summary>
    /// <param name="targetType">The non-nullable CLR type being assigned.</param>
    /// <returns><see langword="true"/> when <paramref name="targetType"/> is a supported numeric CLR type; otherwise, <see langword="false"/>.</returns>
    private static bool IsConvertibleNumericTarget(Type targetType) =>
        targetType == typeof(byte)
        || targetType == typeof(sbyte)
        || targetType == typeof(short)
        || targetType == typeof(ushort)
        || targetType == typeof(int)
        || targetType == typeof(uint)
        || targetType == typeof(long)
        || targetType == typeof(ulong)
        || targetType == typeof(float)
        || targetType == typeof(double)
        || targetType == typeof(decimal);

    /// <summary>
    /// Determines whether the reflected property can accept <see langword="null"/> values.
    /// </summary>
    /// <param name="propertyInfo">The reflected model property being assigned.</param>
    /// <returns><see langword="true"/> when the property is nullable; otherwise, <see langword="false"/>.</returns>
    private static bool IsNullable(PropertyInfo propertyInfo)
    {
        Type type = propertyInfo.PropertyType;

        if (type.IsValueType)
            return Nullable.GetUnderlyingType(type) != null;

        NullabilityInfo nullable = NullabilityContext.Create(propertyInfo);
        return nullable.WriteState == NullabilityState.Nullable;
    }
}
