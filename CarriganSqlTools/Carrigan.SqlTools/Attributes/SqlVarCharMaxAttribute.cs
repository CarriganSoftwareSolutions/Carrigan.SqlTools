using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// The SQL type is determined by the supplied <see cref="EncodingEnum"/>:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="EncodingEnum.Ascii"/> produces <c>VARCHAR(MAX)</c>.</description></item>
/// <item><description><see cref="EncodingEnum.Unicode"/> produces <c>NVARCHAR(MAX)</c>.</description></item>
/// </list>
/// <para>
/// Suggested CLR type: <see cref="string"/>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlVarCharMaxAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlVarCharMaxAttribute"/> class and configures
    /// the associated property to represent a <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c> column,
    /// depending on the specified <see cref="EncodingEnum"/>.
    /// </summary>
    /// <param name="encodingEnum">
    /// Determines whether the column is generated as <c>VARCHAR(MAX)</c> (<see cref="EncodingEnum.Ascii"/>) or
    /// <c>NVARCHAR(MAX)</c> (<see cref="EncodingEnum.Unicode"/>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="EncodingEnum"/> value is supplied. This typically indicates that the
    /// enumeration was extended without updating the <see cref="SqlVarCharMaxAttribute"/> logic.
    /// </exception>
    public SqlVarCharMaxAttribute(EncodingEnum encodingEnum) : base(GetSqlTypeDefinition(encodingEnum))
    {
    }

    private static SqlTypeDefinition GetSqlTypeDefinition(EncodingEnum encodingEnum) =>
        encodingEnum switch
        {
            EncodingEnum.Ascii => SqlTypeDefinition.AsVarCharMax(),
            EncodingEnum.Unicode => SqlTypeDefinition.AsNVarCharMax(),
            _ => throw new NotSupportedException(
                $"Unsupported {nameof(EncodingEnum)} value '{encodingEnum}' for {nameof(SqlVarCharMaxAttribute)}."),
        };
}
