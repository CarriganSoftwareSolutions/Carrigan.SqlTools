using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a PostgreSQL <c>CHAR</c> or <c>VARCHAR</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PostgreSqlCharAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlCharAttribute"/> class.
    /// </summary>
    /// <param name="storageTypeEnum">The SQL storage type option.</param>
    public PostgreSqlCharAttribute(StorageTypeEnum storageTypeEnum) : base(GetFieldProperties(storageTypeEnum, null))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlCharAttribute"/> class.
    /// </summary>
    /// <param name="storageTypeEnum">The SQL storage type option.</param>
    /// <param name="length">The SQL length to apply.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when the requested SQL type option is not supported.
    /// </exception>
    public PostgreSqlCharAttribute(StorageTypeEnum storageTypeEnum, int length) : base(GetFieldProperties(storageTypeEnum, length))
    { }

    private static FieldProperties GetFieldProperties(StorageTypeEnum storageTypeEnum, int? length) =>
        storageTypeEnum switch
        {
            StorageTypeEnum.Fixed => PostgreSqlTypesProvider.AsChar(length, false),
            StorageTypeEnum.Var => PostgreSqlTypesProvider.AsVarChar(length, false),
            _ => throw new NotSupportedException($"Unsupported storage type '{storageTypeEnum}'.")
        };
}
