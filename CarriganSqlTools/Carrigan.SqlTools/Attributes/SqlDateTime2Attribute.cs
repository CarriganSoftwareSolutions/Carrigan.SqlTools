using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTime2Attribute : SqlTypeAttribute
{
    public SqlDateTime2Attribute() : base(SqlTypeDefinition.AsDateTime2()) 
    {
    }
    public SqlDateTime2Attribute(byte fractionalSecondPrecision) : base(SqlTypeDefinition.AsDateTime2(fractionalSecondPrecision)) 
    {
    }
}
