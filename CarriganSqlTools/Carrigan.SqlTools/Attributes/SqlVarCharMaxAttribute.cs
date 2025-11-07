using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlVarCharMaxAttribute : SqlTypeAttribute
{
    public SqlVarCharMaxAttribute(EncodingEnum encoding) :
        base
        (
            (encoding) switch
            {
                (EncodingEnum.Ascii) => SqlTypeDefinition.AsVarCharMax(),
                (EncodingEnum.Unicode) => SqlTypeDefinition.AsNVarCharMax(),
                _ => throw new NotImplementedException(),
            }
        )
    {
    }
}
