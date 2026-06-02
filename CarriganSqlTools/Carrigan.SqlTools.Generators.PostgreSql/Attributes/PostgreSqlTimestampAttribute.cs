using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>TIMESTAMP WITHOUT TIME ZONE</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlTimestampAttribute : SqlTypeAttribute
{
    public PostgreSqlTimestampAttribute() : base(PostgreSqlTypesProvider.AsTimestampWithoutTimeZone())
    { }

    public PostgreSqlTimestampAttribute(byte fractionalSecondsPrecision) : base(PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(fractionalSecondsPrecision))
    { }
}
