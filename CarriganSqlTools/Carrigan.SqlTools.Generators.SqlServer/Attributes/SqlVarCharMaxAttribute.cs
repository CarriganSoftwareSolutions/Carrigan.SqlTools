using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlVarCharMaxAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlVarCharMaxAttribute"/> class.
    /// </summary>
    /// <param name="encodingEnum">The SQL character encoding option.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when the requested SQL type option is not supported.
    /// </exception>
    public SqlVarCharMaxAttribute(EncodingEnum encodingEnum) : base(GetFieldProperties(encodingEnum))
    { }

    private static FieldProperties GetFieldProperties(EncodingEnum encodingEnum) =>
        encodingEnum switch
        {
            EncodingEnum.Ascii => SqlServerTypesProvider.AsVarCharMax(),
            EncodingEnum.Unicode => SqlServerTypesProvider.AsNVarCharMax(),
            _ => throw new NotSupportedException($"Unsupported {nameof(EncodingEnum)} value '{encodingEnum}' for {nameof(SqlVarCharMaxAttribute)}."),
        };
}
