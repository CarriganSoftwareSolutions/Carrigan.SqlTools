// IGNORE SPELLING: bigint, binary, bit, char, cursor, date, datetime, datetime2, datetimeoffset, decimal, float, geography, geometry, hierarchyid, image, int, json, money, nchar, ntext, numeric, nvarchar, real, rowversion, smalldatetime, smallint, smallmoney, sql_variant, table, text, time, timestamp, tinyint, uniqueidentifier, varbinary, varchar, vector, xml

using System;
using System.Xml;
using System.Xml.Linq;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Dialects.SqlServer;

/// <summary>
/// Provides predefined <see cref="FieldProperties"/> values for SQL Server data types.
/// </summary>
public static class SqlServerTypesProvider
{
    /// <summary>
    /// Represents the max integer value for <c>BINARY</c> and <c>VARBINARY</c>.
    /// </summary>
    private const int LIMIT_FOR_BYTE_ARRAY = 8000;

    /// <summary>
    /// Represents the max integer value for <c>CHAR</c> and <c>VARCHAR</c>.
    /// </summary>
    private const int LIMIT_FOR_ASCII = 8000;

    /// <summary>
    /// Represents the max integer value for <c>NCHAR</c> and <c>NVARCHAR</c>.
    /// </summary>
    private const int LIMIT_FOR_UNICODE = 4000;

    /// <summary>
    /// Represents the default nullability used when no explicit nullability value is supplied.
    /// </summary>
    private const bool DEFAULT_IS_NULLABLE = false;

    /// <summary>
    /// Represents the maximum precision allowed by SQL Server <c>DECIMAL</c>.
    /// </summary>
    private const byte LIMIT_FOR_DECIMAL_PRECISION = 38;

    /// <summary>
    /// Represents the maximum fractional seconds precision allowed by SQL Server temporal types.
    /// </summary>
    private const byte LIMIT_FOR_FRACTIONAL_SECONDS_PRECISION = 7;

    /// <summary>
    /// Represents the minimum precision allowed by SQL Server <c>FLOAT</c>.
    /// </summary>
    private const byte LIMIT_FOR_FLOAT_MIN_PRECISION = 1;

    /// <summary>
    /// Represents the maximum precision allowed by SQL Server <c>FLOAT</c>.
    /// </summary>
    private const byte LIMIT_FOR_FLOAT_MAX_PRECISION = 53;

    /// <summary>
    /// Represents the minimum dimensions allowed by SQL Server <c>VECTOR</c>.
    /// </summary>
    private const int LIMIT_FOR_VECTOR_MIN_DIMENSIONS = 1;

    /// <summary>
    /// Represents the maximum dimensions allowed by SQL Server <c>VECTOR</c>.
    /// </summary>
    private const int LIMIT_FOR_VECTOR_MAX_DIMENSIONS = 1998;

    #region Helper Methods

