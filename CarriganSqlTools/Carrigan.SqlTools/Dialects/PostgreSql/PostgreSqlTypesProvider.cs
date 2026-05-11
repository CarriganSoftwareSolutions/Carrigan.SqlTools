// IGNORE SPELLING: bigint, bool, bytea, char, jsonb, numeric, timestamptz, uuid, varchar, varbit, xml, nullability

using Carrigan.SqlTools.Types;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Dialects.PostgreSql;

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

    private static FieldProperties Create(
        string providerTypeName,
        int? length = null,
        bool? isMax = null,
        bool? isUnicode = null,
        bool? isFixedLength = null,
        byte? precision = null,
        byte? scale = null,
        byte? fractionalSecondsPrecision = null,
        string? baseType = null,
        bool? nullable = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerTypeName);

        if (length is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be greater than zero when specified.");
        }

        if (precision is 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precision), precision, "Precision must be greater than zero when specified.");
        }

        if (scale is not null && precision is null)
        {
            throw new ArgumentException("Scale cannot be specified without precision.", nameof(scale));
        }

        if (scale is not null && precision is not null && scale > precision)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale cannot be greater than precision.");
        }

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
            BaseType = baseType?.ToUpperInvariant()
        };
    }

    private static FieldProperties Create(Type clrType, bool? nullable = null)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        Type type = Nullable.GetUnderlyingType(clrType) ?? clrType;
        bool? effectiveNullable = nullable ?? (Nullable.GetUnderlyingType(clrType) is not null ? true : null);

        if (type == typeof(Guid)) return AsUuid(effectiveNullable);
        if (type == typeof(string)) return AsText(effectiveNullable);
        if (type == typeof(char)) return AsChar(1, effectiveNullable);
        if (type == typeof(byte[])) return AsBytea(effectiveNullable);

        if (type == typeof(bool)) return AsBoolean(effectiveNullable);

        if (type == typeof(byte)) return AsSmallInt(effectiveNullable);
        if (type == typeof(sbyte)) return AsSmallInt(effectiveNullable);
        if (type == typeof(short)) return AsSmallInt(effectiveNullable);
        if (type == typeof(ushort)) return AsInteger(effectiveNullable);
        if (type == typeof(int)) return AsInteger(effectiveNullable);
        if (type == typeof(uint)) return AsBigInt(effectiveNullable);
        if (type == typeof(long)) return AsBigInt(effectiveNullable);
        if (type == typeof(ulong)) return AsNumeric(20, 0, effectiveNullable);

        if (type == typeof(float)) return AsReal(effectiveNullable);
        if (type == typeof(double)) return AsDoublePrecision(effectiveNullable);
        if (type == typeof(decimal)) return AsNumeric(effectiveNullable);

        if (type == typeof(DateTime)) return AsTimestampWithoutTimeZone(effectiveNullable);
        if (type == typeof(DateOnly)) return AsDate(effectiveNullable);
        if (type == typeof(TimeOnly)) return AsTimeWithoutTimeZone(effectiveNullable);
        if (type == typeof(DateTimeOffset)) return AsTimestampWithTimeZone(effectiveNullable);

        if (type == typeof(XmlDocument)) return AsXml(effectiveNullable);
        if (type == typeof(XDocument)) return AsXml(effectiveNullable);

        return AsText(effectiveNullable);
    }

    private static void ValidateRange(int value, int minValue, int maxValue, string parameterName)
    {
        if (value < minValue || value > maxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {minValue} and {maxValue}.");
        }
    }

    private static void ValidateCharacterLength(int length) =>
        ValidateRange(length, 1, LIMIT_FOR_CHARACTER_LENGTH, nameof(length));

    private static void ValidateBitLength(int length) =>
        ValidateRange(length, 1, LIMIT_FOR_BIT_LENGTH, nameof(length));

    private static void ValidatePrecision(byte precision) =>
        ValidateRange(precision, 1, LIMIT_FOR_NUMERIC_PRECISION, nameof(precision));

    private static void ValidatePrecisionAndScale(byte precision, byte scale)
    {
        ValidatePrecision(precision);

        if (scale > precision)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale cannot be greater than precision.");
        }
    }

    private static void ValidateFractionalSecondsPrecision(byte fractionalSecondsPrecision) =>
        ValidateRange(fractionalSecondsPrecision, 0, LIMIT_FOR_FRACTIONAL_SECONDS_PRECISION, nameof(fractionalSecondsPrecision));

    private static void ValidateFloatPrecision(byte precision) =>
        ValidateRange(precision, LIMIT_FOR_FLOAT_MIN_PRECISION, LIMIT_FOR_FLOAT_MAX_PRECISION, nameof(precision));

    private static void ValidateVectorDimensions(int dimensions)
    {
        if (dimensions < LIMIT_FOR_VECTOR_MIN_DIMENSIONS)
        {
            throw new ArgumentOutOfRangeException(nameof(dimensions), dimensions, "Vector dimensions must be greater than zero.");
        }
    }

    private static FieldProperties CreateNumericType(bool? nullable) =>
        Create("NUMERIC", nullable: nullable);

    private static FieldProperties CreateNumericType(byte precision, bool? nullable)
    {
        ValidatePrecision(precision);

        return Create(
            providerTypeName: "NUMERIC",
            precision: precision,
            nullable: nullable);
    }

    private static FieldProperties CreateNumericType(byte precision, byte scale, bool? nullable)
    {
        ValidatePrecisionAndScale(precision, scale);

        return Create(
            providerTypeName: "NUMERIC",
            precision: precision,
            scale: scale,
            nullable: nullable);
    }

    #endregion

    #region CLR Types

    public static FieldProperties FromClrType(Type clrType) => Create(clrType);

    public static FieldProperties FromClrType<T>() => FromClrType(typeof(T));

    public static FieldProperties FromNullableClrType(Type clrType) => Create(clrType, nullable: true);

    public static FieldProperties FromNullableClrType<T>() => FromNullableClrType(typeof(T));

    #endregion

    #region UUID

    public static FieldProperties AsUuid(bool? nullable = null) => Create("UUID", nullable: nullable);

    #endregion

    #region Character Types

    public static FieldProperties AsChar(int length, bool? nullable = null)
    {
        ValidateCharacterLength(length);

        return Create(
            providerTypeName: "CHAR",
            length: length,
            isUnicode: true,
            isFixedLength: true,
            nullable: nullable);
    }

    public static FieldProperties AsVarChar(int length, bool? nullable = null)
    {
        ValidateCharacterLength(length);

        return Create(
            providerTypeName: "VARCHAR",
            length: length,
            isUnicode: true,
            isFixedLength: false,
            nullable: nullable);
    }

    public static FieldProperties AsText(bool? nullable = null) =>
        Create(
            providerTypeName: "TEXT",
            isMax: true,
            isUnicode: true,
            isFixedLength: false,
            nullable: nullable);

    #endregion

    #region Binary

    public static FieldProperties AsBytea(bool? nullable = null) =>
        Create(
            providerTypeName: "BYTEA",
            isMax: true,
            isFixedLength: false,
            nullable: nullable);

    #endregion

    #region Boolean

    public static FieldProperties AsBoolean(bool? nullable = null) => Create("BOOLEAN", nullable: nullable);

    #endregion

    #region Integers

    public static FieldProperties AsSmallInt(bool? nullable = null) => Create("SMALLINT", nullable: nullable);

    public static FieldProperties AsInteger(bool? nullable = null) => Create("INTEGER", nullable: nullable);

    public static FieldProperties AsBigInt(bool? nullable = null) => Create("BIGINT", nullable: nullable);

    #endregion

    #region Floating Points

    public static FieldProperties AsReal(bool? nullable = null) => Create("REAL", nullable: nullable);

    public static FieldProperties AsDoublePrecision(bool? nullable = null) => Create("DOUBLE PRECISION", nullable: nullable);

    public static FieldProperties AsFloat(byte precision, bool? nullable = null)
    {
        ValidateFloatPrecision(precision);

        return Create(
            providerTypeName: "FLOAT",
            precision: precision,
            nullable: nullable);
    }

    #endregion

    #region Numeric

    public static FieldProperties AsNumeric(bool? nullable = null) => CreateNumericType(nullable);

    public static FieldProperties AsNumeric(byte precision, bool? nullable = null) => CreateNumericType(precision, nullable);

    public static FieldProperties AsNumeric(byte precision, byte scale, bool? nullable = null) => CreateNumericType(precision, scale, nullable);

    public static FieldProperties AsMoney(bool? nullable = null) => Create("MONEY", nullable: nullable);

    #endregion

    #region Date/Time

    public static FieldProperties AsDate(bool? nullable = null) => Create("DATE", nullable: nullable);

    public static FieldProperties AsTime(bool? nullable = null) => Create("TIME", nullable: nullable);

    public static FieldProperties AsTime(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    public static FieldProperties AsTimeWithoutTimeZone(bool? nullable = null) =>
        Create("TIME WITHOUT TIME ZONE", nullable: nullable);

    public static FieldProperties AsTimeWithoutTimeZone(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME WITHOUT TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    public static FieldProperties AsTimeWithTimeZone(bool? nullable = null) =>
        Create("TIME WITH TIME ZONE", nullable: nullable);

    public static FieldProperties AsTimeWithTimeZone(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME WITH TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    public static FieldProperties AsTimestamp(bool? nullable = null) => Create("TIMESTAMP", nullable: nullable);

    public static FieldProperties AsTimestamp(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    public static FieldProperties AsTimestampWithoutTimeZone(bool? nullable = null) =>
        Create("TIMESTAMP WITHOUT TIME ZONE", nullable: nullable);

    public static FieldProperties AsTimestampWithoutTimeZone(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP WITHOUT TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    public static FieldProperties AsTimestampWithTimeZone(bool? nullable = null) =>
        Create("TIMESTAMP WITH TIME ZONE", nullable: nullable);

    public static FieldProperties AsTimestampWithTimeZone(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP WITH TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    #endregion

    #region XML/JSON

    public static FieldProperties AsXml(bool? nullable = null) => Create("XML", nullable: nullable);

    public static FieldProperties AsJson(bool? nullable = null) => Create("JSON", nullable: nullable);

    public static FieldProperties AsJsonB(bool? nullable = null) => Create("JSONB", nullable: nullable);

    #endregion

    #region Bit Strings

    public static FieldProperties AsBit(int length, bool? nullable = null)
    {
        ValidateBitLength(length);

        return Create(
            providerTypeName: "BIT",
            length: length,
            isFixedLength: true,
            nullable: nullable);
    }

    public static FieldProperties AsVarBit(int length, bool? nullable = null)
    {
        ValidateBitLength(length);

        return Create(
            providerTypeName: "VARBIT",
            length: length,
            isFixedLength: false,
            nullable: nullable);
    }

    #endregion

    #region Vector

    public static FieldProperties AsVector(int dimensions, bool? nullable = null)
    {
        ValidateVectorDimensions(dimensions);

        return Create(
            providerTypeName: "VECTOR",
            length: dimensions,
            nullable: nullable);
    }

    #endregion

    #region Provider Specific

    public static FieldProperties AsProviderSpecific(string providerTypeName, bool? nullable = null) =>
        Create(providerTypeName, nullable: nullable);

    #endregion
}