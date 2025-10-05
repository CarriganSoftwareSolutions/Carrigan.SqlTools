using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Redo documentation.
//TODO: Proof Read Documentation. entire class

//TODO: Create example for readme.md file.
//TODO: Unit tests?
//TODO: do a separate pattern for parameters?
/// <summary>
/// The InvalidSqlIdentifierException is thrown when the identifier used for
/// generating a SQL schema, table, column, parameter or alias do not match
/// the sql naming pattern. 
/// Note: for parameters leave off the @, as the generator adds the leading @.
/// </summary>
public class InvalidSqlIdentifierException: Exception
{
    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="identifierAttribute">name of IdentifierAttribute has invalid names.</param>
    internal InvalidSqlIdentifierException(IdentifierAttribute identifierAttribute):
        base(CreateMessage(identifierAttribute))
    {
    }

    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="id">name of IdentifierAttribute has invalid names.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
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

    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="propertyNames">name of properties that have invalid names.</param>
    internal InvalidSqlIdentifierException(Type type, TableName name) :
        base(CreateMessage(type, name))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="propertyNames">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(Type type, TableName name) =>
        $"Table name, \"{name}\" is an invalid SQL Identifier. Identifier comes from {type.GetQualifiedName()}.";

    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="propertyNames">name of properties that have invalid names.</param>
    internal InvalidSqlIdentifierException(Type type, SchemaName name) :
        base(CreateMessage(type, name))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="propertyNames">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(Type type, SchemaName name) =>
        $"Schema name, \"{name}\" is an invalid SQL Identifier. Identifier comes from {type.GetQualifiedName()}.";

    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="propertyNames">name of properties that have invalid names.</param>
    internal InvalidSqlIdentifierException(Type type, ProcedureName name) :
        base(CreateMessage(type, name))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="propertyNames">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(Type type, ProcedureName name) =>
        $"Procedure name, \"{name}\" is an invalid SQL Identifier. Identifier comes from {type.GetQualifiedName()}.";

    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="identifiers">name of identifiers that are invalid</param>
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, ColumnName>> names) :
        base(CreateMessage(names))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="identifiers">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, ColumnName>> names) =>
        $"The following column names do not follow the SQL naming convention: "
            + Environment.NewLine 
            + names
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + Environment.NewLine
            + "Columns are associated with the following properties:"
            + Environment.NewLine
            + names
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();

    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="identifiers">name of identifiers that are invalid</param>
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, AliasName>> names) :
        base(CreateMessage(names))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="identifiers">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, AliasName>> names) =>
        $"The following alias names do not follow the SQL naming convention: "
            + Environment.NewLine
            + names
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + Environment.NewLine
            + "Aliases are associated with the following properties:"
            + Environment.NewLine
            + names
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();

    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="identifiers">name of identifiers that are invalid</param>
    internal InvalidSqlIdentifierException(params IEnumerable<Tuple<PropertyInfo, ParameterTag>> names) :
        base(CreateMessage(names))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="identifiers">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(params IEnumerable<Tuple<PropertyInfo, ParameterTag>> names) =>
        $"The following parameter names do not follow the SQL naming convention: "
            + Environment.NewLine
            + names
                .Select(name => $"\"{name.Item2}\"")
                .JoinAnd()
            + Environment.NewLine
            + "Parameter are associated with the following properties:"
            + Environment.NewLine
            + names
                .Select(name => $"\"{name.Item1}\"")
                .JoinAnd();





    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="identifiers">name of identifiers that are invalid</param>
    [Obsolete]
    internal InvalidSqlIdentifierException(params IEnumerable<string?> identifiers) :
        base(CreateMessage(identifiers))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="identifiers">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(IEnumerable<string?> identifiers) =>
        $"The following identifies do not follow the SQL naming convention:" +
            identifiers
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
