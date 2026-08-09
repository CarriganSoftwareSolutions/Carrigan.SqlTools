using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

public class Divide : ArithmeticExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Divide"/> class, representing
    /// the SQL logical <c>/</c> arithmetic operator.
    /// </summary>
    /// <param name="numericExpressions">
    /// One or more numeric expressions to  using <c>/</c>.
    /// </param>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Throws an <see cref="ArgumentNullException"/> if no numeric expressions are provided.</description></item>
    /// <item><description>If only one numeric expressions is provided, that predicate is used directly.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpressions"/> is <c>null</c> or contains no elements.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="numericExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    public Divide(IEnumerable<NumericExpression> numericExpressions) : base("/", numericExpressions)
    {
    }
}
