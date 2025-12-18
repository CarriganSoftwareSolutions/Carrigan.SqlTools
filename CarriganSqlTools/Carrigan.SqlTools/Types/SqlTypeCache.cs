using System.Collections.ObjectModel;
using System.Data;
// WHEN ADDING NEW TYPES TO THIS SELECTION,
// be sure to add them to the following places as well:
// - Carrigan.SqlTools.Exceptions SqlTypeMismatchException
// - Carrigan.SqlTools.ReflectorCache SqlTypeNameCache constructor
// - Carrigan.SqlTools.IntegrationTests   FieldsRoundTripTests datasets
// - Carrigan.SqlTools.IntegrationTests.Models FieldsModel properties
// - Carrigan.SqlTools.IntegrationTests.Models FieldsModel CreateTableSql
// - Carrigan.SqlTools.Tests.InvocationTests.TypeTests classes
// Also consider whether the type requires updates in:
// - Carrigan.SqlTools.Attributes.SqlTypeAttribute (derived classes)
// And any corresponding locations in:
// - Carrigan.SqlTools.Analyzers

namespace Carrigan.SqlTools.Types;
/// <summary>
/// Provides a centralized and efficient mapping from CLR <see cref="Type"/> values
/// to their corresponding ADO.NET <see cref="SqlDbType"/> values as used by
/// <see cref="SqlParameter.SqlDbType"/>.
/// </summary>
/// <remarks>
/// <para>
/// This cache ensures consistent, modern SQL parameter typing across all
/// generated commands. The following mappings intentionally use modern SQL types:
/// <list type="bullet">
///   <item><description><see cref="DateTime"/> → <see cref="SqlDbType.DateTime2"/></description></item>
///   <item><description><see cref="DateOnly"/> → <see cref="SqlDbType.Date"/></description></item>
///   <item><description><see cref="TimeOnly"/> → <see cref="SqlDbType.Time"/></description></item>
///   <item><description><see cref="DateTimeOffset"/> → <see cref="SqlDbType.DateTimeOffset"/></description></item>
///   <item><description><see cref="decimal"/> → <see cref="SqlDbType.Decimal"/></description></item>
/// </list>
/// These mappings help prevent issues such as <c>SqlDateTime overflow</c>
/// when passing <see cref="DateTime.MinValue"/> or values occurring before the year 1753.
/// </para>
/// <para>
/// For numeric and textual types, the mapping aligns with SQL Server conventions, such as:
/// <see cref="int"/> → <see cref="SqlDbType.Int"/>,
/// <see cref="string"/> → <see cref="SqlDbType.NVarChar"/>,
/// <see cref="byte[]"/> → <see cref="SqlDbType.VarBinary"/>.
/// </para>
/// </remarks>
public static class SqlTypeCache
{
    /// <summary>
    /// Retrieves the <see cref="SqlDbType"/> for a runtime value.
    /// </summary>
    /// <param name="value">The value to inspect, or <c>null</c>.</param>
    /// <returns>
    /// <see cref="SqlDbType.Variant"/> if <paramref name="value"/> is <c>null</c>;
    /// otherwise the mapped <see cref="SqlDbType"/> of the value's CLR type.
    /// </returns>
    internal static SqlDbType GetSqlDbType(Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        if (underlyingType.IsEnum)
            return MapEnum(underlyingType);
        else if (_cache.TryGetValue(type, out SqlDbType sqlDbType))
            return sqlDbType;
        else
            return SqlDbType.Variant;
    }

    internal static SqlDbType GetSqlDbTypeFromValue(object? value) =>
        value is null ? SqlDbType.Variant : GetSqlDbType(value.GetType());

    /// <summary>
    /// Internal lookup table mapping CLR types to their corresponding SQL types.
    /// </summary>
    private static readonly ReadOnlyDictionary<Type, SqlDbType> _cache;

