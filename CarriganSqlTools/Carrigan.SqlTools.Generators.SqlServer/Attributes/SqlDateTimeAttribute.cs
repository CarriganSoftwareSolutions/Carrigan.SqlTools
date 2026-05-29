using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>DATETIME</c> or <c>SMALLDATETIME</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeAttribute : SqlTypeAttribute
{
    public SqlDateTimeAttribute(SizeableEnum sizeableEnum) : base(GetFieldProperties(sizeableEnum))
    { }

    private static FieldProperties GetFieldProperties(SizeableEnum sizeableEnum) =>
        sizeableEnum switch
        {
            SizeableEnum.Regular => SqlServerTypesProvider.AsDateTime(),
            SizeableEnum.Smaller => SqlServerTypesProvider.AsSmallDateTime(),
            _ => throw new NotSupportedException($"Unsupported date/time size '{sizeableEnum}'."),
        };
}
