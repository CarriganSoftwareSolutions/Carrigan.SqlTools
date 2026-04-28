using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DialectAttribute : Attribute
{
    internal DialectEnum DialectEnum { get; private init; }
    public DialectAttribute(DialectEnum dialect = DialectEnum.SqlServer) => 
        DialectEnum = dialect;
}
