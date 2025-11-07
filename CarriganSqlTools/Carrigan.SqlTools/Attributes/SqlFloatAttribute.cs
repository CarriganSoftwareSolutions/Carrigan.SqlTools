using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlFloatAttribute : SqlTypeAttribute
{
    public SqlFloatAttribute() : base(SqlTypeDefinition.AsFloat())
    {
    }
    public SqlFloatAttribute(byte precision) : base(SqlTypeDefinition.AsFloat(precision))
    {
    }
}
