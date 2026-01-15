namespace Carrigan.SqlTools.Fragments;

internal class SqlFragmentText : SqlFragment
{
    protected string SqlText;

    public SqlFragmentText(string text) =>
        SqlText = text;

    internal override string ToSql() =>
        SqlText;
}
