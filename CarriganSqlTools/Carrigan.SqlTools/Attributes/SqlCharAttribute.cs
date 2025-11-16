using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

//TODO: Documentation and Unit Tests
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlCharAttribute : SqlTypeAttribute
{
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum) : base(GetSqlDbType(encoding, storageTypeEnum))
    {

    }
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int size) : base(GetSqlDbType(encoding, storageTypeEnum, size))
    {

    }

    private static SqlTypeDefinition GetSqlDbType(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int? size = null) => 
        (encoding, storageTypeEnum) switch
        {
            (EncodingEnum.Ascii, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsChar(size),
            (EncodingEnum.Ascii, StorageTypeEnum.Var) => SqlTypeDefinition.AsVarChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsNChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Var) => SqlTypeDefinition.AsNVarChar(size),
            _ => throw new NotSupportedException(),
        };
}
