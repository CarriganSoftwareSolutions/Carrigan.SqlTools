using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlTimeAttribute : SqlTypeAttribute
{
    public SqlTimeAttribute() : base(SqlTypeDefinition.AsTime())
    {
    }
    public SqlTimeAttribute(byte precision) : base(SqlTypeDefinition.AsTime(precision))
    {
    }
}
