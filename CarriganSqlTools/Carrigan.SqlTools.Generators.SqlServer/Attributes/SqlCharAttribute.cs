using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server character column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlCharAttribute : SqlTypeAttribute
{
    /// <summary>
    /// The default SQL Server length used for non-Unicode character fields when no size is specified.
    /// </summary>
    private const int DefaultAsciiLength = 8000;

    /// <summary>
    /// The default SQL Server length used for Unicode character fields when no size is specified.
    /// </summary>
    private const int DefaultUnicodeLength = 4000;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCharAttribute"/> class.
    /// </summary>
    /// <param name="encodingEnum">The SQL character encoding option.</param>
    /// <param name="storageTypeEnum">The SQL storage type option.</param>
    public SqlCharAttribute(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum) : base(GetFieldProperties(encodingEnum, storageTypeEnum))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCharAttribute"/> class.
    /// </summary>
    /// <param name="encodingEnum">The SQL character encoding option.</param>
    /// <param name="storageTypeEnum">The SQL storage type option.</param>
    /// <param name="size">The SQL size to apply.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when the requested SQL type option is not supported.
    /// </exception>
    public SqlCharAttribute(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum, int size) : base(GetFieldProperties(encodingEnum, storageTypeEnum, size))
    { }

    /// <summary>
    /// Maps the provided encoding and storage type options (and optional size) to a corresponding <see cref="FieldProperties"/> instance.
    /// </summary>
    /// <param name="encodingEnum">
    /// The SQL character encoding option.
    /// </param>
    /// <param name="storageTypeEnum">
    /// The SQL storage type option.
    /// </param>
    /// <param name="size">
    /// An optional SQL size to apply. If not provided, defaults will be used based on the encoding and storage type.
    /// </param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the combination of encoding and storage type options is not supported.
    /// </exception>
    private static FieldProperties GetFieldProperties(EncodingEnum encodingEnum, StorageTypeEnum storageTypeEnum, int? size = null) =>
        (encodingEnum, storageTypeEnum) switch
        {
            (EncodingEnum.Ascii, StorageTypeEnum.Fixed) => size is null ? SqlServerTypesProvider.AsChar(DefaultAsciiLength) : SqlServerTypesProvider.AsChar(size.Value),
            (EncodingEnum.Ascii, StorageTypeEnum.Var) => size is null ? SqlServerTypesProvider.AsVarCharMax() : SqlServerTypesProvider.AsVarChar(size.Value),
            (EncodingEnum.Unicode, StorageTypeEnum.Fixed) => size is null ? SqlServerTypesProvider.AsNChar(DefaultUnicodeLength) : SqlServerTypesProvider.AsNChar(size.Value),
            (EncodingEnum.Unicode, StorageTypeEnum.Var) => size is null ? SqlServerTypesProvider.AsNVarCharMax() : SqlServerTypesProvider.AsNVarChar(size.Value),
            _ => throw new NotSupportedException($"Unsupported encoding '{encodingEnum}' with storage type '{storageTypeEnum}'."),
        };
}
