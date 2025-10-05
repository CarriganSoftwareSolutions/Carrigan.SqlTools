using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
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
public class InvalidSqlIdentifierException : Exception
{
    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="identifiers">name of identifiers that are invalid</param>
    public InvalidSqlIdentifierException(params IEnumerable<string?> identifiers) :
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

    /// <summary>
    /// The class constructor for InvalidSqlIdentifierException
    /// The InvalidSqlIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="propertyNames">name of properties that have invalid names.</param>
    public InvalidSqlIdentifierException(params IEnumerable<PropertyName?> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidSqlIdentifierException"/>
    /// </summary>
    /// <param name="propertyNames">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidSqlIdentifierException"/> message.</returns>
    private static string CreateMessage(IEnumerable<PropertyName?> propertyNames) =>
        $"The following properties have SQL identifies that do not follow the SQL naming convention:" +
            propertyNames
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
