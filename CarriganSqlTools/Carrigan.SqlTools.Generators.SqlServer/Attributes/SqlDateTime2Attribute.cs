using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>DATETIME2</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTime2Attribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTime2Attribute"/> class.
    /// </summary>
    public SqlDateTime2Attribute() : base(SqlServerTypesProvider.AsDateTime2())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTime2Attribute"/> class.
    /// </summary>
    /// <param name="fractionalSecondPrecision">The fractional second precision to apply.</param>
    public SqlDateTime2Attribute(byte fractionalSecondPrecision) : base(SqlServerTypesProvider.AsDateTime2(fractionalSecondPrecision))
    { }
}
