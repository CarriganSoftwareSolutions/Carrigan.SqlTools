using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>TEXT</c> or <c>NTEXT</c> column and overrides the default
/// SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// The SQL type is determined by the supplied <see cref="EncodingEnum"/>:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="EncodingEnum.Ascii"/> produces <c>TEXT</c>.</description></item>
/// <item><description><see cref="EncodingEnum.Unicode"/> produces <c>NTEXT</c>.</description></item>
/// </list>
/// <para>
/// SQL Server <c>TEXT</c> and <c>NTEXT</c> are deprecated and are provided only to support legacy database schemas.
/// For new development, prefer <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c>.
/// </para>
/// <para>
/// Suggested CLR type: <see cref="string"/>.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity Framework
/// or database schema.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTextAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTextAttribute"/> class and configures the associated property
    /// to represent either a <c>TEXT</c> or <c>NTEXT</c> column depending on <paramref name="encodingEnum"/>.
    /// </summary>
    /// <param name="encodingEnum">
    /// Determines whether the column is generated as <c>TEXT</c> (<see cref="EncodingEnum.Ascii"/>) or <c>NTEXT</c>
    /// (<see cref="EncodingEnum.Unicode"/>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="EncodingEnum"/> value is supplied. This typically indicates that the
    /// enumeration was extended without updating the <see cref="SqlTextAttribute"/> logic.
    /// </exception>
    public SqlTextAttribute(EncodingEnum encodingEnum)
        : base(GetSqlTypeDefinition(encodingEnum))
    {
    }

    private static SqlTypeDefinition GetSqlTypeDefinition(EncodingEnum encodingEnum) =>
        encodingEnum switch
        {
            EncodingEnum.Ascii => SqlTypeDefinition.AsText(),
            EncodingEnum.Unicode => SqlTypeDefinition.AsNText(),
            _ => throw new NotSupportedException(
                $"Unsupported {nameof(EncodingEnum)} value '{encodingEnum}' for {nameof(SqlTextAttribute)}."),
        };
}
