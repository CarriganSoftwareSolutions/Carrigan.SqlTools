using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>DECIMAL</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDecimalAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class.
    /// </summary>
    public SqlDecimalAttribute() : base(SqlServerTypesProvider.AsDecimal())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    public SqlDecimalAttribute(byte precision) : base(SqlServerTypesProvider.AsDecimal(precision))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    /// <param name="scale">The SQL scale to apply.</param>
    public SqlDecimalAttribute(byte precision, byte scale) : base(SqlServerTypesProvider.AsDecimal(precision, scale))
    { }
}
