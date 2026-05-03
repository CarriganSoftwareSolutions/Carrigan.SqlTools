// IGNORE SPELLING: bigint, binary, bit, char, cursor, date, datetime, datetime2, datetimeoffset, decimal, float, geography, geometry, hierarchyid, image, int, json, money, nchar, ntext, numeric, nvarchar, real, rowversion, smalldatetime, smallint, smallmoney, sql_variant, table, text, time, timestamp, tinyint, uniqueidentifier, varbinary, varchar, vector, xml

using System;

namespace Carrigan.SqlTools.Types;

/// <summary>
/// Represents SQL Server field type properties used to generate a SQL declaration.
/// </summary>
/// <remarks>
/// This class describes the SQL Server side of a mapped field, including the provider type name,
/// length, precision, scale, Unicode behavior, fixed-length behavior, fractional seconds precision,
/// and nullability.
/// </remarks>
public sealed class FieldProperties
{
    /// <summary>
    /// Gets the length used by SQL Server character, binary, and vector types.
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
    /// Gets the precision used by SQL Server decimal, numeric, and float types.
    /// </summary>
    public byte? Precision { get; init; }

    /// <summary>
    /// Gets the scale used by SQL Server decimal and numeric types.
    /// </summary>
    public byte? Scale { get; init; }

    /// <summary>
    /// Gets the fractional seconds precision used by SQL Server time-related types.
    /// </summary>
    public byte? FractionalSecondsPrecision { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field allows SQL <c>NULL</c>.
    /// </summary>
    public bool IsNullable { get; init; }

    /// <summary>
    /// Gets the SQL Server provider type name.
    /// </summary>
    /// <example>
    /// <c>NVARCHAR</c>, <c>DECIMAL</c>, <c>VARBINARY</c>, <c>UNIQUEIDENTIFIER</c>.
    /// </example>
    public string? ProviderTypeName { get; init; }

    /// <summary>
    /// Returns the SQL Server declaration for the field type.
    /// </summary>
    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(ProviderTypeName))
        {
            return string.Empty;
        }

        string declaration = ProviderTypeName.ToUpperInvariant();

        if (IsMax == true)
        {
            declaration += "(MAX)";
        }
        else if (Precision is not null && Scale is not null)
        {
            declaration += $"({Precision}, {Scale})";
        }
        else if (Precision is not null)
        {
            declaration += $"({Precision})";
        }
        else if (FractionalSecondsPrecision is not null)
        {
            declaration += $"({FractionalSecondsPrecision})";
        }
        else if (Length is not null && RequiresLengthDeclaration(declaration))
        {
            declaration += $"({Length})";
        }

        return $"{declaration} {(IsNullable ? "NULL" : "NOT NULL")}";
    }

    /// <summary>
    /// Determines whether the supplied SQL Server type name should include a length declaration.
    /// </summary>
    private static bool RequiresLengthDeclaration(string providerTypeName) =>
        providerTypeName is "CHAR" or "VARCHAR" or "NCHAR" or "NVARCHAR" or "BINARY" or "VARBINARY" or "VECTOR";
}