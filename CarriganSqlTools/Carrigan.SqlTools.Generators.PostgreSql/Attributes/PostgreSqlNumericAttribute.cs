using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>NUMERIC</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlNumericAttribute : SqlTypeAttribute
{
    public PostgreSqlNumericAttribute() : base(PostgreSqlTypesProvider.AsNumeric())
    { }

    public PostgreSqlNumericAttribute(byte precision) : base(PostgreSqlTypesProvider.AsNumeric(precision))
    { }

    public PostgreSqlNumericAttribute(byte precision, byte scale) : base(PostgreSqlTypesProvider.AsNumeric(precision, scale))
    { }
}
