using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

//TODO: Proof read Documentation and Unit Tests

/// <summary>
/// Specifies that a property represents a character string column 
/// fixed-length ASCII <c>CHAR</c>
/// variable-length ASCII <c>VARCHAR</c>
/// fixed-length UNICODE <c>NCHAR</c> 
/// variable-length UNICODE <c>NVARCHAR</c>
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a char type column on a table model,
/// including whether the column uses fixed or variable storage, and optionally its size.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlCharAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBinaryAttribute"/> class that configures the
    /// associated property to represent a  
    /// fixed-length ASCII <c>CHAR</c>
    /// variable-length ASCII <c>VARCHAR</c>
    /// fixed-length UNICODE <c>NCHAR</c> 
    /// variable-length UNICODE <c>NVARCHAR</c>
    /// </summary>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>BINARY</c> storage or variable-length <c>VARBINARY</c> storage.
    /// </param>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="encoding">
    /// Indicates whether the column uses ASCII <c>CHAR</c> or <c>VARCHAR</c> encoding or UNICODE <c>NCHAR</c> or <c>NVARCHAR</c> encoding.
    /// </param>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>CHAR</c> or <c>NCHAR</c> storage or variable-length <c>VARCHAR</c> or <c>NVARCHAR</c> storage.
    /// </param>
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum) : base(GetSqlTypeDefinition(encoding, storageTypeEnum))
    {

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBinaryAttribute"/> class that configures the
    /// associated property to represent a  
    /// fixed-length ASCII <c>CHAR</c>
    /// variable-length ASCII <c>VARCHAR</c>
    /// fixed-length UNICODE <c>NCHAR</c> 
    /// variable-length UNICODE <c>NVARCHAR</c>, 
    /// with an explicit size.
    /// </summary>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>BINARY</c> storage or variable-length <c>VARBINARY</c> storage.
    /// </param>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="encoding">
    /// Indicates whether the column uses ASCII <c>CHAR</c> or <c>VARCHAR</c> encoding or UNICODE <c>NCHAR</c> or <c>NVARCHAR</c> encoding.
    /// </param>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>CHAR</c> or <c>NCHAR</c> storage or variable-length <c>VARCHAR</c> or <c>NVARCHAR</c> storage.
    /// </param>
    /// <param name="size">
    /// Indicate the SQL size for the column associated with the property.
    /// </param>
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int size) : base(GetSqlTypeDefinition(encoding, storageTypeEnum, size))
    {

    }

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
