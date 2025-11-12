using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlBinaryAttribute : SqlTypeAttribute
{
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum) : base (GetSqlSbType(storageTypeEnum))
    {

    }
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum, int size) : base(GetSqlSbType(storageTypeEnum, size))
    {

    }

    private static SqlTypeDefinition GetSqlSbType(StorageTypeEnum storageTypeEnum, int? size = null) => 
        (storageTypeEnum) switch
        {
            (StorageTypeEnum.Fixed) => SqlTypeDefinition.AsBinary(size),
            (StorageTypeEnum.Var) => SqlTypeDefinition.AsVarBinary(size),
            _ => throw new NotImplementedException(),
        };
}
