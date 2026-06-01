namespace Carrigan.SqlTools.Dialects;

internal static class DialectBaseStatics
{
    //TODO: move dialect provider to this project, have it use this static
    /// <summary>
    /// Returns a set of CLR types that are supported by the base SqlToolsReflectorCache for SQL parameter mapping and type inference.
    /// </summary>
    /// <returns>
    /// A <see cref="HashSet{Type}"/> containing the CLR types supported by the base SqlToolsReflectorCache, including but not limited to:
    /// <list type="bullet">
    ///     <item><description><see cref="Guid"/> and nullable <see cref="Guid"/> values, including array variants.</description></item>
    ///     <item><description>Text values, including <see cref="string"/>, <see cref="char"/>, nullable <see cref="char"/>, and supported array variants.</description></item>
    ///     <item><description>Binary values, including <see cref="byte"/> arrays mapped as PostgreSQL <c>bytea</c>.</description></item>
    ///     <item><description><see cref="bool"/> and nullable <see cref="bool"/> values, including array variants.</description></item>
    ///     <item><description>Integer values, including <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, and their nullable and array variants.</description></item>
    ///     <item><description>Numeric values, including <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>, and their nullable and array variants.</description></item>
    ///     <item><description>Date and time values, including <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="TimeSpan"/>, <see cref="DateTimeOffset"/>, and their nullable and array variants.</description></item>
    ///     <item><description>XML values, including <see cref="System.Xml.Linq.XDocument"/>, <see cref="System.Xml.XmlDocument"/>, and supported array variants.</description></item>
    ///     <item><description><see cref="object"/> as a fallback type for unmapped values.</description></item>
    /// </list>
    /// </returns>
    internal static HashSet<Type> SupportedTypes =>
    [
        // Guid
        typeof(Guid),
        typeof(Guid?),
        typeof(Guid[]),
        typeof(Guid?[]),

        // Text
        typeof(string),
        typeof(char),
        typeof(char?),
        typeof(string[]),
        typeof(char?[]),

        // Binary
        // byte[] is bytea, not a PostgreSQL array type.
        typeof(byte[]),
        typeof(byte[][]),

        // Boolean
        typeof(bool),
        typeof(bool?),
        typeof(bool[]),
        typeof(bool?[]),

        // Integers
        typeof(byte),
        typeof(byte?),
        typeof(sbyte),
        typeof(sbyte?),
        typeof(short),
        typeof(short?),
        typeof(int),
        typeof(int?),
        typeof(long),
        typeof(long?),

        typeof(byte?[]),
        typeof(sbyte[]),
        typeof(sbyte?[]),
        typeof(short[]),
        typeof(short?[]),
        typeof(int[]),
        typeof(int?[]),
        typeof(long[]),
        typeof(long?[]),

        // Decimal and floating-point
        typeof(float),
        typeof(float?),
        typeof(double),
        typeof(double?),
        typeof(decimal),
        typeof(decimal?),

        typeof(float[]),
        typeof(float?[]),
        typeof(double[]),
        typeof(double?[]),
        typeof(decimal[]),
        typeof(decimal?[]),

        // Date/Time
        typeof(DateTime),
        typeof(DateTime?),
        typeof(DateOnly),
        typeof(DateOnly?),
        typeof(TimeOnly),
        typeof(TimeOnly?),
        typeof(TimeSpan),
        typeof(TimeSpan?),
        typeof(DateTimeOffset),
        typeof(DateTimeOffset?),

        typeof(DateTime[]),
        typeof(DateTime?[]),
        typeof(DateOnly[]),
        typeof(DateOnly?[]),
        typeof(TimeOnly[]),
        typeof(TimeOnly?[]),
        typeof(TimeSpan[]),
        typeof(TimeSpan?[]),
        typeof(DateTimeOffset[]),
        typeof(DateTimeOffset?[]),

        // XML
        // PostgreSQL/Npgsql treats XML as string-compatible.
        typeof(System.Xml.Linq.XDocument),
        typeof(System.Xml.XmlDocument),
        typeof(System.Xml.Linq.XDocument[]),
        typeof(System.Xml.XmlDocument[]),

        // Fallback
        typeof(object)
    ];

}
