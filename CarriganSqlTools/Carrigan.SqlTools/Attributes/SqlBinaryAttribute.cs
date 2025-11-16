using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
//TODO: Proofread Documentation and Unit Tests

/// <summary>
/// Specifies that a property represents a binary column (fixed-length <c>BINARY</c> or variable-length <c>VARBINARY</c>)
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a binary column on a table model,
/// including whether the column uses fixed or variable storage, and optionally its size.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlBinaryAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBinaryAttribute"/> class that configures the
    /// associated property to represent a binary column using either fixed-length <c>BINARY</c> or
    /// variable-length <c>VARBINARY</c> storage.
    /// </summary>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>BINARY</c> storage or variable-length <c>VARBINARY</c> storage.
    /// </param>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum) : base (GetSqlTypeDefinition(storageTypeEnum))
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBinaryAttribute"/> class that configures the
    /// associated property to represent a binary column using either fixed-length <c>BINARY</c> or
    /// variable-length <c>VARBINARY</c> storage, with an explicit size.
    /// </summary>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>BINARY</c> storage or variable-length <c>VARBINARY</c> storage.
    /// </param>
    /// <param name="size">
    /// The size of the binary column, in bytes. The valid range and interpretation of this value are
    /// determined by the underlying SQL type definition logic.
    /// </param>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum, int size) : base(GetSqlTypeDefinition(storageTypeEnum, size))
    {

    }

    /// <summary>
    /// Creates the <see cref="SqlTypeDefinition"/> corresponding to the requested binary storage type and size.
    /// </summary>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>BINARY</c> storage or variable-length <c>VARBINARY</c> storage.
    /// </param>
    /// <param name="size">
    /// An optional size, in bytes, for the binary column. When <see langword="null"/>, the default
    /// size rules for the requested storage type are applied.
    /// </param>
    /// <returns>
    /// An <see cref="SqlTypeDefinition"/> representing the specified binary storage configuration.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="StorageTypeEnum"/> value is supplied. This typically
    /// indicates that the enumeration has been extended without updating this method.
    /// </exception>
    private static SqlTypeDefinition GetSqlTypeDefinition(StorageTypeEnum storageTypeEnum, int? size = null) => 
        storageTypeEnum switch
        {
            StorageTypeEnum.Fixed => SqlTypeDefinition.AsBinary(size),
            StorageTypeEnum.Var => SqlTypeDefinition.AsVarBinary(size),
            _ => throw new NotSupportedException()
        };
}
