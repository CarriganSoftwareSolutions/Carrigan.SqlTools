using Carrigan.Core.Extensions;
using System.Data;
using System.Reflection;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more SQL types are not supported by the SQL generator.
/// </summary>
/// <remarks>
/// This exception typically occurs when using <see cref="Attributes.SqlTypeAttribute"/> on a property,
/// or when providing an explicit <see cref="SqlDbType"/> to a parameter in a predicate.
/// </remarks>
public class SqlTypeNotSupportedException : Exception
{
    /// <summary>
    /// SQL types that are not currently supported by the SQL generator.
    /// </summary>
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
    internal SqlTypeNotSupportedException(params IEnumerable<(PropertyInfo Property, SqlDbType SqlType)> sqlTypes) :
        base(CreateMessage(sqlTypes))
    {
    }

    /// <summary>
    /// Builds an error message listing unsupported SQL types and the properties they are associated with.
    /// </summary>
    /// <param name="sqlTypes">
    /// Tuples pairing the <see cref="PropertyInfo"/> from which the type originated and
    /// the unsupported <see cref="SqlDbType"/> value.
    /// </param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(params IEnumerable<(PropertyInfo Property, SqlDbType SqlType)> sqlTypes) =>
        $"The following SQL types are not supported by the SQL Builder: "
            + sqlTypes
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + " Columns are associated with the following properties: "
            + sqlTypes
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTypeNotSupportedException"/> class
    /// when one or more SQL types are not supported in parameter usage.
    /// </summary>
    /// <remarks>
    /// This overload is typically used when a caller explicitly specifies an unsupported
    /// <see cref="SqlDbType"/> for a parameter in a predicate or query.
    /// </remarks>
    /// <param name="sqlTypes">
    /// One or more unsupported <see cref="SqlDbType"/> values.
    /// </param>
    internal SqlTypeNotSupportedException(params IEnumerable<SqlDbType> sqlTypes) :
        base(CreateMessage(sqlTypes))
    {
    }

    /// <summary>
    /// Builds an error message listing unsupported SQL types used in parameters.
    /// </summary>
    /// <param name="sqlTypes">
    /// One or more unsupported <see cref="SqlDbType"/> values.
    /// </param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(params IEnumerable<SqlDbType> sqlTypes) =>
        $"The following SQL types are not supported by the SQL Builder in parameters: "
            + sqlTypes
                .Select(name => $"\"{name}\"")
                .JoinAnd();

    /// <summary>
    /// Throws an exception if the specified <paramref name="type"/> exists in <see cref="TypesNotSupported"/>.
    /// </summary>
    /// <param name="type">A <see cref="SqlDbType"/> value to test.</param>
    /// <exception cref="SqlTypeNotSupportedException">
    /// Thrown if the <paramref name="type"/> exists in <see cref="TypesNotSupported"/>.
    /// </exception>
    internal static void ValidateTypeIsSupported(SqlDbType type)
    {
        if (TypesNotSupported.Contains(type))
            throw new SqlTypeNotSupportedException(type);
    }
}