    /// <summary>
    /// Creates a SQL Server field definition.
    /// </summary>
    private static FieldProperties Create(
        string providerTypeName,
        int? length = null,
        bool? isMax = null,
        bool? isUnicode = null,
        bool? isFixedLength = null,
        byte? precision = null,
        byte? scale = null,
        byte? fractionalSecondsPrecision = null,
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
            ProviderTypeName = providerTypeName.ToUpperInvariant()
        };
    }

    /// <summary>
    /// Creates a SQL Server field definition using the default mapping for a CLR type.
    /// </summary>
    private static FieldProperties Create(Type clrType, bool? nullable = null)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        Type type = Nullable.GetUnderlyingType(clrType) ?? clrType;
        bool? effectiveNullable = nullable ?? (Nullable.GetUnderlyingType(clrType) is not null ? true : null);

        if (type == typeof(Guid)) return AsUniqueIdentifier(effectiveNullable);
        if (type == typeof(string)) return AsNVarCharMax(effectiveNullable);
        if (type == typeof(char)) return AsNChar(1, effectiveNullable);
        if (type == typeof(byte[])) return AsVarBinaryMax(effectiveNullable);

        if (type == typeof(bool)) return AsBit(effectiveNullable);

        if (type == typeof(byte)) return AsTinyInt(effectiveNullable);
        if (type == typeof(sbyte)) return AsSmallInt(effectiveNullable);
        if (type == typeof(short)) return AsSmallInt(effectiveNullable);
        if (type == typeof(ushort)) return AsInt(effectiveNullable);
        if (type == typeof(int)) return AsInt(effectiveNullable);
        if (type == typeof(uint)) return AsBigInt(effectiveNullable);
        if (type == typeof(long)) return AsBigInt(effectiveNullable);
        if (type == typeof(ulong)) return AsDecimal(20, 0, effectiveNullable);

        if (type == typeof(float)) return AsReal(effectiveNullable);
        if (type == typeof(double)) return AsFloat(effectiveNullable);
        if (type == typeof(decimal)) return AsDecimal(effectiveNullable);

        if (type == typeof(DateTime)) return AsDateTime2(effectiveNullable);
        if (type == typeof(DateOnly)) return AsDate(effectiveNullable);
        if (type == typeof(TimeOnly)) return AsTime(effectiveNullable);
        if (type == typeof(DateTimeOffset)) return AsDateTimeOffset(effectiveNullable);

        if (type == typeof(XmlDocument)) return AsXml(effectiveNullable);
        if (type == typeof(XDocument)) return AsXml(effectiveNullable);

        return AsSqlVariant(effectiveNullable);
    }

    /// <summary>
    /// Validates that an integer value is inside the specified range.
    /// </summary>
    private static void ValidateRange(int value, int minValue, int maxValue, string parameterName)
    {
        if (value < minValue || value > maxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {minValue} and {maxValue}.");
        }
    }

    /// <summary>
    /// Validates SQL Server decimal precision.
    /// </summary>
    private static void ValidatePrecision(byte precision) => ValidateRange(precision, 1, LIMIT_FOR_DECIMAL_PRECISION, nameof(precision));

    /// <summary>
    /// Validates SQL Server decimal precision and scale.
    /// </summary>
    private static void ValidatePrecisionAndScale(byte precision, byte scale)
    {
        ValidatePrecision(precision);

        if (scale > precision)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale cannot be greater than precision.");
        }
    }

    /// <summary>
    /// Validates SQL Server fractional seconds precision.
    /// </summary>
    private static void ValidateFractionalSecondsPrecision(byte fractionalSecondsPrecision) =>
        ValidateRange(fractionalSecondsPrecision, 0, LIMIT_FOR_FRACTIONAL_SECONDS_PRECISION, nameof(fractionalSecondsPrecision));

    /// <summary>
    /// Validates SQL Server float precision.
    /// </summary>
    private static void ValidateFloatPrecision(byte precision) =>
        ValidateRange(precision, LIMIT_FOR_FLOAT_MIN_PRECISION, LIMIT_FOR_FLOAT_MAX_PRECISION, nameof(precision));

    /// <summary>
    /// Validates SQL Server vector dimensions.
    /// </summary>
    private static void ValidateVectorDimensions(int dimensions) =>
        ValidateRange(dimensions, LIMIT_FOR_VECTOR_MIN_DIMENSIONS, LIMIT_FOR_VECTOR_MAX_DIMENSIONS, nameof(dimensions));

    /// <summary>
    /// Validates SQL Server vector base type.
    /// </summary>
    private static void ValidateVectorBaseType(string baseType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseType);

        if (!baseType.Equals("FLOAT32", StringComparison.OrdinalIgnoreCase) && !baseType.Equals("FLOAT16", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentOutOfRangeException(nameof(baseType), baseType, "Vector base type must be either FLOAT32 or FLOAT16.");
        }
    }

    /// <summary>
    /// Creates a decimal field definition without precision or scale.
    /// </summary>
    private static FieldProperties CreateDecimalType(bool? nullable) => Create("DECIMAL", nullable: nullable);

    /// <summary>
    /// Creates a decimal field definition with precision.
    /// </summary>
    private static FieldProperties CreateDecimalType(byte precision, bool? nullable)
    {
        ValidatePrecision(precision);

        return Create(
            providerTypeName: "DECIMAL",
            precision: precision,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a decimal field definition with precision and scale.
    /// </summary>
    private static FieldProperties CreateDecimalType(byte precision, byte scale, bool? nullable)
    {
        ValidatePrecisionAndScale(precision, scale);

        return Create(
            providerTypeName: "DECIMAL",
            precision: precision,
            scale: scale,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a vector field definition.
    /// </summary>
    private static FieldProperties CreateVectorType(int dimensions, string? baseType, bool? nullable)
    {
        ValidateVectorDimensions(dimensions);

        if (baseType is not null)
        {
            ValidateVectorBaseType(baseType);
        }

        return Create(
            providerTypeName: baseType is null ? "VECTOR" : $"VECTOR({dimensions}, {baseType.ToUpperInvariant()})",
            length: baseType is null ? dimensions : null,
            nullable: nullable);
    }

    #endregion

    #region CLR Types

    /// <summary>
    /// Creates a SQL Server field definition using the default mapping for a CLR type.
    /// </summary>
    public static FieldProperties FromClrType(Type clrType) => Create(clrType);

    /// <summary>
    /// Creates a SQL Server field definition using the default mapping for a CLR type.
    /// </summary>
    public static FieldProperties FromClrType<T>() => FromClrType(typeof(T));

    /// <summary>
    /// Creates a nullable SQL Server field definition using the default mapping for a CLR type.
    /// </summary>
    public static FieldProperties FromNullableClrType(Type clrType) => Create(clrType, nullable: true);

    /// <summary>
    /// Creates a nullable SQL Server field definition using the default mapping for a CLR type.
    /// </summary>
    public static FieldProperties FromNullableClrType<T>() => FromNullableClrType(typeof(T));

    #endregion

    #region UniqueIdentifier

    /// <summary>
    /// Creates a <c>UNIQUEIDENTIFIER</c> field definition.
    /// </summary>
    public static FieldProperties AsUniqueIdentifier(bool? nullable = null) => Create("UNIQUEIDENTIFIER", nullable: nullable);

    #endregion

    #region Chars

    /// <summary>
    /// Creates a <c>CHAR(length)</c> field definition.
    /// </summary>
    public static FieldProperties AsChar(int length, bool? nullable = null)
    {
        ValidateRange(length, 1, LIMIT_FOR_ASCII, nameof(length));

        return Create(
            providerTypeName: "CHAR",
            length: length,
            isMax: false,
            isUnicode: false,
            isFixedLength: true,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <c>VARCHAR(length)</c> field definition.
    /// </summary>
    public static FieldProperties AsVarChar(int length, bool? nullable = null)
    {
        ValidateRange(length, 1, LIMIT_FOR_ASCII, nameof(length));

        return Create(
            providerTypeName: "VARCHAR",
            length: length,
            isMax: false,
            isUnicode: false,
            isFixedLength: false,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <c>VARCHAR(MAX)</c> field definition.
    /// </summary>
    public static FieldProperties AsVarCharMax(bool? nullable = null) =>
        Create(
            providerTypeName: "VARCHAR",
            isMax: true,
            isUnicode: false,
            isFixedLength: false,
            nullable: nullable);

    /// <summary>
    /// Creates an <c>NCHAR(length)</c> field definition.
    /// </summary>
    public static FieldProperties AsNChar(int length, bool? nullable = null)
    {
        ValidateRange(length, 1, LIMIT_FOR_UNICODE, nameof(length));

        return Create(
            providerTypeName: "NCHAR",
            length: length,
            isMax: false,
            isUnicode: true,
            isFixedLength: true,
            nullable: nullable);
    }

    /// <summary>
    /// Creates an <c>NVARCHAR(length)</c> field definition.
    /// </summary>
    public static FieldProperties AsNVarChar(int length, bool? nullable = null)
    {
        ValidateRange(length, 1, LIMIT_FOR_UNICODE, nameof(length));

        return Create(
            providerTypeName: "NVARCHAR",
            length: length,
            isMax: false,
            isUnicode: true,
            isFixedLength: false,
            nullable: nullable);
    }

    /// <summary>
    /// Creates an <c>NVARCHAR(MAX)</c> field definition.
    /// </summary>
    public static FieldProperties AsNVarCharMax(bool? nullable = null) =>
        Create(
            providerTypeName: "NVARCHAR",
            isMax: true,
            isUnicode: true,
            isFixedLength: false,
            nullable: nullable);

    /// <summary>
    /// Creates a <c>TEXT</c> field definition.
    /// </summary>
    [Obsolete("Use VARCHAR(MAX) instead of TEXT.")]
    public static FieldProperties AsText(bool? nullable = null) =>
        Create(
            providerTypeName: "TEXT",
            isMax: true,
            isUnicode: false,
            isFixedLength: false,
            nullable: nullable);

    /// <summary>
    /// Creates an <c>NTEXT</c> field definition.
    /// </summary>
    [Obsolete("Use NVARCHAR(MAX) instead of NTEXT.")]
    public static FieldProperties AsNText(bool? nullable = null) =>
        Create(
            providerTypeName: "NTEXT",
            isMax: true,
            isUnicode: true,
            isFixedLength: false,
            nullable: nullable);

    #endregion

    #region Binary

    /// <summary>
    /// Creates a <c>BINARY(length)</c> field definition.
    /// </summary>
    public static FieldProperties AsBinary(int length, bool? nullable = null)
    {
        ValidateRange(length, 1, LIMIT_FOR_BYTE_ARRAY, nameof(length));

        return Create(
            providerTypeName: "BINARY",
            length: length,
            isMax: false,
            isFixedLength: true,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <c>VARBINARY(length)</c> field definition.
    /// </summary>
    public static FieldProperties AsVarBinary(int length, bool? nullable = null)
    {
        ValidateRange(length, 1, LIMIT_FOR_BYTE_ARRAY, nameof(length));

        return Create(
            providerTypeName: "VARBINARY",
            length: length,
            isMax: false,
            isFixedLength: false,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <c>VARBINARY(MAX)</c> field definition.
    /// </summary>
    public static FieldProperties AsVarBinaryMax(bool? nullable = null) =>
        Create(
            providerTypeName: "VARBINARY",
            isMax: true,
            isFixedLength: false,
            nullable: nullable);

    /// <summary>
    /// Creates an <c>IMAGE</c> field definition.
    /// </summary>
    [Obsolete("Use VARBINARY(MAX) instead of IMAGE.")]
    public static FieldProperties AsImage(bool? nullable = null) =>
        Create(
            providerTypeName: "IMAGE",
            isMax: true,
            isFixedLength: false,
            nullable: nullable);

    /// <summary>
    /// Creates a <c>ROWVERSION</c> field definition.
    /// </summary>
    public static FieldProperties AsRowVersion(bool? nullable = null) =>
        Create(
            providerTypeName: "ROWVERSION",
            length: 8,
            isMax: false,
            isFixedLength: true,
            nullable: nullable);

    /// <summary>
    /// Creates a <c>TIMESTAMP</c> field definition.
    /// </summary>
    [Obsolete("TIMESTAMP is a deprecated synonym for ROWVERSION. Use AsRowVersion instead.")]
    public static FieldProperties AsTimestamp(bool? nullable = null) =>
        Create(
            providerTypeName: "TIMESTAMP",
            length: 8,
            isMax: false,
            isFixedLength: true,
            nullable: nullable);

    #endregion

    #region Bits

    /// <summary>
    /// Creates a <c>BIT</c> field definition.
    /// </summary>
    public static FieldProperties AsBit(bool? nullable = null) => Create("BIT", nullable: nullable);

    #endregion

    #region Integers

    /// <summary>
    /// Creates a <c>TINYINT</c> field definition.
    /// </summary>
    public static FieldProperties AsTinyInt(bool? nullable = null) => Create("TINYINT", nullable: nullable);

    /// <summary>
    /// Creates a <c>SMALLINT</c> field definition.
    /// </summary>
    public static FieldProperties AsSmallInt(bool? nullable = null) => Create("SMALLINT", nullable: nullable);

    /// <summary>
    /// Creates an <c>INT</c> field definition.
    /// </summary>
    public static FieldProperties AsInt(bool? nullable = null) => Create("INT", nullable: nullable);

    /// <summary>
    /// Creates a <c>BIGINT</c> field definition.
    /// </summary>
    public static FieldProperties AsBigInt(bool? nullable = null) => Create("BIGINT", nullable: nullable);

    #endregion

    #region Floating Points

    /// <summary>
    /// Creates a <c>REAL</c> field definition.
    /// </summary>
    public static FieldProperties AsReal(bool? nullable = null) => Create("REAL", nullable: nullable);

    /// <summary>
    /// Creates a <c>FLOAT</c> field definition.
    /// </summary>
    public static FieldProperties AsFloat(bool? nullable = null) => Create("FLOAT", nullable: nullable);

    /// <summary>
    /// Creates a <c>FLOAT(precision)</c> field definition.
    /// </summary>
    public static FieldProperties AsFloat(byte precision, bool? nullable = null)
    {
        ValidateFloatPrecision(precision);

        return Create(
            providerTypeName: "FLOAT",
            precision: precision,
            nullable: nullable);
    }

    #endregion

    #region Decimals

    /// <summary>
    /// Creates a <c>DECIMAL</c> field definition.
    /// </summary>
    public static FieldProperties AsDecimal(bool? nullable = null) => CreateDecimalType(nullable);

    /// <summary>
    /// Creates a <c>DECIMAL(precision)</c> field definition.
    /// </summary>
    public static FieldProperties AsDecimal(byte precision, bool? nullable = null) => CreateDecimalType(precision, nullable);

    /// <summary>
    /// Creates a <c>DECIMAL(precision, scale)</c> field definition.
    /// </summary>
    public static FieldProperties AsDecimal(byte precision, byte scale, bool? nullable = null) => CreateDecimalType(precision, scale, nullable);

    /// <summary>
    /// Creates a <c>MONEY</c> field definition.
    /// </summary>
    public static FieldProperties AsMoney(bool? nullable = null) => Create("MONEY", nullable: nullable);

    /// <summary>
    /// Creates a <c>SMALLMONEY</c> field definition.
    /// </summary>
    public static FieldProperties AsSmallMoney(bool? nullable = null) => Create("SMALLMONEY", nullable: nullable);

    #endregion

    #region Date/Time

    /// <summary>
    /// Creates a <c>DATE</c> field definition.
    /// </summary>
    public static FieldProperties AsDate(bool? nullable = null) => Create("DATE", nullable: nullable);

    /// <summary>
    /// Creates a <c>TIME</c> field definition.
    /// </summary>
    public static FieldProperties AsTime(bool? nullable = null) => Create("TIME", nullable: nullable);

    /// <summary>
    /// Creates a <c>TIME(fractionalSecondsPrecision)</c> field definition.
    /// </summary>
    public static FieldProperties AsTime(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "TIME",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <c>DATETIME</c> field definition.
    /// </summary>
    public static FieldProperties AsDateTime(bool? nullable = null) => Create("DATETIME", nullable: nullable);

    /// <summary>
    /// Creates a <c>DATETIME2</c> field definition.
    /// </summary>
    public static FieldProperties AsDateTime2(bool? nullable = null) => Create("DATETIME2", nullable: nullable);

    /// <summary>
    /// Creates a <c>DATETIME2(fractionalSecondsPrecision)</c> field definition.
    /// </summary>
    public static FieldProperties AsDateTime2(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "DATETIME2",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    /// <summary>
    /// Creates a <c>SMALLDATETIME</c> field definition.
    /// </summary>
    public static FieldProperties AsSmallDateTime(bool? nullable = null) => Create("SMALLDATETIME", nullable: nullable);

    /// <summary>
    /// Creates a <c>DATETIMEOFFSET</c> field definition.
    /// </summary>
    public static FieldProperties AsDateTimeOffset(bool? nullable = null) => Create("DATETIMEOFFSET", nullable: nullable);

    /// <summary>
    /// Creates a <c>DATETIMEOFFSET(fractionalSecondsPrecision)</c> field definition.
    /// </summary>
    public static FieldProperties AsDateTimeOffset(byte fractionalSecondsPrecision, bool? nullable = null)
    {
        ValidateFractionalSecondsPrecision(fractionalSecondsPrecision);

        return Create(
            providerTypeName: "DATETIMEOFFSET",
            fractionalSecondsPrecision: fractionalSecondsPrecision,
            nullable: nullable);
    }

    #endregion

    #region XML/JSON

    /// <summary>
    /// Creates an <c>XML</c> field definition.
    /// </summary>
    public static FieldProperties AsXml(bool? nullable = null) => Create("XML", nullable: nullable);

    /// <summary>
    /// Creates a <c>JSON</c> field definition.
    /// </summary>
    public static FieldProperties AsJson(bool? nullable = null) => Create("JSON", nullable: nullable);

    #endregion

    #region Spatial

    /// <summary>
    /// Creates a <c>GEOMETRY</c> field definition.
    /// </summary>
    public static FieldProperties AsGeometry(bool? nullable = null) => Create("GEOMETRY", nullable: nullable);

    /// <summary>
    /// Creates a <c>GEOGRAPHY</c> field definition.
    /// </summary>
    public static FieldProperties AsGeography(bool? nullable = null) => Create("GEOGRAPHY", nullable: nullable);

    #endregion

    #region Vector

    /// <summary>
    /// Creates a <c>VECTOR(dimensions)</c> field definition.
    /// </summary>
    public static FieldProperties AsVector(int dimensions, bool? nullable = null) => CreateVectorType(dimensions, null, nullable);

    /// <summary>
    /// Creates a <c>VECTOR(dimensions, baseType)</c> field definition.
    /// </summary>
    public static FieldProperties AsVector(int dimensions, string baseType, bool? nullable = null) => CreateVectorType(dimensions, baseType, nullable);

    #endregion

    #region Special Data Types

    /// <summary>
    /// Creates a <c>SQL_VARIANT</c> field definition.
    /// </summary>
    public static FieldProperties AsSqlVariant(bool? nullable = null) => Create("SQL_VARIANT", nullable: nullable);

    /// <summary>
    /// Creates a <c>HIERARCHYID</c> field definition.
    /// </summary>
    public static FieldProperties AsHierarchyId(bool? nullable = null) => Create("HIERARCHYID", nullable: nullable);

    /// <summary>
    /// Creates a <c>CURSOR</c> field definition.
    /// </summary>
    public static FieldProperties AsCursor(bool? nullable = null) => Create("CURSOR", nullable: nullable);

    /// <summary>
    /// Creates a <c>TABLE</c> field definition.
    /// </summary>
    public static FieldProperties AsTable(bool? nullable = null) => Create("TABLE", nullable: nullable);

    /// <summary>
    /// Creates a provider-specific SQL Server field definition.
    /// </summary>
    public static FieldProperties AsProviderSpecific(string providerTypeName, bool? nullable = null) => Create(providerTypeName, nullable: nullable);

    #endregion
}