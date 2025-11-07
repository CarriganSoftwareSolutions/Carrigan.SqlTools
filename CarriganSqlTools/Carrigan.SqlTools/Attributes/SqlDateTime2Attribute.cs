using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlDateTime2Attribute : SqlTypeAttribute
{
    public SqlDateTime2Attribute() : base(SqlTypeDefinition.AsDateTime2()) 
    {
    }
    public SqlDateTime2Attribute(byte precision) : base(SqlTypeDefinition.AsDateTime2(precision)) 
    {
    }
}
