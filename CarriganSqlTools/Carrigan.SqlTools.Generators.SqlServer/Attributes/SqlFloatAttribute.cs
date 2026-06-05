using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>FLOAT</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlFloatAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFloatAttribute"/> class.
    /// </summary>
    public SqlFloatAttribute() : base(SqlServerTypesProvider.AsFloat())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFloatAttribute"/> class.
    /// </summary>
    /// <param name="precision">The SQL precision to apply.</param>
    public SqlFloatAttribute(byte precision) : base(SqlServerTypesProvider.AsFloat(precision))
    { }
}
