using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base type for all SQL predicate nodes used to compose <c>WHERE</c> and <c>JOIN</c> conditions.
/// Supports recursive SQL generation and parameter collection (with automatic de-duplication/prefixing).
/// </summary>
public abstract class Predicates

{
    /// <summary>
    /// Recursively enumerates all <see cref="Parameter"/>s contained within this predicate tree.
    /// </summary>
    internal abstract IEnumerable<Parameter> Parameters { get; }

    /// <summary>
    /// Recursively enumerates all <see cref="ColumnBase"/> nodes contained within this predicate tree.
    /// </summary>
    internal abstract IEnumerable<ColumnBase> Columns { get; }

    /// <summary>
    /// Generates the SQL fragment for this predicate tree.
    /// </summary>
    /// <remarks>
    /// Before rendering, this method computes duplicate user-supplied parameter names and
    /// passes that set to the recursive <see cref="ToSql(string, IEnumerable{ParameterTag})"/> overload,
    /// which may add disambiguating prefixes to produce unique parameter names.
    /// </remarks>
    /// <returns>The SQL fragment represented by this predicate tree.</returns>
    internal string ToSql()
    {
        //get an IEnumerable of all the duplicate parameter names
        IEnumerable<ParameterTag> duplicates = Parameters
            .Select(parameter => parameter.Name)
            .GroupBy(name => name)
            .Where(nameGroup => nameGroup.Count() > 1)
            .Select(nameGroup => nameGroup.Key);

        return ToSql(string.Empty, duplicates);
    }

    /// <summary>
    /// Recursively generates the SQL fragment for this predicate tree, applying a prefix to
    /// duplicate parameter names to ensure uniqueness and alignment with <see cref="GetParameters()"/>.
    /// </summary>
    /// <param name="prefix">
    /// A recursion-built prefix used to disambiguate duplicate parameter names. This is applied
    /// only when the parameter’s base name appears in <paramref name="duplicates"/>.
    /// </param>
    /// <param name="duplicates">
    /// The set of base <see cref="ParameterTag"/> names that occur more than once within the predicate tree.
    /// </param>
    /// <returns>The SQL fragment for this node.</returns>
    internal abstract string ToSql(string prefix, IEnumerable<ParameterTag> duplicates);

    /// <summary>
    /// Recursively collects all parameters as key–value pairs suitable for command binding.
    /// </summary>
    /// <remarks>
    /// This method computes duplicates in the same way as <see cref="ToSql()"/> and uses the recursive
    /// <see cref="GetParameters(string, IEnumerable{ParameterTag})"/> overload to apply the same
    /// disambiguating prefixes, ensuring parameter names in the emitted SQL match the binding keys.
    /// </remarks>
    /// <returns>
    /// A dictionary mapping the final <see cref="ParameterTag"/> for each parameter to its value.
    /// </returns>
    internal Dictionary<ParameterTag,object> GetParameters()
    {
        //get an IEnumerable of all the duplicate parameter names
        IEnumerable<ParameterTag> duplicates = Parameters
            .Select(parameter => parameter.Name)
            .GroupBy(parameter => parameter)
            .Where(nameGroup => nameGroup.Count() > 1)
            .Select(nameGroup => nameGroup.Key);

        return GetParameters(string.Empty, duplicates)
               .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    /// <summary>
    /// Recursively collects parameters as key–value pairs, applying a prefix to duplicate names
    /// to ensure uniqueness and alignment with <see cref="ToSql(string, IEnumerable{ParameterTag})"/>.
    /// </summary>
    /// <param name="prefix">
    /// A recursion-built prefix used to disambiguate duplicate parameter names. This is applied
    /// only when the parameter’s base name appears in <paramref name="duplicates"/>.
    /// </param>
    /// <param name="duplicates">
    /// The set of base <see cref="ParameterTag"/> names that occur more than once within the predicate tree.
    /// </param>
    /// <returns>An enumeration of final parameter tags and their associated values.</returns>
    internal abstract IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates);
}
