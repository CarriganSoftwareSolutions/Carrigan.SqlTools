using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a legacy SQL Server <c>IMAGE</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlImageAttribute : SqlTypeAttribute
{
#pragma warning disable CS0618 // IMAGE is intentionally exposed for legacy database schemas.
    public SqlImageAttribute() : base(SqlServerTypesProvider.AsImage())
    { }
#pragma warning restore CS0618
}
