using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents an <c>IMAGE</c> SQL column and overrides the default SQL type mapping
/// for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// SQL Server <c>IMAGE</c> is deprecated. This attribute is provided only to support legacy database schemas.
/// For new development, prefer <c>VARBINARY(MAX)</c> via <see cref="SqlVarBinaryMaxAttribute"/>.
/// </para>
/// <para>
/// Suggested CLR type: <see cref="byte"/>[].
/// If the decorated property type is incompatible with <c>IMAGE</c>, the SQL generator's validation checks can throw
/// <c>SqlTypeMismatchException</c> (or an <c>AggregateException</c> containing it) when initializing
/// <c>SqlGenerator&lt;T&gt;</c>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlImageAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlImageAttribute"/> class and configures
    /// the associated property to represent an <c>IMAGE</c> column.
    /// </summary>
    public SqlImageAttribute() : base(SqlTypeDefinition.AsImage())
    {
    }
}
