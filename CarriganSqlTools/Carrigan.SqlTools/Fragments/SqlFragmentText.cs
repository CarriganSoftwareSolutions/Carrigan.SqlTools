using System;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents a SQL fragment that emits literal SQL text.
/// </summary>
/// <remarks>
/// This fragment does not perform any formatting or normalization; it returns the text exactly as provided.
/// </remarks>
internal class SqlFragmentText : SqlFragment
{
    private readonly string _sqlText;

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentText"/>.
    /// </summary>
    /// <param name="text">The literal SQL text for this fragment.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is <c>null</c>.</exception>
    internal SqlFragmentText(string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));
        _sqlText = text;
    }

    /// <summary>
    /// Converts this fragment to SQL.
    /// </summary>
    /// <returns>The literal SQL text for this fragment.</returns>
    internal override string ToSql() =>
        _sqlText;
}
