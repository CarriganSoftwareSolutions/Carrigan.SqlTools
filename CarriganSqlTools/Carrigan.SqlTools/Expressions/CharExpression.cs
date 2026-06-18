using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

public abstract class CharExpression : SqlExpression
{
    protected CharExpression(IEnumerable<CharExpression> childExpressions) : base(childExpressions)
    {
    }
}
