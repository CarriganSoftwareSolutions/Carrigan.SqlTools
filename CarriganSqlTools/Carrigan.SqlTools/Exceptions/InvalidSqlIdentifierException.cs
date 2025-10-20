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
    #region IdentifierAttribute
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid <see cref="IdentifierAttribute"/>.
    /// </summary>
    /// <param name="identifierAttribute">
    /// The <see cref="IdentifierAttribute"/> instance that supplied the invalid identifier.
    /// </param>
    internal InvalidSqlIdentifierException(IdentifierAttribute identifierAttribute) :
        base(CreateMessage(identifierAttribute))
    {
    }

    /// <summary>
    /// Builds an error message describing an invalid identifier provided via an
    /// <see cref="IdentifierAttribute"/>.
    /// </summary>
    /// <param name="id">The attribute instance that supplied the invalid identifier.</param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(IdentifierAttribute id)
    {
        static string GetName(string? schema, string table)
        {
            if (schema == null)
                return table;
            else
                return $"{schema}.{table}";
        }

        return $"SQL Identifier, \"{GetName(id.Schema, id.Name)}\", is invalid. Identifier comes from {id.MemberName}";
    }
    #endregion

    #region TabelName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid table name associated with a specific declaring <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="TableName"/>.</param>
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
    private static string CreateMessage(Type type, TableName name) =>
        $"Table name, \"{name}\" is an invalid SQL Identifier. Identifier comes from {type.GetQualifiedName()}.";
    #endregion

    #region SchemaName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid schema name associated with a specific declaring <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="SchemaName"/>.</param>
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
    private static string CreateMessage(Type type, SchemaName name) =>
        $"Schema name, \"{name}\" is an invalid SQL Identifier. Identifier comes from {type.GetQualifiedName()}.";
    #endregion

    #region ProcedureName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for an invalid stored procedure name associated with a specific declaring <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The declaring type where the invalid identifier was discovered.</param>
    /// <param name="name">The invalid <see cref="ProcedureName"/>.</param>
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
    private static string CreateMessage(Type type, ProcedureName name) =>
        $"Procedure name, \"{name}\" is an invalid SQL Identifier. Identifier comes from {type.GetQualifiedName()}.";
    #endregion

    #region SchenaName
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for one or more invalid column names and their associated properties.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="ColumnName"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, ColumnName>> names) :
        base(CreateMessage(names))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid column names and the properties they are associated with.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="ColumnName"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, ColumnName>> names) =>
        $"The following column names do not follow the SQL naming convention: "
            + names
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + " Columns are associated with the following properties: "
            + names
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();
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
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, AliasName>> names) :
        base(CreateMessage(names))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid alias names and the properties they are associated with.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="AliasName"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, AliasName>> names) =>
        $"The following alias names do not follow the SQL naming convention: "
            + names
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + " Aliases are associated with the following properties: "
            + names
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// for one or more invalid alias names.
    /// </summary>
    /// <param name="names">The invalid <see cref="AliasName"/> values.</param>
    internal InvalidSqlIdentifierException(params IEnumerable<AliasName> names) :
        base(CreateMessage(names))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid alias names.
    /// </summary>
    /// <param name="names">The invalid <see cref="AliasName"/> values.</param>
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
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, ParameterTag>> names) :
        base(CreateMessage(names))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid parameter names and the properties they are associated with.
    /// </summary>
    /// <param name="names">
    /// Tuples pairing the invalid <see cref="ParameterTag"/> values with the
    /// <see cref="PropertyInfo"/> each originated from.
    /// </param>
    /// <returns>A formatted error message.</returns>

    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, ParameterTag>> names) =>
        $"The following parameter names do not follow the SQL naming convention: "
            + names
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + " Parameter are associated with the following properties: "
            + names
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();
    #endregion

    #region Obsolete String Base Identifiers, still used for RoleTags
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSqlIdentifierException"/> class
    /// using a legacy string-based collection of identifier values.
    /// </summary>
    /// <param name="identifiers">The invalid identifier strings.</param>
    [Obsolete]
    internal InvalidSqlIdentifierException(params IEnumerable<string?> identifiers) :
        base(CreateMessage(identifiers))
    {
    }

    /// <summary>
    /// Builds an error message listing invalid identifiers (legacy string-based overload).
    /// </summary>
    /// <param name="identifiers">The invalid identifier strings.</param>
    /// <returns>A formatted error message.</returns>
    private static string CreateMessage(IEnumerable<string?> identifiers) =>
        $"The following identifies do not follow the SQL naming convention: " +
            identifiers
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd(); 
    #endregion
}
