// IGNORE SPELLING: bigint, bool, bytea, char, jsonb, numeric, timestamptz, uuid, varchar, varbit, xml, nullability

using Carrigan.SqlTools.Types;
using System.Reflection.Metadata;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// Provides predefined <see cref="FieldProperties"/> values for PostgreSQL data types.
/// </summary>
public static class PostgreSqlTypesProvider
{
    private const int LIMIT_FOR_CHARACTER_LENGTH = 10_485_760;
    private const int LIMIT_FOR_BIT_LENGTH = 10_485_760;
    private const bool DEFAULT_IS_NULLABLE = false;
    private const byte LIMIT_FOR_NUMERIC_PRECISION = 255;
    private const byte LIMIT_FOR_FRACTIONAL_SECONDS_PRECISION = 6;
    private const byte LIMIT_FOR_FLOAT_MIN_PRECISION = 1;
    private const byte LIMIT_FOR_FLOAT_MAX_PRECISION = 53;
    private const int LIMIT_FOR_VECTOR_MIN_DIMENSIONS = 1;

    #region Helper Methods

    private static FieldProperties Create
    (
        string providerTypeName,
        int? length = null,
        bool? isMax = null,
        bool? isUnicode = null,
        bool? isFixedLength = null,
        byte? precision = null,
        byte? scale = null,
        byte? fractionalSecondsPrecision = null,
        bool? isArray = null,
        string? baseType = null,
        bool? nullable = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerTypeName);

        if (length is <= 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be greater than zero when specified.");

        if (precision is 0)
            throw new ArgumentOutOfRangeException(nameof(precision), precision, "Precision must be greater than zero when specified.");

        if (scale is not null && precision is null)
            throw new ArgumentException("Scale cannot be specified without precision.", nameof(scale));

        if (scale is not null && precision is not null && scale > precision)
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale cannot be greater than precision.");

        return new FieldProperties
        {
            Length = length,
            IsMax = isMax,
            IsUnicode = isUnicode,
            IsFixedLength = isFixedLength,
            Precision = precision,
            Scale = scale,
            FractionalSecondsPrecision = fractionalSecondsPrecision,
            IsNullable = nullable ?? DEFAULT_IS_NULLABLE,
            ProviderTypeName = providerTypeName.ToUpperInvariant(),
            BaseType = baseType?.ToUpperInvariant(),
            IsArray = isArray
        };
    }

    private static FieldProperties Create(Type clrType, bool? nullable = null)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        Type type = Nullable.GetUnderlyingType(clrType) ?? clrType;
        bool? effectiveNullable = nullable ?? (Nullable.GetUnderlyingType(clrType) is not null ? true : null);

        if (type == typeof(byte[])) return AsBytea(false, effectiveNullable);

        if (type.IsArray)
        {
            Type elementType = type.GetElementType()
                ?? throw new ArgumentException("Array types must have an element type.", nameof(clrType));
            Type effectiveElementType = Nullable.GetUnderlyingType(elementType) ?? elementType;

            return CreateFromClrType(effectiveElementType, true, effectiveNullable);
        }

