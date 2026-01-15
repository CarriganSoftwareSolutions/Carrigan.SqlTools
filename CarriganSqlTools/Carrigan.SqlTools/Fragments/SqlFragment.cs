using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Fragments;

internal abstract class SqlFragment
{
    internal abstract string ToSql();

    public override string ToString() =>
        ToSql();
}
