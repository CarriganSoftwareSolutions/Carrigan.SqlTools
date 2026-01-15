using Carrigan.SqlTools.PredicatesLogic;
using System;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents a SQL fragment that renders a predicate <see cref="Parameter"/>.
/// </summary>
/// <remarks>
/// This fragment is used by fragment-based predicate rendering to keep SQL text generation
/// and parameter extraction aligned.
/// </remarks>
internal class SqlFragmentParameter : SqlFragment
{
    /// <summary>
    /// The underlying predicate parameter represented by this fragment.
    /// </summary>
    internal readonly Parameter Parameter;

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/>.
    /// </summary>
    /// <param name="parameter">The parameter to render.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is <c>null</c>.</exception>
    internal SqlFragmentParameter(Parameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        Parameter = parameter;
    }

    /// <summary>
    /// Converts this fragment to SQL.
    /// </summary>
    /// <returns>The SQL representation of the underlying <see cref="Parameter"/>.</returns>
    internal override string ToSql() =>
        Parameter.ToSql();
}
