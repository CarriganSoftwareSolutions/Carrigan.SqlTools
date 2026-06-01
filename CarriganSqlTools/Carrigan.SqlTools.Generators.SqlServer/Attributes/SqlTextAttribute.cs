using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a legacy SQL Server <c>TEXT</c> or <c>NTEXT</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTextAttribute : SqlTypeAttribute
{
    public SqlTextAttribute(EncodingEnum encodingEnum) : base(GetFieldProperties(encodingEnum))
    { }

#pragma warning disable CS0618 // TEXT and NTEXT are intentionally exposed for legacy database schemas.
    private static FieldProperties GetFieldProperties(EncodingEnum encodingEnum) =>
        encodingEnum switch
        {
            EncodingEnum.Ascii => SqlServerTypesProvider.AsText(),
            EncodingEnum.Unicode => SqlServerTypesProvider.AsNText(),
            _ => throw new NotSupportedException($"Unsupported {nameof(EncodingEnum)} value '{encodingEnum}' for {nameof(SqlTextAttribute)}."),
        };
#pragma warning restore CS0618
}
