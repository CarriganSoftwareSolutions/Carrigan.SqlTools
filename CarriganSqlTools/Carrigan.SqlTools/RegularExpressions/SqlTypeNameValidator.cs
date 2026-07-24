namespace Carrigan.SqlTools.RegularExpressions;

/// <summary>
/// Validates provider type names before they are emitted into generated SQL.
/// </summary>
internal static class SqlTypeNameValidator
{
    /// <summary>
    /// Normalizes and validates a provider type name.
    /// </summary>
    /// <param name="providerTypeName">The provider type name to validate.</param>
    /// <param name="allowedMultiWordTypeNames">
    /// Provider-defined multiword type names that are valid as complete tokens.
    /// </param>
    /// <returns>The normalized provider type name.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="providerTypeName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="providerTypeName"/> is empty, whitespace, or contains SQL syntax
    /// rather than a simple or schema-qualified type identifier.
    /// </exception>
    internal static string Normalize(string providerTypeName, IReadOnlySet<string>? allowedMultiWordTypeNames = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerTypeName);

        string normalized = providerTypeName.Trim().ToUpperInvariant();

        if (allowedMultiWordTypeNames?.Contains(normalized) == true)
            return normalized;

        string[] identifierParts = normalized.Split('.');

        if (identifierParts.Length is < 1 or > 3  || identifierParts.Any(SqlIdentifierPattern.Fails))
        {
            throw new ArgumentException("Provider type names must be simple or schema-qualified SQL identifiers.", nameof(providerTypeName));
        }

        return string.Join(".", identifierParts);
    }
}
