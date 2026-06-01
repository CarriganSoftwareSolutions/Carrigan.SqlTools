using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server character column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlCharAttribute : SqlTypeAttribute
{
    private const int DefaultAsciiLength = 8000;
    private const int DefaultUnicodeLength = 4000;

    public SqlCharAttribute(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum) : base(GetFieldProperties(encodingEnum, storageTypeEnum))
    { }

    public SqlCharAttribute(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum, int size) : base(GetFieldProperties(encodingEnum, storageTypeEnum, size))
    { }

    private static FieldProperties GetFieldProperties(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum, int? size = null) =>
        (encodingEnum, storageTypeEnum) switch
        {
            (EncodingEnum.Ascii, StorageTypeEnum.Fixed) => size is null ? SqlServerTypesProvider.AsChar(DefaultAsciiLength) : SqlServerTypesProvider.AsChar(size.Value),
            (EncodingEnum.Ascii, StorageTypeEnum.Var) => size is null ? SqlServerTypesProvider.AsVarCharMax() : SqlServerTypesProvider.AsVarChar(size.Value),
            (EncodingEnum.Unicode, StorageTypeEnum.Fixed) => size is null ? SqlServerTypesProvider.AsNChar(DefaultUnicodeLength) : SqlServerTypesProvider.AsNChar(size.Value),
            (EncodingEnum.Unicode, StorageTypeEnum.Var) => size is null ? SqlServerTypesProvider.AsNVarCharMax() : SqlServerTypesProvider.AsNVarChar(size.Value),
            _ => throw new NotSupportedException($"Unsupported encoding '{encodingEnum}' with storage type '{storageTypeEnum}'."),
        };
}
