using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>BINARY</c> or <c>VARBINARY</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlBinaryAttribute : SqlTypeAttribute
{
    private const int DefaultBinaryLength = 8000;

    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum) : base(GetFieldProperties(storageTypeEnum))
    { }

    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum, int size) : base(GetFieldProperties(storageTypeEnum, size))
    { }

    private static FieldProperties GetFieldProperties(StorageTypeEnum storageTypeEnum, int? size = null) =>
        storageTypeEnum switch
        {
            StorageTypeEnum.Fixed => size is null ? SqlServerTypesProvider.AsBinary(DefaultBinaryLength) : SqlServerTypesProvider.AsBinary(size.Value),
            StorageTypeEnum.Var => size is null ? SqlServerTypesProvider.AsVarBinaryMax() : SqlServerTypesProvider.AsVarBinary(size.Value),
            _ => throw new NotSupportedException($"Unsupported storage type '{storageTypeEnum}'.")
        };
}
