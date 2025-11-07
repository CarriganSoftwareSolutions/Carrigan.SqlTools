using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlDateTimeAttribute : SqlTypeAttribute
{
    public SqlDateTimeAttribute(SizeableEnum dateTimeSize) :
        base
        (
            (dateTimeSize) switch
            {
                (SizeableEnum.Regular) => SqlTypeDefinition.AsDateTime(),
                (SizeableEnum.Smaller) => SqlTypeDefinition.AsSmallDateTime(),
                _ => throw new NotImplementedException(),
            }
        )
    {
    }
}
