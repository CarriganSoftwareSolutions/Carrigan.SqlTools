using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlVarCharMaxAttribute : SqlTypeAttribute
{
    public SqlVarCharMaxAttribute(EncodingEnum encoding) :
        base
        (
            encoding switch
            {
                EncodingEnum.Ascii => SqlTypeDefinition.AsVarCharMax(),
                EncodingEnum.Unicode => SqlTypeDefinition.AsNVarCharMax(),
                _ => throw new NotSupportedException(),
            }
        )
    {
    }
}
