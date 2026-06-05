using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>VARBINARY(MAX)</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlVarBinaryMaxAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlVarBinaryMaxAttribute"/> class.
    /// </summary>
    public SqlVarBinaryMaxAttribute() : base(SqlServerTypesProvider.AsVarBinaryMax())
    { }
}
