using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>NUMERIC</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlNumericAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlNumericAttribute"/> class.
    /// </summary>
    public PostgreSqlNumericAttribute() : base(PostgreSqlTypesProvider.AsNumeric(false))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlNumericAttribute"/> class.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    public PostgreSqlNumericAttribute(byte precision) : base(PostgreSqlTypesProvider.AsNumeric(precision, false))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlNumericAttribute"/> class.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    /// <param name="scale">The SQL scale to apply.</param>
    public PostgreSqlNumericAttribute(byte precision, byte scale) : base(PostgreSqlTypesProvider.AsNumeric(precision, scale, false))
    { }
}
