using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>DATETIME</c> or <c>SMALLDATETIME</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeAttribute"/> class.
    /// </summary>
    /// <param name="sizeableEnum">The SQL type size option.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when the requested SQL type option is not supported.
    /// </exception>
    public SqlDateTimeAttribute(SizeableEnum sizeableEnum) : base(GetFieldProperties(sizeableEnum))
    { }

    /// <summary>
    /// Gets the <see cref="FieldProperties"/> corresponding to the specified <see cref="SizeableEnum"/> value.
    /// </summary>
    /// <param name="sizeableEnum">
    /// The <see cref="SizeableEnum"/> value indicating the desired SQL type size. Supported values are:
    /// </param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private static FieldProperties GetFieldProperties(SizeableEnum sizeableEnum) =>
        sizeableEnum switch
        {
            SizeableEnum.Regular => SqlServerTypesProvider.AsDateTime(),
            SizeableEnum.Smaller => SqlServerTypesProvider.AsSmallDateTime(),
            _ => throw new NotSupportedException($"Unsupported date/time size '{sizeableEnum}'."),
        };
}
