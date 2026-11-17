using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

public class From : ISelectable, ISqlFragment
{
    public readonly ISqlFragment FromFragment;
    public readonly TableAlias? TableAlias;
    public SelectTags SelectTags { get; }
    internal From(TableTag tableTag, SelectTags selectTags, TableAlias? tableAlias = null)
    {
        FromFragment = tableTag;
        SelectTags = selectTags.ResultReferences(tableAlias);
        TableAlias = tableAlias;
    }
    public From(Subquery subquery, TableAlias? tableAlias = null)
    {
        FromFragment = subquery;
        SelectTags = subquery.SelectTags.ResultReferences(tableAlias);
        TableAlias = tableAlias;
    }

    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        ToSqlFragments(dialect).OfType<SqlFragmentParameter>();
    
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        ToSqlFragments(dialect);

    public IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        foreach(ISqlFragment sqlFragment in FromFragment.Flatten(dialect))
        {
            yield return sqlFragment;
        }
        if(TableAlias is not null)
        {
            yield return ISqlFragment.Space;
            yield return ISqlFragment.As;
            yield return ISqlFragment.Space;
            yield return TableAlias;
        }
    }

    public string ToSql(ISqlDialects dialect) =>
        ToSqlFragments(dialect).ToSql(dialect);
}
