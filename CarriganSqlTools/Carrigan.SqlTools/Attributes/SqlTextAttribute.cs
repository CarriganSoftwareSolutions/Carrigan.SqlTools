using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTextAttribute : SqlTypeAttribute
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
