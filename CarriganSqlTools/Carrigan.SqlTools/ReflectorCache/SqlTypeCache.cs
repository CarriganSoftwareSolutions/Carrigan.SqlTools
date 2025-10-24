using System.Collections.ObjectModel;
using System.Data;

//WHEN ADDING NEW TYPES TO THIS SELECTION,
//be sure to add them to add types to the following places as well:
// - Carrigan.SqlTools.ReflectorCache SqlTypeNameCache's SqlTypeNameCache Constructor
// - Carrigan.SqlTools.IntegrationTests FieldsRoundTripTests datasets
// - Carrigan.SqlTools.IntegrationTests.Models FieldsModel's properties.
// - Carrigan.SqlTools.IntegrationTests.Models FieldsModel's CreateTableSql property.
// - Carrigan.SqlTools.Tests.InvocationTests.TypeTests Classes

namespace Carrigan.SqlTools.ReflectorCache;
//TODO: Proof read, unit test
/// <summary>
/// Provides a fast, centralized mapping from CLR <see cref="Type"/> to the corresponding
/// ADO.NET <see cref="SqlDbType"/> used by <see cref="SqlParameter.SqlDbType"/>.
/// </summary>
/// <remarks>
/// <para>
/// This cache ensures consistent, modern parameter typing across all generated commands:
/// <list type="bullet">
///   <item><description><see cref="DateTime"/> → <see cref="SqlDbType.DateTime2"/></description></item>
///   <item><description><see cref="DateOnly"/> → <see cref="SqlDbType.Date"/></description></item>
///   <item><description><see cref="TimeOnly"/> → <see cref="SqlDbType.Time"/></description></item>
///   <item><description><see cref="DateTimeOffset"/> → <see cref="SqlDbType.DateTimeOffset"/></description></item>
///   <item><description><see cref="decimal"/> → <see cref="SqlDbType.Decimal"/></description></item>
/// </list>
/// These modern mappings avoid legacy truncation and range issues such as <c>SqlDateTime overflow</c>
/// when passing <see cref="DateTime.MinValue"/> or any other DateTime value before the 1753.
/// </para>
/// <para>
/// For numeric and textual types, the mapping follows SQL Server conventions:
/// <see cref="int"/> → <see cref="SqlDbType.Int"/>,
/// <see cref="string"/> → <see cref="SqlDbType.NVarChar"/>,
/// <see cref="byte[]"/> → <see cref="SqlDbType.VarBinary"/>.
/// </para>
/// </remarks>
public static class SqlTypeCache
{
    /// <summary>
    /// Tries to retrieve the <see cref="SqlDbType"/> for the specified CLR type.
    /// </summary>
    public static bool TryGetSqlDbType(Type type, out SqlDbType sqlDbType) =>
        _cache.TryGetValue(type, out sqlDbType);

    /// <summary>
    /// Retrieves the <see cref="SqlDbType"/> for the specified CLR type.
    /// Throws <see cref="NotSupportedException"/> if no mapping exists.
    /// </summary>
    public static SqlDbType GetSqlDbType(Type type) =>
        _cache.TryGetValue(type, out SqlDbType t)
            ? t
            : throw new NotSupportedException($"No SqlDbType mapping registered for CLR type '{type.FullName}'.");

    public static SqlDbType GetSqlDbTypeFromValue(object? value) =>
        value is null ? SqlDbType.Variant : SqlTypeCache.GetSqlDbType(value.GetType());

    private static readonly ReadOnlyDictionary<Type, SqlDbType> _cache;

    static SqlTypeCache()
    {
        // Mapping aligns with ADO.NET defaults but uses modern equivalents where possible
        IEnumerable<KeyValuePair<Type, SqlDbType>> keyValuePairs =
        [
            // Guid
            new(typeof(Guid),           SqlDbType.UniqueIdentifier),
            new(typeof(Guid?),          SqlDbType.UniqueIdentifier),

            // Text
            new(typeof(string),         SqlDbType.NVarChar),
            new(typeof(char),           SqlDbType.NChar),
            new(typeof(char?),          SqlDbType.NChar),

            //Binary
            new(typeof(byte[]),         SqlDbType.VarBinary),

            // Boolean
            new(typeof(bool),           SqlDbType.Bit),
            new(typeof(bool?),          SqlDbType.Bit),

            // Integers
            new(typeof(byte),           SqlDbType.TinyInt),
            new(typeof(byte?),          SqlDbType.TinyInt),
            new(typeof(sbyte),          SqlDbType.SmallInt),
            new(typeof(sbyte?),         SqlDbType.SmallInt),
            new(typeof(short),          SqlDbType.SmallInt),
            new(typeof(short?),         SqlDbType.SmallInt),
            new(typeof(int),            SqlDbType.Int),
            new(typeof(int?),           SqlDbType.Int),
            new(typeof(long),           SqlDbType.BigInt),
            new(typeof(long?),          SqlDbType.BigInt),

            //TODO: Unsigned integers. Not supported yet. Add in the future after issues mapping resolved, or remove.
            //new(typeof(ushort),         SqlDbType.Int),
            //new(typeof(ushort?),        SqlDbType.Int),
            //new(typeof(uint),           SqlDbType.BigInt),
            //new(typeof(uint?),          SqlDbType.BigInt),
            //new(typeof(ulong),          SqlDbType.Decimal),   
            //new(typeof(ulong?),         SqlDbType.Decimal),

            // Decimal and floating-point
            new(typeof(decimal),        SqlDbType.Decimal),
            new(typeof(decimal?),       SqlDbType.Decimal),
            new(typeof(float),          SqlDbType.Real),
            new(typeof(float?),         SqlDbType.Real),
            new(typeof(double),         SqlDbType.Float),
            new(typeof(double?),        SqlDbType.Float),

            // Date/Time
            new(typeof(DateTime),       SqlDbType.DateTime2),
            new(typeof(DateTime?),      SqlDbType.DateTime2),
            new(typeof(DateOnly),       SqlDbType.Date),
            new(typeof(DateOnly?),      SqlDbType.Date),
            new(typeof(TimeOnly),       SqlDbType.Time),
            new(typeof(TimeOnly?),      SqlDbType.Time),
            new(typeof(DateTimeOffset), SqlDbType.DateTimeOffset),
            new(typeof(DateTimeOffset?),SqlDbType.DateTimeOffset),

            //TODO: TimeSpan: Not supported yet. Add in the future after issues mapping resolved, or remove.
            //property.PropertyType == typeof(TimeSpan) ||                    // SQL BIGINT
            //property.PropertyType == typeof(TimeSpan?) ||                   // SQL BIGINT (nullable)

            //TODO: XML:Not supported yet. Add in the future after issues mapping resolved, or remove.
            new(typeof(System.Xml.Linq.XDocument), SqlDbType.Xml),
            new(typeof(System.Xml.XmlDocument),    SqlDbType.Xml),

            // Fallback
            new(typeof(object),         SqlDbType.Variant)
        ];

        _cache = new ReadOnlyDictionary<Type, SqlDbType>(new Dictionary<Type, SqlDbType>(keyValuePairs));
    }

    internal static IEnumerable<Type> GetAll() => 
        _cache.Keys;
}
