using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation. entire class

//TODO: Create example for readme.md file.
/// <summary>
/// The InvalidParameterIdentifierException is thrown when the identifier used for
/// generating a SQL schema, table, column, parameter or alias do not match
/// the sql naming pattern. 
/// Note: for parameters leave off the @, as the generator adds the leading @.
/// </summary>
public class InvalidParameterIdentifierException : Exception
{
    /// <summary>
    /// The class constructor for InvalidParameterIdentifierException
    /// The InvalidParameterIdentifierException is thrown when the identifier used for
    /// generating a SQL schema, table, column, parameter or alias do not match
    /// the sql naming pattern. 
    /// Note: for parameters leave off the @, as the generator adds the leading @.
    /// </summary>
    /// <param name="identifiers">name of identifiers that are invalid</param>
    internal InvalidParameterIdentifierException(params IEnumerable<string?> identifiers) :
        base(CreateMessage(identifiers))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidParameterIdentifierException"/>
    /// </summary>
    /// <param name="identifiers">The names of the invalid identifiers.</param>
    /// <returns>An <see cref="InvalidParameterIdentifierException"/> message.</returns>
    private static string CreateMessage(IEnumerable<string?> identifiers) =>
        $"The following Parameters do not follow the SQL naming convention:" +
            identifiers
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