    /// <summary>
    /// Maps an enum type to the <see cref="SqlDbType"/> of its underlying type.
    /// </summary>
    /// <param name="enumType">The enum <see cref="Type"/>.</param>
    /// <returns>
    /// The mapped <see cref="SqlDbType"/> for the enum’s underlying type,
    /// or <see cref="SqlDbType.Variant"/> if no mapping exists.
    /// </returns>
    private static SqlDbType MapEnum(Type enumType)
    {
        Type underlyingType = Enum.GetUnderlyingType(enumType);
        if (_cache.TryGetValue(underlyingType, out SqlDbType sqlType)) 
            return sqlType;
        else
            return SqlDbType.Variant;
    }

    /// <summary>
    /// Initializes the internal cache of CLR-to-SQL type mappings.
    /// </summary>
    static SqlTypeCache()
    {
        // Mapping aligns with ADO.NET defaults but uses modern equivalents where possible
        IEnumerable<KeyValuePair<Type, SqlDbType>> keyValuePairs =
        [
            // Guid
            new(typeof(Guid),               SqlDbType.UniqueIdentifier),
            new(typeof(Guid?),              SqlDbType.UniqueIdentifier),

            // Text
            new(typeof(string),             SqlDbType.NVarChar),
            new(typeof(char),               SqlDbType.NChar),
            new(typeof(char?),              SqlDbType.NChar),

            // Binary
            new(typeof(byte[]),             SqlDbType.VarBinary),

            // Boolean
            new(typeof(bool),               SqlDbType.Bit),
            new(typeof(bool?),              SqlDbType.Bit),

            // Integers
            new(typeof(byte),               SqlDbType.TinyInt),
            new(typeof(byte?),              SqlDbType.TinyInt),
            new(typeof(sbyte),              SqlDbType.SmallInt),
            new(typeof(sbyte?),             SqlDbType.SmallInt),
            new(typeof(short),              SqlDbType.SmallInt),
            new(typeof(short?),             SqlDbType.SmallInt),
            new(typeof(int),                SqlDbType.Int),
            new(typeof(int?),               SqlDbType.Int),
            new(typeof(long),               SqlDbType.BigInt),
            new(typeof(long?),              SqlDbType.BigInt),

            // Decimal and floating-point
            new(typeof(float),              SqlDbType.Real),
            new(typeof(float?),             SqlDbType.Real),
            new(typeof(double),             SqlDbType.Float),
            new(typeof(double?),            SqlDbType.Float),
            new(typeof(decimal),            SqlDbType.Decimal),
            new(typeof(decimal?),           SqlDbType.Decimal),

            // Date/Time
            new(typeof(DateTime),           SqlDbType.DateTime2),
            new(typeof(DateTime?),          SqlDbType.DateTime2),
            new(typeof(DateOnly),           SqlDbType.Date),
            new(typeof(DateOnly?),          SqlDbType.Date),
            new(typeof(TimeOnly),           SqlDbType.Time),
            new(typeof(TimeOnly?),          SqlDbType.Time),
            new(typeof(DateTimeOffset),     SqlDbType.DateTimeOffset),
            new(typeof(DateTimeOffset?),    SqlDbType.DateTimeOffset),

            //Note: Is Enum is not directly handled by the cache, but rather the underlying type of the enum is handled by the cache.

            //TODO: XML:Not supported yet. Add in the future after issues mapping resolved, or remove.
            //new(typeof(System.Xml.Linq.XDocument), SqlDbType.Xml),
            //new(typeof(System.Xml.XmlDocument),    SqlDbType.Xml),
            //new(typeof(JSON???), SqlDbType.Json),

            // Fallback
            new(typeof(object),             SqlDbType.Variant)
        ];

        _cache = new ReadOnlyDictionary<Type, SqlDbType>(new Dictionary<Type, SqlDbType>(keyValuePairs));
    }

    /// <summary>
    /// Returns all CLR types that are explicitly supported by this cache.
    /// </summary>
    /// <returns>
    /// All CLR types that are explicitly supported by this cache.
    /// </returns>
    internal static IEnumerable<Type> GetAllCSharpTypes() => 
        _cache.Keys;
}