        return CreateFromClrType(type, false, effectiveNullable);
    }

    private static FieldProperties CreateFromClrType(Type type, bool isArray, bool? nullable)
    {
        if (type == typeof(Guid)) return AsUuid(isArray, nullable);

        if (type == typeof(string)) return AsText(isArray, nullable);
        if (type == typeof(char)) return AsChar(1, isArray, nullable);

        if (type == typeof(byte[])) return AsBytea(isArray, nullable);

        if (type == typeof(bool)) return AsBoolean(isArray, nullable);

        if (type == typeof(byte)) return AsSmallInt(isArray, nullable);
        if (type == typeof(sbyte)) return AsSmallInt(isArray, nullable);
        if (type == typeof(short)) return AsSmallInt(isArray, nullable);
        if (type == typeof(ushort)) return AsInteger(isArray, nullable);
        if (type == typeof(int)) return AsInteger(isArray, nullable);
        if (type == typeof(uint)) return AsBigInt(isArray, nullable);
        if (type == typeof(long)) return AsBigInt(isArray, nullable);
        if (type == typeof(ulong)) return AsNumeric(20, 0, isArray, nullable);

        if (type == typeof(float)) return AsReal(isArray, nullable);
        if (type == typeof(double)) return AsDoublePrecision(isArray, nullable);
        if (type == typeof(decimal)) return AsNumeric(isArray, nullable);

        if (type == typeof(DateTime)) return AsTimestampWithoutTimeZone(isArray, nullable);
        if (type == typeof(DateOnly)) return AsDate(isArray, nullable);
        if (type == typeof(TimeOnly)) return AsTimeWithoutTimeZone(isArray, nullable);
        if (type == typeof(TimeSpan)) return AsInterval(isArray, nullable);
        if (type == typeof(DateTimeOffset)) return AsTimestampWithTimeZone(isArray, nullable);

        if (type == typeof(XmlDocument)) return AsXml(isArray, nullable);
        if (type == typeof(XDocument)) return AsXml(isArray, nullable);

        return AsText(isArray, nullable);
    }

    private static void ValidateRange(int? value, bool allowNullValue, int minValue, int maxValue, string parameterName)
    {
        if (value is null)
        {
            if (allowNullValue is false)
                throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {minValue} and {maxValue}.");
        }
        else if (value < minValue || value > maxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {minValue} and {maxValue}.");
        }
    }

    private static void ValidateCharacterLength(int? length) =>
        ValidateRange(length, true, 1, LIMIT_FOR_CHARACTER_LENGTH, nameof(length));

    private static void ValidateBitLength(int length) =>
        ValidateRange(length, false, 1, LIMIT_FOR_BIT_LENGTH, nameof(length));

    private static void ValidatePrecision(byte precision) =>
        ValidateRange(precision, false, 1, LIMIT_FOR_NUMERIC_PRECISION, nameof(precision));

    private static void ValidatePrecisionAndScale(byte precision, byte scale)
    {
        ValidatePrecision(precision);

        if (scale > precision)
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale cannot be greater than precision.");
    }

    private static void ValidateFractionalSecondsPrecision(byte fractionalSecondsPrecision) =>
        ValidateRange(fractionalSecondsPrecision, false, 0, LIMIT_FOR_FRACTIONAL_SECONDS_PRECISION, nameof(fractionalSecondsPrecision));

    private static void ValidateFloatPrecision(byte? precision) =>
        ValidateRange(precision, true, LIMIT_FOR_FLOAT_MIN_PRECISION, LIMIT_FOR_FLOAT_MAX_PRECISION, nameof(precision));

    private static void ValidateVectorDimensions(int dimensions)
    {
        if (dimensions < LIMIT_FOR_VECTOR_MIN_DIMENSIONS)
        {
            throw new ArgumentOutOfRangeException(nameof(dimensions), dimensions, "Vector dimensions must be greater than zero.");
        }
    }

    private static FieldProperties CreateNumericType(bool isArray, bool? nullable) =>
        Create("NUMERIC", isArray: isArray, nullable: nullable);

    private static FieldProperties CreateNumericType(byte precision, bool isArray, bool? nullable)
    {
        ValidatePrecision(precision);

        return Create(
            providerTypeName: "NUMERIC",
            precision: precision,
            isArray: isArray,
            nullable: nullable);
    }

    private static FieldProperties CreateNumericType(byte precision, byte scale, bool isArray, bool? nullable)
    {
        ValidatePrecisionAndScale(precision, scale);

        return Create(
            providerTypeName: "NUMERIC",
            precision: precision,
            scale: scale,
            isArray: isArray,
            nullable: nullable);
    }

    #endregion

    #region CLR Types

    public static FieldProperties FromClrType(Type clrType) =>
        Create(clrType);

    /// <summary>
    /// Creates PostgreSQL <see cref="FieldProperties"/> from a CLR value.
    /// </summary>
    /// <param name="value">The CLR value whose runtime type should be mapped to a PostgreSQL provider type.</param>
    /// <returns>
    /// A <see cref="FieldProperties"/> instance representing the PostgreSQL type that corresponds
    /// to the runtime type of <paramref name="value"/>, or PostgreSQL <c>UNKNOWN</c> when the value is null.
    /// </returns>
    /// <remarks>
    /// <para>
    /// When <paramref name="value"/> is <see langword="null"/> or <see cref="DBNull.Value"/>, this method returns
    /// an unknown nullable PostgreSQL provider type. This allows PostgreSQL to infer the intended type from
    /// the SQL context when possible.
    /// </para>
    /// <para>
    /// Prefer explicit <see cref="FieldProperties"/> when the intended PostgreSQL type is known.
    /// </para>
    /// </remarks>
    public static FieldProperties FromClrValue(object? value)
    {
        if (value is null || value == DBNull.Value)
            return AsUnknown(nullable: true);
        else
            return Create(value.GetType());
    }
    #endregion

    #region UUID

    public static FieldProperties AsUuid(bool isArray, bool? nullable = null) =>
        Create("UUID", isArray: isArray, nullable: nullable);

    #endregion

    #region Character Types

    public static FieldProperties AsChar(int? length, bool isArray, bool? nullable = null)
    {
        ValidateCharacterLength(length);

        return Create(
            providerTypeName: "CHAR",
            length: length,
            isUnicode: true,
            isFixedLength: true,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsVarChar(int? length, bool isArray, bool? nullable = null)
    {
        ValidateCharacterLength(length);

        return Create(
            providerTypeName: "VARCHAR",
            length: length,
            isUnicode: true,
            isFixedLength: false,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsText(bool isArray, bool? nullable = null) =>
        Create(
            providerTypeName: "TEXT",
            isMax: true,
            isUnicode: true,
            isFixedLength: false,
            isArray: isArray,
            nullable: nullable);

    #endregion

    #region Binary

    public static FieldProperties AsBytea(bool isArray, bool? nullable = null) =>
        Create(
            providerTypeName: "BYTEA",
            isMax: true,
            isFixedLength: false,
            isArray: isArray,
            nullable: nullable);

    #endregion

    #region Boolean

    public static FieldProperties AsBoolean(bool isArray, bool? nullable = null) =>
        Create("BOOLEAN", isArray: isArray, nullable: nullable);

    #endregion

    #region Integers

    public static FieldProperties AsSmallInt(bool isArray, bool? nullable = null) =>
        Create("SMALLINT", isArray: isArray, nullable: nullable);

    public static FieldProperties AsInteger(bool isArray, bool? nullable = null) =>
        Create("INTEGER", isArray: isArray, nullable: nullable);

    public static FieldProperties AsBigInt(bool isArray, bool? nullable = null) =>
        Create("BIGINT", isArray: isArray, nullable: nullable);

    #endregion

    #region Floating Points

    public static FieldProperties AsReal(bool isArray, bool? nullable = null) =>
        Create("REAL", isArray: isArray, nullable: nullable);

    public static FieldProperties AsDoublePrecision(bool isArray, bool? nullable = null) =>
        Create("DOUBLE PRECISION", isArray: isArray, nullable: nullable);

    public static FieldProperties AsFloat(byte? precision, bool isArray, bool? nullable = null)
    {
        ValidateFloatPrecision(precision);

        return Create(
            providerTypeName: "FLOAT",
            precision: precision,
            isArray: isArray,
            nullable: nullable);
    }

    #endregion

    #region Numeric

    public static FieldProperties AsNumeric(bool isArray, bool? nullable = null) =>
        CreateNumericType(isArray, nullable);

    public static FieldProperties AsNumeric(byte precision, bool isArray, bool? nullable = null) =>
        CreateNumericType(precision, isArray, nullable);

    public static FieldProperties AsNumeric(byte precision, byte scale, bool isArray, bool? nullable = null) =>
        CreateNumericType(precision, scale, isArray, nullable);

    public static FieldProperties AsMoney(bool isArray, bool? nullable = null) =>
        Create("MONEY", isArray: isArray, nullable: nullable);

    #endregion

    #region Date/Time

    public static FieldProperties AsDate(bool isArray, bool? nullable = null) =>
        Create("DATE", isArray: isArray, nullable: nullable);

    public static FieldProperties AsTime(bool isArray, bool? nullable = null) =>
        Create("TIME", isArray: isArray, nullable: nullable);

    public static FieldProperties AsTime(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsTimeWithoutTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIME WITHOUT TIME ZONE", isArray: isArray, nullable: nullable);

    public static FieldProperties AsTimeWithoutTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME WITHOUT TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsTimeWithTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIME WITH TIME ZONE", isArray: isArray, nullable: nullable);

    public static FieldProperties AsTimeWithTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME WITH TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsTimestamp(bool isArray, bool? nullable = null) =>
        Create("TIMESTAMP", isArray: isArray, nullable: nullable);

    public static FieldProperties AsTimestamp(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsTimestampWithoutTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIMESTAMP WITHOUT TIME ZONE", isArray: isArray, nullable: nullable);

    public static FieldProperties AsTimestampWithoutTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP WITHOUT TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsTimestampWithTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIMESTAMP WITH TIME ZONE", isArray: isArray, nullable: nullable);

    public static FieldProperties AsTimestampWithTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP WITH TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsInterval(bool isArray, bool? nullable = null) =>
        Create("INTERVAL", isArray: isArray, nullable: nullable);

    #endregion

    #region XML/JSON

    public static FieldProperties AsXml(bool isArray, bool? nullable = null) =>
        Create("XML", isArray: isArray, nullable: nullable);

    public static FieldProperties AsJson(bool isArray, bool? nullable = null) =>
        Create("JSON", isArray: isArray, nullable: nullable);

    public static FieldProperties AsJsonB(bool isArray, bool? nullable = null) =>
        Create("JSONB", isArray: isArray, nullable: nullable);

    #endregion

    #region Bit Strings

    public static FieldProperties AsBit(int length, bool isArray, bool? nullable = null)
    {
        ValidateBitLength(length);

        return Create(
            providerTypeName: "BIT",
            length: length,
            isFixedLength: true,
            isArray: isArray,
            nullable: nullable);
    }

    public static FieldProperties AsVarBit(int length, bool isArray, bool? nullable = null)
    {
        ValidateBitLength(length);

        return Create(
            providerTypeName: "VARBIT",
            length: length,
            isFixedLength: false,
            isArray: isArray,
            nullable: nullable);
    }

    #endregion

    #region Vector

    public static FieldProperties AsVector(int dimensions, bool isArray, bool? nullable = null)
    {
        ValidateVectorDimensions(dimensions);

        return Create(
            providerTypeName: "VECTOR",
            length: dimensions,
            isArray: isArray,
            nullable: nullable);
    }

    #endregion

    #region Unknown
    public static FieldProperties AsUnknown(bool? nullable) =>
        Create("UNKNOWN", nullable: nullable);
    #endregion

    #region Provider Specific

    public static FieldProperties AsProviderSpecific(string providerTypeName, bool? nullable = null) =>
        Create(providerTypeName, nullable: nullable);

    #endregion
}