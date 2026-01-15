using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Extension methods of <see cref="SqlFragment"/> and <see cref="IEnumerable{SqlFragment}"/>
/// </summary>
internal static class SqlFragmentExtensions
{
    /// <summary>
    /// Gets all Parameters from an <see cref="IEnumerable{SqlFragment}"/> as a <see cref="Dictionary{ParameterTag, object}/>
    /// </summary>
    /// <param name="sqlFragments"></param>
    /// <returns>Returns all Parameters from an <see cref="IEnumerable{SqlFragment}"/> as a <see cref="Dictionary{ParameterTag, object}/></returns>
    internal static Dictionary<ParameterTag, object> GetParameters(this IEnumerable<SqlFragment> sqlFragments) =>
        sqlFragments
            .OfType<SqlFragmentParameter>()
            .Select(fragment => fragment.Parameter)
            .ToDictionary(pair => pair.Name, pair => pair.Value ?? DBNull.Value);

    /// <summary>
    /// Generates a string that represents an <see cref="IEnumerable{SqlFragment}"/>
    /// </summary>
    /// <param name="sqlFragments">an ENumeration of <see cref="SqlFragment"/></param>
    /// <returns>a string that represents an <see cref="IEnumerable{SqlFragment}"/></returns>
    internal static string ToSql(this IEnumerable<SqlFragment> sqlFragments) =>
        string.Concat(sqlFragments.Select(fragment => fragment.ToSql()));
}
