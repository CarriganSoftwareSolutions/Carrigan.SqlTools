using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more SQL parameter identifiers fail to meet the naming convention
/// required by the SQL generator.
/// </summary>
/// <remarks>
/// <para>
/// This exception indicates that a provided parameter name does not conform to the SQL parameter
/// naming pattern enforced by the system.
/// </para>
/// <para>
/// For parameters, do not include the <c>@</c> symbol in the provided name. The SQL generator automatically
/// prepends the <c>@</c> symbol when constructing parameter identifiers.
/// </para>
/// </remarks>
public class InvalidParameterIdentifierException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidParameterIdentifierException"/> class.
    /// </summary>
    /// <param name="identifiers">
    /// A collection of invalid parameter identifiers that violated the SQL naming convention.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="identifiers"/> is <c>null</c>.</exception>
    internal InvalidParameterIdentifierException(params IEnumerable<string?> identifiers)
        : base(CreateMessage(identifiers))
    {
    }

    private static string CreateMessage(IEnumerable<string?> identifiers)
    {
        ArgumentNullException.ThrowIfNull(identifiers, nameof(identifiers));

        IReadOnlyCollection<string> invalidIdentifiers =
            [..
                identifiers
                    .Select(identifier => identifier ?? "<null>")
                    .Distinct()
            ];

        return $"The following parameter identifier(s) do not follow the SQL naming convention: {invalidIdentifiers.JoinAnd()}";
    }
}
