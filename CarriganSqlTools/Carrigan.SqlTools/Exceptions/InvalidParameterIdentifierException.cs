using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more parameter identifiers fail to meet the SQL naming convention
/// required by the SQL generator.
/// </summary>
/// <remarks>
/// This exception indicates that a provided schema, table, column, parameter, or alias name
/// does not conform to the SQL identifier naming pattern enforced by the system.
/// 
/// <para><b>Note:</b></para>
/// For parameters, do <b>not</b> include the <c>@</c> symbol in the provided name.
/// The SQL generator automatically prepends the <c>@</c> symbol when constructing parameter identifiers.
/// </remarks>
public class InvalidParameterIdentifierException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidParameterIdentifierException"/> class.
    /// </summary>
    /// <param name="identifiers">
    /// A collection of invalid parameter, column, table, schema, or alias names that violated
    /// the SQL identifier naming convention.
    /// </param>
    /// <remarks>
    /// Each identifier in <paramref name="identifiers"/> is validated against the internal SQL
    /// naming pattern. If any are found to be invalid, this exception is thrown during SQL
    /// generation or reflection.
    /// </remarks>
    internal InvalidParameterIdentifierException(params IEnumerable<string?> identifiers) :
        base(CreateMessage(identifiers))
    {
    }

    /// <summary>
    /// Builds a detailed exception message listing all invalid identifiers.
    /// </summary>
    /// <param name="identifiers">The collection of invalid SQL identifiers.</param>
    /// <returns>
    /// A human-readable message listing the invalid parameters that failed validation.
    /// </returns>
    private static string CreateMessage(IEnumerable<string?> identifiers) =>
        $"The following Parameters do not follow the SQL naming convention: " +
            identifiers
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
