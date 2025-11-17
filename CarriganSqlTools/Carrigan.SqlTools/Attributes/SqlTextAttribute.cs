using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a legacy <c>TEXT</c> or <c>NTEXT</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a SQL Server
/// <c>TEXT</c> or <c>NTEXT</c> column on a table model.
///
/// <para>
/// The SQL type is determined by the supplied <see cref="EncodingEnum"/>:
/// <list type="bullet">
/// <item><description><see cref="EncodingEnum.Ascii"/> → <c>TEXT</c></description></item>
/// <item><description><see cref="EncodingEnum.Unicode"/> → <c>NTEXT</c></description></item>
/// </list>
/// </para>
///
/// <para>
/// SQL Server <c>TEXT</c> and <c>NTEXT</c> types are deprecated and preserved only for
/// legacy databases. For new designs, use <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c>
/// instead. This attribute exists to accurately model and generate SQL for legacy schemas.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTextAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTextAttribute"/> class and configures
    /// the associated property to represent a legacy <c>TEXT</c> or <c>NTEXT</c> column,
    /// depending on the specified <see cref="EncodingEnum"/>.
    /// </summary>
    /// <param name="encoding">
    /// Determines whether the column is generated as <c>TEXT</c> 
    /// (<see cref="EncodingEnum.Ascii"/>) or <c>NTEXT</c> 
    /// (<see cref="EncodingEnum.Unicode"/>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="EncodingEnum"/> value is supplied.
    /// This typically indicates that the enumeration was extended without updating the
    /// <see cref="SqlTextAttribute"/> logic.
    /// </exception>
    public SqlTextAttribute(EncodingEnum encoding) : base (GetSqlSbType(encoding))
    {
    }

    private static SqlTypeDefinition GetSqlSbType(EncodingEnum encoding) => 
        encoding switch
        {
            EncodingEnum.Ascii => SqlTypeDefinition.AsText(),
            EncodingEnum.Unicode => SqlTypeDefinition.AsNText(),
            _ => throw new NotSupportedException($"Unsupported EncodingEnum value '{encoding}' for SqlTextAttribute. " +
                "This usually indicates that the enumeration was extended without updating this attribute."),
        };
}
