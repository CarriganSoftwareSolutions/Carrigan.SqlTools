using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a binary SQL column (fixed-length <c>BINARY</c> or variable-length
/// <c>VARBINARY</c>) and overrides the default SQL type mapping for that property.
/// </summary>
/// <remarks>
/// <para>
/// In Carrigan.SqlTools, this attribute supplies the <see cref="SqlTypeDefinition"/> used by the SQL generator
/// when emitting column declarations and parameters.
/// </para>
/// <para>
/// When an explicit size is not provided, this attribute uses the defaults produced by
/// <see cref="SqlTypeDefinition.AsBinary(int?)"/> and <see cref="SqlTypeDefinition.AsVarBinary(int?)"/>:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="StorageTypeEnum.Fixed"/> produces <c>BINARY(8000)</c>.</description></item>
/// <item><description><see cref="StorageTypeEnum.Var"/> produces <c>VARBINARY(MAX)</c>.</description></item>
/// </list>
/// <para>
/// When an explicit size is provided, the valid range is <c>1</c>–<c>8000</c> bytes inclusive for both
/// <c>BINARY(n)</c> and <c>VARBINARY(n)</c>.
/// </para>
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
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="StorageTypeEnum"/> value is supplied. This typically indicates that the
    /// enumeration has been extended without updating this attribute.
    /// </exception>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum)
        : base(GetSqlTypeDefinition(storageTypeEnum))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBinaryAttribute"/> class that configures the
    /// associated property to represent a binary column using either fixed-length <c>BINARY</c> or
    /// variable-length <c>VARBINARY</c> storage, with an explicit size.
    /// </summary>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>BINARY</c> storage or variable-length <c>VARBINARY</c> storage.
    /// </param>
    /// <param name="size">
    /// The size of the binary column in bytes. The valid range is <c>1</c>–<c>8000</c> inclusive.
    /// </param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is outside the valid range for the selected SQL type.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="StorageTypeEnum"/> value is supplied. This typically indicates that the
    /// enumeration has been extended without updating this attribute.
    /// </exception>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum, int size)
        : base(GetSqlTypeDefinition(storageTypeEnum, size))
    { }

    /// <summary>
    /// Creates the <see cref="SqlTypeDefinition"/> corresponding to the requested binary storage type and optional size.
    /// </summary>
    /// <param name="storageTypeEnum">
    /// Indicates whether the column uses fixed-length <c>BINARY</c> storage or variable-length <c>VARBINARY</c> storage.
    /// </param>
    /// <param name="size">
    /// An optional size in bytes. When <see langword="null"/>, the default size behavior for the selected SQL type is used.
    /// </param>
    /// <returns>
    /// A <see cref="SqlTypeDefinition"/> representing the specified binary storage configuration.
    /// </returns>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is outside the valid range for the selected SQL type.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="StorageTypeEnum"/> value is supplied. This typically indicates that the
    /// enumeration has been extended without updating this method.
    /// </exception>
    private static SqlTypeDefinition GetSqlTypeDefinition(StorageTypeEnum storageTypeEnum, int? size = null) =>
        storageTypeEnum switch
        {
            StorageTypeEnum.Fixed => SqlTypeDefinition.AsBinary(size),
            StorageTypeEnum.Var => SqlTypeDefinition.AsVarBinary(size),
            _ => throw new NotSupportedException($"Unsupported storage type '{storageTypeEnum}'.")
        };
}
