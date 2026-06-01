using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>DATETIMEOFFSET</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeOffsetAttribute : SqlTypeAttribute
{
    public SqlDateTimeOffsetAttribute() : base(SqlServerTypesProvider.AsDateTimeOffset())
    { }

    public SqlDateTimeOffsetAttribute(byte fractionalSecondPrecision) : base(SqlServerTypesProvider.AsDateTimeOffset(fractionalSecondPrecision))
    { }
}
