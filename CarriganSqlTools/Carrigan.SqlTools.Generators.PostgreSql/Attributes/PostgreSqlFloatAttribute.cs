using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>FLOAT</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlFloatAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlFloatAttribute"/> class.
    /// </summary>
    public PostgreSqlFloatAttribute() : base(PostgreSqlTypesProvider.AsFloat(null, false))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlFloatAttribute"/> class.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    public PostgreSqlFloatAttribute(byte precision) : base(PostgreSqlTypesProvider.AsFloat(precision, false))
    { }
}
