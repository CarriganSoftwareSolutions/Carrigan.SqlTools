using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>DATETIMEOFFSET</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeOffsetAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeOffsetAttribute"/> class.
    /// </summary>
    public SqlDateTimeOffsetAttribute() : base(SqlServerTypesProvider.AsDateTimeOffset())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeOffsetAttribute"/> class.
    /// </summary>
    /// <param name="fractionalSecondPrecision">The fractional second precision to apply.</param>
    public SqlDateTimeOffsetAttribute(byte fractionalSecondPrecision) : base(SqlServerTypesProvider.AsDateTimeOffset(fractionalSecondPrecision))
    { }
}
