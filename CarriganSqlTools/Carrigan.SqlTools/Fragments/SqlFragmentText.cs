using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Fragments;

internal class SqlFragmentText : SqlFragment
{
    protected string SqlText;

    public SqlFragmentText(string text) =>
        SqlText = text;

    internal override string ToSql() =>
        SqlText;
}
