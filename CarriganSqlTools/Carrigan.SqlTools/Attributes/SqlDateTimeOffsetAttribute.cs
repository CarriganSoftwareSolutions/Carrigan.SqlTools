using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlDateTimeOffsetAttribute : SqlTypeAttribute
{
    public SqlDateTimeOffsetAttribute() : base(SqlTypeDefinition.AsDateTimeOffset())
    {
    }
    public SqlDateTimeOffsetAttribute(byte precision) : base(SqlTypeDefinition.AsDateTimeOffset(precision))
    {
    }
}
