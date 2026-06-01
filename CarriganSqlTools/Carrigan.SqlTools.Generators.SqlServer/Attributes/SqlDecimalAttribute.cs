using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>DECIMAL</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDecimalAttribute : SqlTypeAttribute
{
    public SqlDecimalAttribute() : base(SqlServerTypesProvider.AsDecimal())
    { }

    public SqlDecimalAttribute(byte precision) : base(SqlServerTypesProvider.AsDecimal(precision))
    { }

    public SqlDecimalAttribute(byte precision, byte scale) : base(SqlServerTypesProvider.AsDecimal(precision, scale))
    { }
}
