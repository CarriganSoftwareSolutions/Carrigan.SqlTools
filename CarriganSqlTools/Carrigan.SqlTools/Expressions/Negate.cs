using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

public class Negate : NumericExpression
{
    /// <summary>
    /// Negate operator
    /// </summary>
    /// <param name="numericExpression">Represents the child numeric expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpression"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="numericExpression"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected Negate(NumericExpression numericExpression) : base([numericExpression], $"(-{numericExpression})") =>
        ArgumentNullException.ThrowIfNull(numericExpression, nameof(numericExpression));

    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(-");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return new SqlFragmentText(")");
    }
}
