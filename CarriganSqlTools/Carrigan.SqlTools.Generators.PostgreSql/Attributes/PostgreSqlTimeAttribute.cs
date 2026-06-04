using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>TIME WITHOUT TIME ZONE</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTimeAttribute"/> class.
    /// </summary>
    public PostgreSqlTimeAttribute() : base(PostgreSqlTypesProvider.AsTimeWithoutTimeZone(false))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTimeAttribute"/> class.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    public PostgreSqlTimeAttribute(byte fractionalSecondsPrecision) : base(PostgreSqlTypesProvider.AsTimeWithoutTimeZone(fractionalSecondsPrecision, false))
    { }
}
