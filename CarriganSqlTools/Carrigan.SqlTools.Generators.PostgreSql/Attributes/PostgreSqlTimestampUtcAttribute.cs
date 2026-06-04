using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>TIMESTAMP WITH TIME ZONE</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlTimestampUtcAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTimestampUtcAttribute"/> class.
    /// </summary>
    public PostgreSqlTimestampUtcAttribute() : base(PostgreSqlTypesProvider.AsTimestampWithTimeZone(false))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTimestampUtcAttribute"/> class.
    /// </summary>
    /// <param name="fractionalSecondsPrecision">The fractional seconds precision to apply.</param>
    public PostgreSqlTimestampUtcAttribute(byte fractionalSecondsPrecision) : base(PostgreSqlTypesProvider.AsTimestampWithTimeZone(fractionalSecondsPrecision, false))
    { }
}
