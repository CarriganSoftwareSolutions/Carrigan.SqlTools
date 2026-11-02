using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.JoinTypes;

internal class JoinFragment : ISqlFragment
{
    internal static JoinFragment CrossJoin = new(new SqlFragmentText("CROSS JOIN"), false);
    internal static JoinFragment FullJoin = new(new SqlFragmentText("FULL JOIN"));
    internal static JoinFragment InnerJoin = new(new SqlFragmentText("INNER JOIN"));
    internal static JoinFragment Join = new(new SqlFragmentText("JOIN"));
    internal static JoinFragment LeftJoin = new(new SqlFragmentText("LEFT JOIN"));
    internal static JoinFragment RightJoin = new(new SqlFragmentText("RIGHT JOIN"));

    private readonly ISqlFragment SqlFragment;

    internal readonly bool HasPredicates;

    public JoinFragment(ISqlFragment sqlFragment, bool hasPredicates = true)
    {
        SqlFragment = sqlFragment;
        HasPredicates = hasPredicates;
    }

    [ExternalOnly]
    public JoinFragment(string joinText, bool hasPredicates = true)
    {
        SqlFragment = new SqlFragmentText(joinText);
        HasPredicates = hasPredicates;
    }

    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        SqlFragment.Flatten(dialect);

    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        SqlFragment.GetSqlFragmentParameters(dialect);

    string ISqlFragment.ToSql(ISqlDialects dialect) =>
        SqlFragment.ToSql(dialect);
}
