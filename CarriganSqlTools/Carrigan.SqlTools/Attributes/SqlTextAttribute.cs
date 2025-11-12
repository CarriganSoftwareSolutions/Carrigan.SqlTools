using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property)]
public class SqlTextAttribute : SqlTypeAttribute
{
    public SqlTextAttribute(EncodingEnum encoding) : base (GetSqlSbType(encoding))
    {

    }

    private static SqlTypeDefinition GetSqlSbType(EncodingEnum encoding) => 
        (encoding) switch
        {
            (EncodingEnum.Ascii) => SqlTypeDefinition.AsText(),
            (EncodingEnum.Unicode) => SqlTypeDefinition.AsNText(),
            _ => throw new NotImplementedException(),
        };
}
