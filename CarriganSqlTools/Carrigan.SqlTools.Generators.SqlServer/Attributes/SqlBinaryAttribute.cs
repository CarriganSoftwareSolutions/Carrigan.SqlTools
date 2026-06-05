using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>BINARY</c> or <c>VARBINARY</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlBinaryAttribute : SqlTypeAttribute
{
    /// <summary>
    /// The default SQL Server length used for fixed-length binary fields when no size is specified.
    /// </summary>
    private const int DefaultBinaryLength = 8000;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBinaryAttribute"/> class.
    /// </summary>
    /// <param name="storageTypeEnum">The SQL storage type option.</param>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum) : base(GetFieldProperties(storageTypeEnum))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBinaryAttribute"/> class.
    /// </summary>
    /// <param name="storageTypeEnum">The SQL storage type option.</param>
    /// <param name="size">The SQL size to apply.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when the requested SQL type option is not supported.
    /// </exception>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum, int size) : base(GetFieldProperties(storageTypeEnum, size))
    { }


    /// <summary>
    /// Creates field-property metadata for SQL Server binary type metadata.
    /// </summary>
    /// <returns>The field metadata consumed by SQL type rendering.</returns>
    private static FieldProperties GetFieldProperties(StorageTypeEnum storageTypeEnum, int? size = null) =>
        storageTypeEnum switch
        {
            StorageTypeEnum.Fixed => size is null ? SqlServerTypesProvider.AsBinary(DefaultBinaryLength) : SqlServerTypesProvider.AsBinary(size.Value),
            StorageTypeEnum.Var => size is null ? SqlServerTypesProvider.AsVarBinaryMax() : SqlServerTypesProvider.AsVarBinary(size.Value),
            _ => throw new NotSupportedException($"Unsupported storage type '{storageTypeEnum}'.")
        };
}
