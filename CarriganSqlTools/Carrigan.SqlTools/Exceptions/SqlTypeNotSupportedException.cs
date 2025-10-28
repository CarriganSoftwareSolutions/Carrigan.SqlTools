using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Reflection;

//TODO: proof read documentation

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more SQL types are not supported by the SQL generator.
/// </summary>
/// <remarks>
/// This exception typically occurs when using the SqlTypeAttribute,
/// or when providing a type to a Parameter as a predicate.
/// </remarks>
public class SqlTypeNotSupportedException : Exception
{
    public static readonly IEnumerable<SqlDbType> TypesNotSupported =
        [
            SqlDbType.Timestamp,
            SqlDbType.Structured,
            SqlDbType.Variant,
            SqlDbType.Udt,
            SqlDbType.Xml
        ];

    /// <summary>
    /// Thrown when one or more SQL types are not supported by the SQL generator.
    /// </summary>
    /// <remarks>
    /// This exception typically occurs when using the SqlTypeAttribute,
    /// or when providing a type to a Parameter as a predicate.
    /// </remarks>
    /// <param name="sqlTypes">
    /// Tuples pairing the unsupported <see cref="SqlDbType"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    internal SqlTypeNotSupportedException(params IEnumerable<Tuple<PropertyInfo, SqlDbType>> sqlTypes) :
        base(CreateMessage(sqlTypes))
    {
    }

    /// <summary>
    /// Builds an error message listing unsupported SQL types and the properties they are associated with.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the unsupported <see cref="SqlDbType"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, SqlDbType>> sqlTypes) =>
        $"The following SQL types are not supported by the SQL Builder: "
            + sqlTypes
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + " Columns are associated with the following properties: "
            + sqlTypes
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();


    /// <summary>
    /// Thrown when one or more SQL types are not supported by the SQL generator.
    /// </summary>
    /// <remarks>
    /// This exception typically occurs when using the SqlTypeAttribute,
    /// or when providing a type to a Parameter as a predicate.
    /// </remarks>
    /// <param name="sqlTypes">
    /// the unsupported <see cref="SqlDbType"/> 
    /// </param>
    internal SqlTypeNotSupportedException(params IEnumerable<SqlDbType> sqlTypes) :
        base(CreateMessage(sqlTypes))
    {
    }

    /// <summary>
    /// Builds an error message listing unsupported SQL types and the properties they are associated with.
    /// </summary>
    /// <param name="names">
    /// the unsupported <see cref="SqlDbType"/> 
    /// </param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(params IEnumerable<SqlDbType> sqlTypes) =>
        $"The following SQL types are not supported by the SQL Builder in parameters: "
            + sqlTypes
                .Select(name => $"\"{name}\"")
                .JoinAnd();

    internal static void ValidateTypeIsSupported(SqlDbType type)
    {
        if (TypesNotSupported.Contains(type))
            throw new SqlTypeNotSupportedException(type);
    }
}
