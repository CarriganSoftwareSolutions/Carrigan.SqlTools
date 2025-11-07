using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlDecimalAttribute : SqlTypeAttribute
{
    public SqlDecimalAttribute() : base(SqlTypeDefinition.AsDecimal())
    {
    }
    public SqlDecimalAttribute(byte precision) : base(SqlTypeDefinition.AsDecimal(precision))
    {
    }
    public SqlDecimalAttribute(byte precision, byte scale) : base(SqlTypeDefinition.AsDecimal(precision, scale))
    {
    }
}
