using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.PredicatesLogic;

internal class EmptyPredicate : Predicates
{
    internal EmptyPredicate() : base([])
    {
    }

    internal override IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates) => [];
}
