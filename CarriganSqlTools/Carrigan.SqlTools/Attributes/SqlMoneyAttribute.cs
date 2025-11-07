using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlMoneyAttribute : SqlTypeAttribute
{
    public SqlMoneyAttribute(SizeableEnum moneySize) :
        base
        (
            (moneySize) switch
            {
                (SizeableEnum.Regular) => SqlTypeDefinition.AsMoney(),
                (SizeableEnum.Smaller) => SqlTypeDefinition.AsSmallMoney(),
                _ => throw new NotImplementedException(),
            }
        )
    {
    }
}
