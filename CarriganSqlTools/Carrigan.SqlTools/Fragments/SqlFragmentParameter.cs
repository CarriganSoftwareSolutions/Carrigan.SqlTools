using Carrigan.SqlTools.PredicatesLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Fragments;

internal class SqlFragmentParameter : SqlFragment
{
    internal Parameter Parameter;
    internal SqlFragmentParameter(Parameter parameter) =>
        Parameter = parameter;

    internal override string ToSql() =>
        Parameter.ToSql();
}
