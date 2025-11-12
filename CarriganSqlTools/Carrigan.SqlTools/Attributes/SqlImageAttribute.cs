using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlImageAttribute : SqlTypeAttribute
{
    public SqlImageAttribute() : base (SqlTypeDefinition.AsImage())
    {

    }
}
