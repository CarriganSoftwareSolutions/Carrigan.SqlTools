using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>FLOAT</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlFloatAttribute : SqlTypeAttribute
{
    public SqlFloatAttribute() : base(SqlServerTypesProvider.AsFloat())
    { }

    public SqlFloatAttribute(byte precision) : base(SqlServerTypesProvider.AsFloat(precision))
    { }
}
