using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>TIMESTAMP WITHOUT TIME ZONE</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlTimestampAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTimestampAttribute"/> class.
    /// </summary>
    public PostgreSqlTimestampAttribute() : base(PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(false))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTimestampAttribute"/> class.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    public PostgreSqlTimestampAttribute(byte fractionalSecondsPrecision) : base(PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(fractionalSecondsPrecision, false))
    { }
}
