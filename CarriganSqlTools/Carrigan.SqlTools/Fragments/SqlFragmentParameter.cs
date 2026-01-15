using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents an SQL fragment that renders a single parameter reference.
/// </summary>
/// <remarks>
/// This fragment wraps a <see cref="Parameter"/> so it can participate in fragment concatenation
/// while preserving access to the parameter’s tag and bound value for later materialization.
/// </remarks>
internal class SqlFragmentParameter : SqlFragment
{
    /// <summary>
    /// The wrapped parameter instance used for SQL rendering and parameter materialization.
    /// </summary>
    internal readonly Parameter Parameter;

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/> with the provided parameter.
    /// </summary>
    /// <param name="parameter">The parameter to wrap.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="parameter"/> is <c>null</c>.
    /// </exception>
    internal SqlFragmentParameter(Parameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        Parameter = parameter;
    }

    /// <summary>
    /// Converts this fragment into its SQL representation.
    /// </summary>
    /// <returns>The SQL parameter name produced by the wrapped <see cref="Parameter"/>.</returns>
    /// <remarks>
    /// Any exception thrown by <see cref="Parameter.ToSql"/> will be propagated to the caller.
    /// </remarks>
    internal override string ToSql() =>
        Parameter.ToSql();
}
