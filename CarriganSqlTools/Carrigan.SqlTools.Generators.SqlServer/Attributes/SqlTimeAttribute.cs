using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>TIME</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTimeAttribute : SqlTypeAttribute
{
    public SqlTimeAttribute() : base(SqlServerTypesProvider.AsTime())
    { }

    public SqlTimeAttribute(byte fractionalSecondPrecision) : base(SqlServerTypesProvider.AsTime(fractionalSecondPrecision))
    { }
}
