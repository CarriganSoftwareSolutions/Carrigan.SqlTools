using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more SQL identifiers used for schema, table, column,
/// parameter, or alias names fail to match the SQL naming convention enforced
/// by the SQL generator.
/// </summary>
/// <remarks>
/// This exception typically occurs during reflection or SQL generation when
/// the system encounters an identifier that violates the internal SQL naming
/// pattern (for example, starting with a digit or containing invalid symbols).
/// 
/// <para><b>Note:</b></para>
/// For parameter identifiers, omit the leading <c>@</c> symbol; the SQL generator
/// automatically adds it during parameter construction.
/// </remarks>
public class InvalidSqlIdentifierException: Exception
{
    #region TabelName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid table name associated with a specific declaring <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="TableName"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.
    /// </exception>
    internal InvalidSqlIdentifierException(Type type, TableName name) :
        base(CreateMessage(type, name))
    {
    }

    /// <summary>
    /// Builds an error message describing an invalid table name for a given declaring type.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="TableName"/>.</param>
    /// <returns>A formatted error message.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.
    /// </exception>
    private static string CreateMessage(Type type, TableName name)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        return $"Table name \"{name}\" is an invalid SQL identifier. Identifier comes from {type.GetQualifiedName()}.";
    }
    #endregion

    #region SchemaName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid schema name associated with a specific declaring <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="SchemaName"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.
    /// </exception>
    internal InvalidSqlIdentifierException(Type type, SchemaName name) :
        base(CreateMessage(type, name))
    {
    }

    /// <summary>
    /// Builds an error message describing an invalid schema name for a given declaring type.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="SchemaName"/>.</param>
    /// <returns>A formatted error message.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.
    /// </exception>
    private static string CreateMessage(Type type, SchemaName name)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        return $"Schema name \"{name}\" is an invalid SQL identifier. Identifier comes from {type.GetQualifiedName()}.";
    }
    #endregion

    #region ProcedureName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid stored procedure name associated with a specific declaring <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="ProcedureName"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.
    /// </exception>
    internal InvalidSqlIdentifierException(Type type, ProcedureName name) :
        base(CreateMessage(type, name))
    {
    }

    /// <summary>
    /// Builds an error message describing an invalid stored procedure name for a given declaring type.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="ProcedureName"/>.</param>
    /// <returns>A formatted error message.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.
    /// </exception>
    private static string CreateMessage(Type type, ProcedureName name)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        return $"Procedure name \"{name}\" is an invalid SQL identifier. Identifier comes from {type.GetQualifiedName()}.";
    }
    #endregion

    #region ColumnName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for one or more invalid column names and their associated properties.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="ColumnName"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="names"/> is <c>null</c>.</exception>
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, ColumnName>> names) :
        base(CreateMessage(names))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid column names and the properties they are associated with.
    /// </summary>
    /// <param name="pairs">
    /// Tuples pairing the invalid <see cref="ColumnName"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <returns>A formatted error message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pairs"/> is <c>null</c>.</exception>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, ColumnName>> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs, nameof(pairs));

        return "The following column name(s) do not follow the SQL naming convention: "
            + pairs.Select(pair => $"\"{pair.Item2}\"").JoinAnd()
            + ". Columns are associated with the following propertie(s): "
            + pairs.Select(pair => $"\"{FormatPropertyInfo(pair.Item1)}\"").JoinAnd()
            + ".";
    }
    #endregion

    #region AliasName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for one or more invalid alias names and their associated properties.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="AliasName"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="names"/> is <c>null</c>.</exception>
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, AliasName>> names) :
        base(CreateMessage(names))
    {
    }

    /// Builds an error message listing invalid alias names and the properties they are associated with.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="AliasName"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <returns>A formatted error message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="names"/> is <c>null</c>.</exception>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, AliasName>> names)
    {
        ArgumentNullException.ThrowIfNull(names, nameof(names));

        IReadOnlyCollection<Tuple<PropertyInfo, AliasName>> pairs = [.. names];

        return "The following alias name(s) do not follow the SQL naming convention: "
            + pairs.Select(pair => $"\"{pair.Item2}\"").JoinAnd()
            + ". Aliases are associated with the following propertie(s): "
            + pairs.Select(pair => $"\"{FormatPropertyInfo(pair.Item1)}\"").JoinAnd()
            + ".";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid alias name.
    /// </summary>
    /// <param name="name">The invalid alias name.</param>
    internal InvalidSqlIdentifierException(params IEnumerable<AliasName> names) :
        base(CreateMessage(names))
    {
    }

    /// <summary>
    /// Builds an error message describing an invalid alias name.
    /// </summary>
    /// <param name="name">The invalid alias name.</param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(params IEnumerable<AliasName> names) =>
        $"The following alias names do not follow the SQL naming convention: "
            + names
                .Select(name => name.ToString())
                .JoinAnd();
    #endregion

    #region ParameterTag 
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for one or more invalid parameter names and their associated properties.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="ParameterTag"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="names"/> is <c>null</c>.</exception>
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, ParameterTag>> names) :
        base(CreateMessage(names))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid parameter names and the properties they are associated with.
    /// </summary>
    /// <param name="pairs">
    /// Tuples pairing the invalid <see cref="ParameterTag"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <returns>A formatted error message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pairs"/> is <c>null</c>.</exception>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, ParameterTag>> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs, nameof(pairs));

        return "The following parameter name(s) do not follow the SQL naming convention: "
            + pairs.Select(pair => $"\"{pair.Item2}\"").JoinAnd()
            + ". Parameters are associated with the following propertie(s): "
            + pairs.Select(pair => $"\"{FormatPropertyInfo(pair.Item1)}\"").JoinAnd()
            + ".";
    }
    #endregion

    #region Obsolete String Base Identifiers, still used for RoleTags
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for one or more invalid string identifiers.
    /// </summary>
    /// <param name="identifiers">The invalid identifiers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="identifiers"/> is <c>null</c>.</exception>
    [Obsolete]
    internal InvalidSqlIdentifierException(params string?[] identifiers)
        : base(CreateMessage(identifiers))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid identifiers.
    /// </summary>
    /// <param name="identifiers">The invalid identifiers.</param>
    /// <returns>A formatted error message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="identifiers"/> is <c>null</c>.</exception>
    private static string CreateMessage(IEnumerable<string?> identifiers)
    {
        ArgumentNullException.ThrowIfNull(identifiers, nameof(identifiers));

        return "The following identifier(s) do not follow the SQL naming convention: "
            + identifiers.Select(identifier => identifier ?? "<null>").JoinAnd()
            + ".";
    }
    #endregion

    private static string FormatPropertyInfo(PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null)
            return "<null>";

        else
            return propertyInfo.Name;
    }
}
