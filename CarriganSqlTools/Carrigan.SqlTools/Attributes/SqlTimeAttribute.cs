using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTimeAttribute : SqlTypeAttribute
{
    public SqlTimeAttribute() : base(SqlTypeDefinition.AsTime())
    {
    }
    public SqlTimeAttribute(byte precision) : base(SqlTypeDefinition.AsTime(precision))
    {
    }
}
