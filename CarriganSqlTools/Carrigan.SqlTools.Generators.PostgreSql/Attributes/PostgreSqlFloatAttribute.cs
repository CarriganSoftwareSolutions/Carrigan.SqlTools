using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>FLOAT</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlFloatAttribute : SqlTypeAttribute
{
    public PostgreSqlFloatAttribute() : base(PostgreSqlTypesProvider.AsFloat(null))
    { }
    public PostgreSqlFloatAttribute(byte precision) : base(PostgreSqlTypesProvider.AsFloat(precision))
    { }
}
