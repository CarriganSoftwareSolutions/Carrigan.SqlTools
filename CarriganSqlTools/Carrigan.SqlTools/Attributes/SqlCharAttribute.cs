using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

//TODO: Documentation and Unit Tests
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlCharAttribute : SqlTypeAttribute
{
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum) : base(GetSqlTypeDefinition(encoding, storageTypeEnum))
    {

    }
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
