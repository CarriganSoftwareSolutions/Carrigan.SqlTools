using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlVarBinaryMaxAttribute : SqlTypeAttribute
{
    public SqlVarBinaryMaxAttribute() : base(SqlTypeDefinition.AsVarBinaryMax())
    {
    }
}
