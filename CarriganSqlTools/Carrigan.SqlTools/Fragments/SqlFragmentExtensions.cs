using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Fragments;

internal static class SqlFragmentExtensions
{
    internal static Dictionary<ParameterTag, object> GetParameters(this IEnumerable<SqlFragment> sqlFragments) =>
        sqlFragments
            .OfType<SqlFragmentParameter>()
            .Select(fragment => fragment.Parameter)
            .ToDictionary(pair => pair.Name, pair => pair.Value ?? DBNull.Value);
    internal static string ToSql(this IEnumerable<SqlFragment> sqlFragments) =>
        string.Concat(sqlFragments.Select(fragment => fragment.ToSql()));
}
