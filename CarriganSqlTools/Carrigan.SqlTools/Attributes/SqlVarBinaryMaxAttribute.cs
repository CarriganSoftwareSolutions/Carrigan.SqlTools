using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlVarBinaryMaxAttribute : SqlTypeAttribute
{
    public SqlVarBinaryMaxAttribute() : base(SqlTypeDefinition.AsVarBinaryMax())
    {
    }
}
