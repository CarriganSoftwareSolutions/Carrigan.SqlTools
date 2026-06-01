namespace Carrigan.SqlTools.Dialects;

internal static class DialectStatics
{
    /// <summary>
    /// Returns a set of CLR types that are supported by the SQL Server dialect for parameter values and type mapping.
    /// </summary>
    /// <returns>
    /// A <see cref="HashSet{Type}"/> containing the CLR types supported by the SQLServer dialect, including but not limited to:
    /// <list type="bullet">
    ///     <item><description><see cref="Guid"/> values, including nullable values.</description></item>
    ///     <item><description>Text values, including <see cref="string"/>, <see cref="char"/>, and nullable <see cref="char"/> values.</description></item>
    ///     <item><description>Binary values, including <see cref="byte"/> arrays.</description></item>
    ///     <item><description><see cref="bool"/> values, including nullable values.</description></item>
    ///     <item><description>Integer values, including <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, and their nullable variants.</description></item>
    ///     <item><description>Numeric values, including <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>, and their nullable variants.</description></item>
    ///     <item><description>Date and time values, including <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="DateTimeOffset"/>, and their nullable variants.</description></item>
    ///     <item><description>XML values, including <see cref="System.Xml.Linq.XDocument"/> and <see cref="System.Xml.XmlDocument"/>.</description></item>
    ///     <item><description><see cref="object"/> as a fallback type for unmapped values.</description></item>
    /// </list>
    /// </returns>
    internal static HashSet<Type> SupportedTypes =>
    [
        // Guid
        typeof(Guid),
        typeof(Guid?),

        // Text
        typeof(string),
        typeof(char),
        typeof(char?),

        // Binary
        typeof(byte[]),

        // Boolean
        typeof(bool),
        typeof(bool?),

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

        // Decimal and floating-point
        typeof(float),
        typeof(float?),
        typeof(double),
        typeof(double?),
        typeof(decimal),
        typeof(decimal?),

        // Date/Time
        typeof(DateTime),
        typeof(DateTime?),
        typeof(DateOnly),
        typeof(DateOnly?),
        typeof(TimeOnly),
        typeof(TimeOnly?),
        typeof(DateTimeOffset),
        typeof(DateTimeOffset?),

        // XML
        typeof(System.Xml.Linq.XDocument),
        typeof(System.Xml.XmlDocument),

        // Fallback
        typeof(object)
    ];

}
