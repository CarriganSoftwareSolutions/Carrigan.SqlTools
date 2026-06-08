// IGNORE SPELLING: binary, bit, cursor, date, decimal, float, geography, geometry, image, money, numeric, real, table, text, time, timestamp, vector

namespace Carrigan.SqlTools.Types;

/// <summary>
/// Describes dialect-specific SQL type metadata used to generate a column or parameter declaration.
/// </summary>
/// <remarks>
/// This class describes the database side of a mapped field, including the provider type name,
/// length, precision, scale, Unicode behavior, fixed-length behavior, fractional seconds precision,
/// provider base type, and nullability.
/// </remarks>
public sealed class FieldProperties
{
    /// <summary>
    /// Gets the length used by dialect types that accept a length argument, such as character, binary, bit-string, or vector types.
    /// </summary>
    public int? Length { get; init; }

    /// <summary>
    /// Gets a value indicating whether the type uses <c>MAX</c> instead of a numeric length.
    /// </summary>
    public bool? IsMax { get; init; }

    /// <summary>
    /// Gets a value indicating whether the type stores Unicode character data.
    /// </summary>
    public bool? IsUnicode { get; init; }

    /// <summary>
    /// Gets a value indicating whether the type uses fixed-length storage.
    /// </summary>
    public bool? IsFixedLength { get; init; }

    /// <summary>
    /// Gets the precision used by dialect types that support numeric precision.
    /// </summary>
    public byte? Precision { get; init; }

    /// <summary>
    /// Gets the scale used by dialect types that support numeric scale.
    /// </summary>
    public byte? Scale { get; init; }

    /// <summary>
    /// Gets the fractional seconds precision used by dialect time-related types.
    /// </summary>
    public byte? FractionalSecondsPrecision { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field allows SQL <c>NULL</c>.
    /// </summary>
    public bool IsNullable { get; init; }

    /// <summary>
    /// Gets the provider-specific SQL type name.
    /// </summary>
    /// <example>
    /// <c>NVARCHAR</c>, <c>DECIMAL</c>, <c>VARBINARY</c>, <c>UNIQUEIDENTIFIER</c>.
    /// </example>
    public string? ProviderTypeName { get; init; }

    /// <summary>
    /// Gets the provider base type for dialect types that support an additional base type argument.
    /// </summary>
    /// <example>
    /// <c>FLOAT32</c> or <c>FLOAT16</c> for <c>VECTOR</c>.
    /// </example>
    public string? BaseType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field is an array type.
    /// </summary>
    public bool? IsArray { get; set; }

}
