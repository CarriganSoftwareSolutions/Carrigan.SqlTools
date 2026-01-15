using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Fragments;

internal class SqlFragmentParameter : SqlFragment
{
    internal readonly Parameter Parameter;
    internal SqlFragmentParameter(Parameter parameter) =>
        Parameter = parameter;

    internal override string ToSql() =>
        Parameter.ToSql();
}
