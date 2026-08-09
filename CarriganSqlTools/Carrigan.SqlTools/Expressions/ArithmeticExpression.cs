using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

public abstract class ArithmeticExpression : NumericExpression
{
    /// <summary>
    /// The SQL logical operator text placed between rendered child predicates.
    /// </summary>
    private readonly string _operator;

    /// <summary>
    /// Base constructor for all arithmetic expression classes.
    /// </summary>
    /// <param name="operation">the operator for an arithmetic operation.</param>
    /// <param name="numericExpressions">Represents the child numeric expressions.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpressions"/> or  <paramref name="operation"/>  is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="numericExpressions"/> or  <paramref name="operation"/>  contains disallowed <c>null</c> values.
    /// </exception>
    protected ArithmeticExpression(string operation, IEnumerable<NumericExpression> numericExpressions)
        : base(numericExpressions, string.Join(operation, $"({numericExpressions.Select(expression => expression)})"))
    {
        ArgumentNullException.ThrowIfNull(numericExpressions, nameof(numericExpressions));
        ArgumentNullException.ThrowIfNull(operation, nameof(operation));

        if (operation.IsEmpty())
            throw new ArgumentException("Numeric operator text cannot be empty or whitespace.", nameof(operation));

        _operator = operation;
    }

    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        int index = 0;

        if (ChildNodes.Count() == 1)
        {
            foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
                yield return fragment;

            yield break;
        }

        yield return new SqlFragmentText("(");
        foreach (SqlExpression predicate in ChildNodes)
        {
            if (index > 0)
                yield return new SqlFragmentText($" {_operator} ");
            foreach (ISqlFragment fragment in predicate.ToSqlFragments(dialect))
                yield return fragment;
            index++;
        }
        yield return new SqlFragmentText(")");
    }
}
