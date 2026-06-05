using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>TIME</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTimeAttribute"/> class.
    /// </summary>
    public SqlTimeAttribute() : base(SqlServerTypesProvider.AsTime())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTimeAttribute"/> class.
    /// </summary>
    /// <param name="fractionalSecondPrecision">The fractional second precision to apply.</param>
    public SqlTimeAttribute(byte fractionalSecondPrecision) : base(SqlServerTypesProvider.AsTime(fractionalSecondPrecision))
    { }
}
