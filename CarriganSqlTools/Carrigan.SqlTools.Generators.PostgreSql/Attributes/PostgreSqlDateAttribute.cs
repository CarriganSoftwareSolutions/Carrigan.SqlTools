using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>DATE</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlDateAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlDateAttribute"/> class.
    /// </summary>
    public PostgreSqlDateAttribute() : base(PostgreSqlTypesProvider.AsDate(false))
    { }
}
