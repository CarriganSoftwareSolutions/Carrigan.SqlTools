namespace Carrigan.SqlTools.Fragments;

internal class SqlFragmentText : SqlFragment
{
    protected readonly string SqlText;

    internal SqlFragmentText(string text) =>
        SqlText = text;

    internal override string ToSql() =>
        SqlText;
}
