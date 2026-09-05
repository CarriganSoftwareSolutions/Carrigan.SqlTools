using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using Carrigan.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

public class Coalesce : SqlExpression
{
    private IEnumerable<SqlExpression> Values { get; init; }
    public override IEnumerable<TableTag> LeafTables { get; }

    
    public Coalesce(params IEnumerable<SqlExpression> values) : base([], ToDialectNeutralString(values))
    {
        IEnumerable<TableTag> GetLeafTables()
        {
            foreach (SqlExpression sqlExpression in Values)
                foreach (TableTag tableTag in sqlExpression.LeafTables)
                    yield return tableTag;
        }

        Values = ValidateValues(values);
        LeafTables = GetLeafTables();
    }

    private static string ToDialectNeutralString(IEnumerable<SqlExpression> values) =>
        $"COALESCE({string.Join(", ", Values.Select(value => value.ToString()))}"

    private static IEnumerable<SqlExpression> ValidateValues(IEnumerable<SqlExpression> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Count() < 2)
            throw new ArgumentException("Coalesce requires two or more values.");

        return values;
    }

    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("COALESCE(");
        foreach(ISqlFragment sqlFragment in Values.Select(value => (ISqlFragment) value).JoinFragments(ISqlFragment.CommaSpace))
        {
            yield return sqlFragment;
        }
        yield return ISqlFragment.CloseParentheses;
    }

    public override bool IsAggregate()
    {
        if(Values.Select(value => value.IsAggregate()).AllEqual() ?? false)
        {
            return Values.First().IsAggregate();
        }
        else
        {
            throw new AggregateInconsistencyException();
        }
    }
}
