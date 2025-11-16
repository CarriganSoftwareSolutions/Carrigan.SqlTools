using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a character string column—
/// fixed-length ASCII <c>CHAR</c>,
/// variable-length ASCII <c>VARCHAR</c>, 
/// fixed-length Unicode <c>NCHAR</c>, or 
/// variable-length Unicode <c>NVARCHAR</c>—
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a character-type column
/// on a table model, including whether the column uses ASCII or Unicode encoding, 
/// fixed-length or variable-length storage, and optionally its size.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlCharAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCharAttribute"/> class that configures the
    /// associated property to represent a character string column using the specified encoding and
    /// storage type.
    /// </summary>
    /// <param name="encoding">
    /// Indicates whether the column uses ASCII encoding (<c>CHAR</c> or <c>VARCHAR</c>)
    /// or Unicode encoding (<c>NCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length storage (<c>CHAR</c> or <c>NCHAR</c>)
    /// or variable-length storage (<c>VARCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum) : base(GetSqlTypeDefinition(encoding, storageTypeEnum))
    {

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCharAttribute"/> class that configures the
    /// associated property to represent a character string column using the specified encoding,
    /// storage type, and an explicit size.
    /// </summary>
    /// <param name="encoding">
    /// Indicates whether the column uses ASCII encoding (<c>CHAR</c> or <c>VARCHAR</c>)
    /// or Unicode encoding (<c>NCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length storage (<c>CHAR</c> or <c>NCHAR</c>)
    /// or variable-length storage (<c>VARCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="size">
    /// Indicates the SQL size for the character column represented by the property.
    /// </param>
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int size) : base(GetSqlTypeDefinition(encoding, storageTypeEnum, size))
    {

    }

    /// <summary>
    /// Creates the <see cref="SqlTypeDefinition"/> that represents the character-column metadata
    /// for the property decorated with <see cref="SqlCharAttribute"/>.  
    /// The returned type definition reflects the selected encoding (ASCII or Unicode),
    /// storage type (fixed or variable length), and an optional SQL size.
    /// </summary>
    /// <param name="encoding">
    /// Specifies whether the column uses ASCII encoding (<c>CHAR</c> or <c>VARCHAR</c>)
    /// or Unicode encoding (<c>NCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="storageTypeEnum">
    /// Specifies whether the column uses fixed-length storage (<c>CHAR</c> or <c>NCHAR</c>)
    /// or variable-length storage (<c>VARCHAR</c> or <c>NVARCHAR</c>).
    /// </param>
    /// <param name="size">
    /// The optional SQL length for the column, expressed in characters.  
    /// When <see langword="null"/>, the default sizing rules for the selected SQL type are applied.
    /// </param>
    /// <returns>
    /// A <see cref="SqlTypeDefinition"/> describing the SQL Server character type
    /// that should be generated for the column represented by the property.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported combination of <see cref="EncodingEnum"/> and 
    /// <see cref="StorageTypeEnum"/> is provided.  
    /// This typically indicates that the enumeration was extended without updating this method.
    /// </exception>
    private static SqlTypeDefinition GetSqlTypeDefinition(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int? size = null) => 
        (encoding, storageTypeEnum) switch
        {
            (EncodingEnum.Ascii, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsChar(size),
            (EncodingEnum.Ascii, StorageTypeEnum.Var) => SqlTypeDefinition.AsVarChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsNChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Var) => SqlTypeDefinition.AsNVarChar(size),
            _ => throw new NotSupportedException($"Unsupported encoding '{encoding}' with storage type '{storageTypeEnum}'."),
        };
}
