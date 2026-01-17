using Carrigan.Core.Enums;
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
            SqlDbType.Udt
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sqlTypes"/> is <c>null</c>.</exception>
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
    private static string CreateMessage(params IEnumerable<(PropertyInfo Property, SqlDbType SqlType)> sqlTypes)
    {
        ArgumentNullException.ThrowIfNull(sqlTypes, nameof(sqlTypes));

        IEnumerable<(PropertyInfo Property, SqlDbType SqlType)> materialized =
            sqlTypes.Materialize(NullOptionsEnum.Allowed);

        IReadOnlyCollection<string> types =
            [..
                materialized
                    .Select(static pair => pair.SqlType.ToString())
                    .Distinct()
                    .Select(static type => $"\"{type}\"")
            ];

        IReadOnlyCollection<string> properties =
            [..
                materialized
                    .Select(static pair => FormatProperty(pair.Property))
                    .Distinct()
                    .Select(static property => $"\"{property}\"")
            ];

        return "The following SQL types are not supported by the SQL Builder: "
            + types.JoinAnd()
            + " Columns are associated with the following properties: "
            + properties.JoinAnd()
            + ".";
    }

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
    private static string CreateMessage(params IEnumerable<SqlDbType> sqlTypes)
    {
        ArgumentNullException.ThrowIfNull(sqlTypes, nameof(sqlTypes));

        IEnumerable<SqlDbType> materialized =
            sqlTypes.Materialize(NullOptionsEnum.Allowed);

        IReadOnlyCollection<string> types =
            [..
                materialized
                    .Select(static type => type.ToString())
                    .Distinct()
                    .Select(static type => $"\"{type}\"")
            ];

        return "The following SQL type(s) are not supported by the SQL generator in parameters: "
            + types.JoinAnd()
            + ".";
    }

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

    private static string FormatProperty(PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null)
            return "<null>";

        else
            return propertyInfo.Name;
    }
}
