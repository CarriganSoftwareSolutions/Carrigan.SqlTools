using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

public abstract class NumericExpression : SqlExpression
{
    protected NumericExpression(IEnumerable<NumericExpression> childExpressions) : base(childExpressions)
    {
    }
}
