// IGNORE SPELLING: jsonb, varbit

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
    /// <summary>
    /// The maximum allowed length for character types (CHAR, VARCHAR, TEXT)
    /// </summary>
    private const int LIMIT_FOR_CHARACTER_LENGTH = 10_485_760;
    /// <summary>
    /// The maximum allowed length for bit types (BIT, VARBIT)
    /// </summary>
    private const int LIMIT_FOR_BIT_LENGTH = 10_485_760;
    /// <summary>
    /// The default nullability for PostgreSQL types when not explicitly specified.
    /// </summary>
    private const bool DEFAULT_IS_NULLABLE = false;
    /// <summary>
    /// The maximum allowed precision for numeric types (NUMERIC, DECIMAL).
    /// </summary>
    private const byte LIMIT_FOR_NUMERIC_PRECISION = 255;
    /// <summary>
    /// The maximum allowed precision for fractional seconds in time-related types (TIME, TIMESTAMP).
    /// </summary>
    private const byte LIMIT_FOR_FRACTIONAL_SECONDS_PRECISION = 6;
    /// <summary>
    /// The minimum allowed precision for floating-point types (FLOAT).
    /// </summary>
    private const byte LIMIT_FOR_FLOAT_MIN_PRECISION = 1;
    /// <summary>
    /// The maximum allowed precision for floating-point types (FLOAT).
    /// </summary>
    private const byte LIMIT_FOR_FLOAT_MAX_PRECISION = 53;
    /// <summary>
    /// The minimum allowed number of dimensions for vector types (VECTOR).
    /// </summary>
    private const int LIMIT_FOR_VECTOR_MIN_DIMENSIONS = 1;

    #region Helper Methods

    /// <summary>
    /// Centralized factory method for creating <see cref="FieldProperties"/> instances with validation.
    /// </summary>
    /// <param name="providerTypeName">
    /// The PostgreSQL provider type name (e.g., "VARCHAR", "NUMERIC"). This is required and must not be null or whitespace.
    /// </param>
    /// <param name="length">
    /// The length to apply for applicable types (e.g., character types). Must be greater than zero when specified, and is validated according to PostgreSQL limits.
    /// </param>
    /// <param name="isMax">
    /// Indicates whether the type should use the maximum length allowed by PostgreSQL (e.g., TEXT, BYTEA). This is mutually exclusive with a specified length.
    /// </param>
    /// <param name="isUnicode">
    /// Indicates whether the type should be treated as Unicode. This is typically true for character types and false for binary types, but can be specified explicitly.
    /// </param>
    /// <param name="isFixedLength">
    /// Indicates whether the type should be treated as fixed-length (e.g., CHAR) or variable-length (e.g., VARCHAR).
    /// This is typically true for CHAR and false for VARCHAR and TEXT, but can be specified explicitly.
    /// </param>
    /// <param name="precision">
    /// The precision to apply for applicable types (e.g., NUMERIC, FLOAT). Must be greater than zero when specified, and is validated according to PostgreSQL limits.
    /// </param>
    /// <param name="scale">
    /// The scale to apply for applicable types (e.g., NUMERIC, DECIMAL). Must be greater than zero when specified, and is validated according to PostgreSQL limits.
    /// </param>
    /// <param name="fractionalSecondsPrecision">
    /// The precision for fractional seconds in time-related types (TIME, TIMESTAMP). Must be greater than zero when specified, and is validated according to PostgreSQL limits.
    /// </param>
    /// <param name="isArray">
    /// Indicates whether the type should be treated as an array.
    /// </param>
    /// <param name="baseType">
    /// The base type for the field.
    /// </param>
    /// <param name="nullable">
    /// Indicates whether the field is nullable.
    /// </param>
    /// <returns>The field properties for the requested PostgreSQL provider type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a size, precision, or scale value is outside the supported PostgreSQL range.</exception>
    /// <exception cref="ArgumentException">Thrown when the requested provider type and modifiers form an invalid PostgreSQL field definition.</exception>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance based on the provided CLR type, with optional explicit nullability override.
    /// </summary>
    /// <param name="clrType">The CLR type for which to create field properties.</param>
    /// <param name="nullable">Indicates whether the field is nullable.</param>
    /// <returns>The created <see cref="FieldProperties"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided CLR type is not supported.</exception>
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

    /// <summary>
    /// Helper method to create <see cref="FieldProperties"/> from a CLR type, with explicit array and nullability information.
    /// </summary>
    /// <param name="type">The CLR type for which to create field properties.</param>
    /// <param name="isArray">Indicates whether the field is an array.</param>
    /// <param name="nullable">Indicates whether the field is nullable.</param>
    /// <returns>The created <see cref="FieldProperties"/> instance.</returns>
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

    /// <summary>
    /// Validates that a nullable integer value falls within a specified range, with an option to allow null values.
    /// </summary>
    /// <param name="value">
    /// The nullable integer value to validate. If <paramref name="allowNullValue"/> is <see langword="true"/>, this can be <see langword="null"/>; otherwise,
    /// it must have a value within the specified range.
    /// </param>
    /// <param name="allowNullValue">Indicates whether null values are allowed.</param>
    /// <param name="minValue">The minimum allowed value.</param>
    /// <param name="maxValue">The maximum allowed value.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a size, precision, or scale value is outside the supported PostgreSQL range.</exception>
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

    /// <summary>
    /// Validates that a nullable byte value falls within a specified range, with an option to allow null values.
    /// </summary>
    /// <param name="length">
    /// The nullable byte value to validate. If <paramref name="allowNullValue"/> is <see langword="true"/>, this can be <see langword="null"/>; otherwise,
    /// it must have a value within the specified range.
    /// </param>
    private static void ValidateCharacterLength(int? length) =>
        ValidateRange(length, true, 1, LIMIT_FOR_CHARACTER_LENGTH, nameof(length));

    /// <summary>
    /// Validates that a nullable integer value for bit length falls within the allowed range for PostgreSQL bit types.
    /// </summary>
    /// <param name="length">
    /// The nullable integer value representing the bit length to validate. If this value is specified (i.e., not null), it must be greater than zero and less
    /// than or equal to the defined limit for bit length.
    /// </param>
    private static void ValidateBitLength(int length) =>
        ValidateRange(length, false, 1, LIMIT_FOR_BIT_LENGTH, nameof(length));

    /// <summary>
    /// Validates that a nullable byte value for precision falls within the allowed range for PostgreSQL numeric types.
    /// </summary>
    /// <param name="precision">
    /// The nullable byte value representing the precision to validate. If this value is specified (i.e., not null), it must be greater than zero and less
    /// than or equal to the defined limit for numeric precision.
    /// </param>
    private static void ValidatePrecision(byte precision) =>
        ValidateRange(precision, false, 1, LIMIT_FOR_NUMERIC_PRECISION, nameof(precision));

    /// <summary>
    /// Validates a PostgreSQL numeric precision and scale pair before field metadata is created.
    /// </summary>
    /// <param name="precision">The total number of significant digits.</param>
    /// <param name="scale">The number of fractional digits.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="precision"/> is outside PostgreSQL's supported range or when <paramref name="scale"/> is greater than <paramref name="precision"/>.
    /// </exception>
    private static void ValidatePrecisionAndScale(byte precision, byte scale)
    {
        ValidatePrecision(precision);

        if (scale > precision)
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale cannot be greater than precision.");
    }

    /// <summary>
    /// Validates that a nullable byte value for fractional seconds precision falls within the allowed range for PostgreSQL time-related types.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">
    /// The nullable byte value representing the fractional seconds precision to validate. If this value is specified (i.e., not null), it must be greater than
    /// or equal to zero and less than or equal to the defined limit for fractional seconds precision.
    /// </param>
    private static void ValidateFractionalSecondsPrecision(byte fractionalSecondsPrecision) =>
        ValidateRange(fractionalSecondsPrecision, false, 0, LIMIT_FOR_FRACTIONAL_SECONDS_PRECISION, nameof(fractionalSecondsPrecision));

    /// <summary>
    /// Validates that a nullable byte value for floating-point precision falls within the allowed range for PostgreSQL floating-point types.
    /// </summary>
    /// <param name="precision">
    /// The nullable byte value representing the floating-point precision to validate. If this value is specified (i.e., not null), it must be greater than or equal to
    /// the defined minimum limit for float precision and less than or equal to the defined maximum limit for float precision.
    /// </param>
    private static void ValidateFloatPrecision(byte? precision) =>
        ValidateRange(precision, true, LIMIT_FOR_FLOAT_MIN_PRECISION, LIMIT_FOR_FLOAT_MAX_PRECISION, nameof(precision));

    /// <summary>
    /// Validates that an integer value for vector dimensions is greater than or equal to the defined minimum limit for vector dimensions.
    /// </summary>
    /// <param name="dimensions">
    /// The integer value representing the vector dimensions to validate. It must be greater than or equal to the defined minimum limit for vector dimensions.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the provided dimensions value is less than the defined minimum limit for vector dimensions.
    /// </exception>
    private static void ValidateVectorDimensions(int dimensions)
    {
        if (dimensions < LIMIT_FOR_VECTOR_MIN_DIMENSIONS)
        {
            throw new ArgumentOutOfRangeException(nameof(dimensions), dimensions, "Vector dimensions must be greater than zero.");
        }
    }

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for a numeric type (NUMERIC) with optional array and nullability settings.
    /// </summary>
    /// <param name="isArray">
    /// Indicates whether the type should be treated as an array. If <see langword="true"/>, the resulting <see cref="FieldProperties"/> will indicate that it
    /// represents an array of NUMERIC types; if <see langword="false"/>, it will represent a single NUMERIC type.
    /// </param>
    /// <param name="nullable">
    /// The explicit SQL nullability override. If set to <see langword="true"/>, the resulting <see cref="FieldProperties"/> will indicate that the type is nullable;
    /// if set to <see langword="false"/>, it will indicate that the type is not nullable; if set to <c>null</c>, the default nullability will be applied (which is
    /// defined as non-nullable in this provider).
    /// </param>
    /// <returns>
    /// A <see cref="FieldProperties"/> instance representing the PostgreSQL NUMERIC type, configured according to the specified array and nullability settings.
    /// </returns>
    private static FieldProperties CreateNumericType(bool isArray, bool? nullable) =>
        Create("NUMERIC", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for a numeric type (NUMERIC) with specified precision and optional array and nullability settings.
    /// </summary>
    /// <param name="precision">
    /// The SQL precision to apply for the NUMERIC type. This value must be greater than zero and less than or equal to the defined limit for numeric precision.
    /// </param>
    /// <param name="isArray">
    /// Indicates whether the type should be treated as an array. If <see langword="true"/>, the resulting <see cref="FieldProperties"/> will indicate that it
    /// represents an array of NUMERIC types; if <see langword="false"/>, it will represent a single NUMERIC type.
    /// </param>
    /// <param name="nullable">
    /// The explicit SQL nullability override. If set to <see langword="true"/>, the resulting <see cref="FieldProperties"/> will indicate that the type is nullable;
    /// if set to <see langword="false"/>, it will indicate that the type is not nullable; if set to <c>null</c>, the default nullability will be applied.
    /// </param>
    /// <returns>
    /// A <see cref="FieldProperties"/> instance representing the PostgreSQL NUMERIC type, configured according to the specified array and nullability settings.
    /// </returns>
    private static FieldProperties CreateNumericType(byte precision, bool isArray, bool? nullable)
    {
        ValidatePrecision(precision);

        return Create(
            providerTypeName: "NUMERIC",
            precision: precision,
            isArray: isArray,
            nullable: nullable);
    }


    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for a numeric type (NUMERIC) with specified precision and scale, along with optional array and nullability settings.
    /// </summary>
    /// <param name="precision">
    /// The SQL precision to apply for the NUMERIC type. This value must be greater than zero and less than or equal to the defined limit for numeric precision.
    /// </param>
    /// <param name="scale">
    /// The SQL scale to apply for the NUMERIC type. This value must be greater than or equal to zero and less than or equal to the specified precision.
    /// </param>
    /// <param name="isArray">
    /// Indicates whether the type should be treated as an array. If <see langword="true"/>, the resulting <see cref="FieldProperties"/> will indicate that it
    /// represents an array of NUMERIC types; if <see langword="false"/>, it will represent a single NUMERIC type.
    /// </param>
    /// <param name="nullable">
    /// The explicit SQL nullability override. If set to <see langword="true"/>, the resulting <see cref="FieldProperties"/> will indicate that the type is nullable;
    /// if set to <see langword="false"/>, it will indicate that the type is not nullable; if set to <c>null</c>, the default nullability will be applied.
    /// </param>
    /// <returns>
    /// A <see cref="FieldProperties"/> instance representing the PostgreSQL NUMERIC type, configured according to the specified array and nullability settings.
    /// </returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL provider type mapped from the supplied CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type to map to a PostgreSQL provider type.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL provider type mapped from the supplied CLR type.</returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL UUID type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL UUID type.</returns>
    public static FieldProperties AsUuid(bool isArray, bool? nullable = null) =>
        Create("UUID", isArray: isArray, nullable: nullable);

    #endregion

    #region Character Types

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL CHAR type.
    /// </summary>
    /// <param name="length">The SQL length to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL CHAR type.</returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL VARCHAR type.
    /// </summary>
    /// <param name="length">The SQL length to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL VARCHAR type.</returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TEXT type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TEXT type.</returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL BYTEA type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL BYTEA type.</returns>
    public static FieldProperties AsBytea(bool isArray, bool? nullable = null) =>
        Create(
            providerTypeName: "BYTEA",
            isMax: true,
            isFixedLength: false,
            isArray: isArray,
            nullable: nullable);

    #endregion

    #region Boolean

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL BOOLEAN type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL BOOLEAN type.</returns>
    public static FieldProperties AsBoolean(bool isArray, bool? nullable = null) =>
        Create("BOOLEAN", isArray: isArray, nullable: nullable);

    #endregion

    #region Integers

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL SMALLINT type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL SMALLINT type.</returns>
    public static FieldProperties AsSmallInt(bool isArray, bool? nullable = null) =>
        Create("SMALLINT", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL INTEGER type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL INTEGER type.</returns>
    public static FieldProperties AsInteger(bool isArray, bool? nullable = null) =>
        Create("INTEGER", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL BIGINT type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL BIGINT type.</returns>
    public static FieldProperties AsBigInt(bool isArray, bool? nullable = null) =>
        Create("BIGINT", isArray: isArray, nullable: nullable);

    #endregion

    #region Floating Points

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL REAL type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL REAL type.</returns>
    public static FieldProperties AsReal(bool isArray, bool? nullable = null) =>
        Create("REAL", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL DOUBLE PRECISION type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL DOUBLE PRECISION type.</returns>
    public static FieldProperties AsDoublePrecision(bool isArray, bool? nullable = null) =>
        Create("DOUBLE PRECISION", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL FLOAT type.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL FLOAT type.</returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL NUMERIC type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL NUMERIC type.</returns>
    public static FieldProperties AsNumeric(bool isArray, bool? nullable = null) =>
        CreateNumericType(isArray, nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL NUMERIC type.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL NUMERIC type.</returns>
    public static FieldProperties AsNumeric(byte precision, bool isArray, bool? nullable = null) =>
        CreateNumericType(precision, isArray, nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL NUMERIC type.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    /// <param name="scale">The SQL scale to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL NUMERIC type.</returns>
    public static FieldProperties AsNumeric(byte precision, byte scale, bool isArray, bool? nullable = null) =>
        CreateNumericType(precision, scale, isArray, nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL MONEY type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL MONEY type.</returns>
    public static FieldProperties AsMoney(bool isArray, bool? nullable = null) =>
        Create("MONEY", isArray: isArray, nullable: nullable);

    #endregion

    #region Date/Time

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL DATE type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL DATE type.</returns>
    public static FieldProperties AsDate(bool isArray, bool? nullable = null) =>
        Create("DATE", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIME type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIME type.</returns>
    public static FieldProperties AsTime(bool isArray, bool? nullable = null) =>
        Create("TIME", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIME type.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIME type.</returns>
    public static FieldProperties AsTime(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIME WITHOUT TIME ZONE type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIME WITHOUT TIME ZONE type.</returns>
    public static FieldProperties AsTimeWithoutTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIME WITHOUT TIME ZONE", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIME WITHOUT TIME ZONE type.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIME WITHOUT TIME ZONE type.</returns>
    public static FieldProperties AsTimeWithoutTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME WITHOUT TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIME WITH TIME ZONE type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIME WITH TIME ZONE type.</returns>
    public static FieldProperties AsTimeWithTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIME WITH TIME ZONE", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIME WITH TIME ZONE type.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIME WITH TIME ZONE type.</returns>
    public static FieldProperties AsTimeWithTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME WITH TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIMESTAMP type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIMESTAMP type.</returns>
    public static FieldProperties AsTimestamp(bool isArray, bool? nullable = null) =>
        Create("TIMESTAMP", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIMESTAMP type.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIMESTAMP type.</returns>
    public static FieldProperties AsTimestamp(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIMESTAMP WITHOUT TIME ZONE type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIMESTAMP WITHOUT TIME ZONE type.</returns>
    public static FieldProperties AsTimestampWithoutTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIMESTAMP WITHOUT TIME ZONE", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIMESTAMP WITHOUT TIME ZONE type.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIMESTAMP WITHOUT TIME ZONE type.</returns>
    public static FieldProperties AsTimestampWithoutTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP WITHOUT TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIMESTAMP WITH TIME ZONE type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIMESTAMP WITH TIME ZONE type.</returns>
    public static FieldProperties AsTimestampWithTimeZone(bool isArray, bool? nullable = null) =>
        Create("TIMESTAMP WITH TIME ZONE", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL TIMESTAMP WITH TIME ZONE type.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL TIMESTAMP WITH TIME ZONE type.</returns>
    public static FieldProperties AsTimestampWithTimeZone(byte fractionalSecondsPrecision, bool isArray, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIMESTAMP WITH TIME ZONE",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            isArray: isArray,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL INTERVAL type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL INTERVAL type.</returns>
    public static FieldProperties AsInterval(bool isArray, bool? nullable = null) =>
        Create("INTERVAL", isArray: isArray, nullable: nullable);

    #endregion

    #region XML/JSON

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL XML type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL XML type.</returns>
    public static FieldProperties AsXml(bool isArray, bool? nullable = null) =>
        Create("XML", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL JSON type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL JSON type.</returns>
    public static FieldProperties AsJson(bool isArray, bool? nullable = null) =>
        Create("JSON", isArray: isArray, nullable: nullable);

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL JSONB type.
    /// </summary>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL JSONB type.</returns>
    public static FieldProperties AsJsonB(bool isArray, bool? nullable = null) =>
        Create("JSONB", isArray: isArray, nullable: nullable);

    #endregion

    #region Bit Strings

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL BIT type.
    /// </summary>
    /// <param name="length">The SQL length to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL BIT type.</returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL VARBIT type.
    /// </summary>
    /// <param name="length">The SQL length to apply.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL VARBIT type.</returns>
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

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the PostgreSQL VECTOR type.
    /// </summary>
    /// <param name="dimensions">The vector dimension count.</param>
    /// <param name="isArray">Indicates whether the PostgreSQL provider type should be marked as an array.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the PostgreSQL VECTOR type.</returns>
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
    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for PostgreSQL unknown type inference.
    /// </summary>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for PostgreSQL unknown type inference.</returns>
    public static FieldProperties AsUnknown(bool? nullable) =>
        Create("UNKNOWN", nullable: nullable);
    #endregion

    #region Provider Specific

    /// <summary>
    /// Creates a <see cref="FieldProperties"/> instance for the specified PostgreSQL provider-specific type.
    /// </summary>
    /// <param name="providerTypeName">The provider-specific SQL type name.</param>
    /// <param name="nullable">The explicit SQL nullability override, or <c>null</c> to use the default.</param>
    /// <returns>A <see cref="FieldProperties"/> instance configured for the specified PostgreSQL provider-specific type.</returns>
    public static FieldProperties AsProviderSpecific(string providerTypeName, bool? nullable = null) =>
        Create(providerTypeName, nullable: nullable);

    #endregion
}
