using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlCharAttribute : SqlTypeAttribute
{
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum) : base (GetSqlSbType(encoding, storageTypeEnum))
    {

    }
    public SqlCharAttribute(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int size) : base(GetSqlSbType(encoding, storageTypeEnum, size))
    {

    }

    private static SqlTypeDefinition GetSqlSbType(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int? size = null) => 
        (encoding, storageTypeEnum) switch
        {
            (EncodingEnum.Ascii, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsChar(size),
            (EncodingEnum.Ascii, StorageTypeEnum.Var) => SqlTypeDefinition.AsVarChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Fixed) => SqlTypeDefinition.AsNChar(size),
            (EncodingEnum.Unicode, StorageTypeEnum.Var) => SqlTypeDefinition.AsNVarChar(size),
            _ => throw new NotImplementedException(),
        };
}
