using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a character string SQL column—fixed-length ASCII <c>CHAR</c>,
/// variable-length ASCII <c>VARCHAR</c>, fixed-length Unicode <c>NCHAR</c>, or variable-length Unicode
/// <c>NVARCHAR</c>—and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// <para>
/// When <paramref name="size"/> is <see langword="null"/>, default sizing rules are applied:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="EncodingEnum.Ascii"/> + <see cref="StorageTypeEnum.Fixed"/> produces <c>CHAR(8000)</c>.</description></item>
/// <item><description><see cref="EncodingEnum.Ascii"/> + <see cref="StorageTypeEnum.Var"/> produces <c>VARCHAR(MAX)</c>.</description></item>
/// <item><description><see cref="EncodingEnum.Unicode"/> + <see cref="StorageTypeEnum.Fixed"/> produces <c>NCHAR(4000)</c>.</description></item>
/// <item><description><see cref="EncodingEnum.Unicode"/> + <see cref="StorageTypeEnum.Var"/> produces <c>NVARCHAR(MAX)</c>.</description></item>
/// </list>
/// <para>
/// When an explicit <paramref name="size"/> is provided (in characters):
/// </para>
/// <list type="bullet">
/// <item><description><c>CHAR(n)</c> and <c>VARCHAR(n)</c> allow sizes <c>1</c>–<c>8000</c>.</description></item>
/// <item><description><c>NCHAR(n)</c> and <c>NVARCHAR(n)</c> allow sizes <c>1</c>–<c>4000</c>.</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlCharAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCharAttribute"/> class that configures the
    /// associated property to represent a character string column using the specified encoding and
    /// storage type.
    /// </summary>
    /// <param name="encodingEnum">
    /// Indicates whether the column uses ASCII encoding (<c>CHAR</c> or <c>VARCHAR</c>) or Unicode encoding
    /// (<c>NCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length storage (<c>CHAR</c> or <c>NCHAR</c>) or variable-length
    /// storage (<c>VARCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported combination of <see cref="EncodingEnum"/> and <see cref="StorageTypeEnum"/> is provided.
    /// This typically indicates that the enumeration was extended without updating this attribute.
    /// </exception>
    public SqlCharAttribute(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum) : base(GetSqlTypeDefinition(encodingEnum, storageTypeEnum))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCharAttribute"/> class that configures the
    /// associated property to represent a character string column using the specified encoding,
    /// storage type, and an explicit size.
    /// </summary>
    /// <param name="encodingEnum">
    /// Indicates whether the column uses ASCII encoding (<c>CHAR</c> or <c>VARCHAR</c>) or Unicode encoding
    /// (<c>NCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length storage (<c>CHAR</c> or <c>NCHAR</c>) or variable-length
    /// storage (<c>VARCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="size">The SQL length for the column, expressed in characters.</param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is outside the valid range for the selected SQL type.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported combination of <see cref="EncodingEnum"/> and <see cref="StorageTypeEnum"/> is provided.
    /// This typically indicates that the enumeration was extended without updating this attribute.
    /// </exception>
    public SqlCharAttribute(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum, int size) : base(GetSqlTypeDefinition(encodingEnum, storageTypeEnum, size))
    { }

    /// <summary>
    /// Creates the <see cref="SqlTypeDefinition"/> that represents the character-column metadata for the
    /// property decorated with <see cref="SqlCharAttribute"/>.
    /// </summary>
    /// <param name="encodingEnum">
    /// Specifies whether the column uses ASCII encoding (<c>CHAR</c> or <c>VARCHAR</c>) or Unicode encoding
    /// (<c>NCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="storageTypeEnum">
    /// Specifies whether the column uses fixed-length storage (<c>CHAR</c> or <c>NCHAR</c>) or variable-length storage
    /// (<c>VARCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="size">
    /// The optional SQL length for the column, expressed in characters. When <see langword="null"/>, default sizing rules
    /// for the selected SQL type are applied.
    /// </param>
    /// <returns>
    /// A <see cref="SqlTypeDefinition"/> describing the SQL Server character type that should be generated for the column
    /// represented by the property.
    /// </returns>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is outside the valid range for the selected SQL type.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported combination of <see cref="EncodingEnum"/> and <see cref="StorageTypeEnum"/> is provided.
    /// This typically indicates that the enumeration was extended without updating this method.
    /// </exception>
    private static SqlTypeDefinition GetSqlTypeDefinition(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum, int? size = null) =>
        (encodingEnum, storageTypeEnum) switch
        {
            (EncodingEnum.Ascii, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsChar(size),
            (EncodingEnum.Ascii, StorageTypeEnum.Var) => SqlTypeDefinition.AsVarChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsNChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Var) => SqlTypeDefinition.AsNVarChar(size),
            _ => throw new NotSupportedException($"Unsupported encoding '{encodingEnum}' with storage type '{storageTypeEnum}'."),
        };
}
