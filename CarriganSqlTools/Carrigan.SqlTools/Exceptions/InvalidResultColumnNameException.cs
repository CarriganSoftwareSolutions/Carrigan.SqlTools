using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;

//TODO: Proof Read Documentation. entire class
/// <summary>
/// The InvalidResultColumnNameException is thrown when a <see cref="ResultColumnName"/> is passed in that does exist
/// in the target class <see cref="T"/>.
/// </summary>
/// <typeparam name="T">Type T for which the <see cref="ResultColumnName"/> was to be used.</typeparam>
public class InvalidResultColumnNameException<T> : Exception
{
    /// <summary>
    /// This is the constructor for InvalidResultColumnNameException.
    /// The InvalidResultColumnNameException is thrown when a <see cref="ResultColumnName"/> is passed in that does exist
    /// in the target class <see cref="T"/>.
    /// </summary>
    /// <param name="resultColumnNames">The names of the invalid <see cref="ResultColumnName"/>.</param>
    internal InvalidResultColumnNameException(params IEnumerable<ResultColumnName> resultColumnNames) :
        base(CreateMessage(resultColumnNames))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidResultColumnNameException{T}"/>
    /// </summary>
    /// <param name="ResultColumnNames">The names of the invalid <see cref="ResultColumnName"/>.</param>
    /// <returns>An <see cref="InvalidResultColumnNameException{T}"/> message.</returns>
    private static string CreateMessage(IEnumerable<ResultColumnName> ResultColumnNames) =>
        $"The ADO column name does not have corresponding property name that matches: " +
            ResultColumnNames
                .Select(ResultColumnName => (string)ResultColumnName) 
                .JoinAnd();
}
