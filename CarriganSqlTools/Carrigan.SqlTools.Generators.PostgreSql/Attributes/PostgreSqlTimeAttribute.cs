using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>TIME WITHOUT TIME ZONE</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlTimeAttribute : SqlTypeAttribute
{
    public PostgreSqlTimeAttribute() : base(PostgreSqlTypesProvider.AsTimeWithoutTimeZone())
    { }

    public PostgreSqlTimeAttribute(byte fractionalSecondsPrecision) : base(PostgreSqlTypesProvider.AsTimeWithoutTimeZone(fractionalSecondsPrecision))
    { }
}
