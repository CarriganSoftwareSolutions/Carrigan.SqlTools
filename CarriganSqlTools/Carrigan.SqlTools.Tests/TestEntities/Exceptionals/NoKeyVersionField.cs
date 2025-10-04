using Carrigan.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
public class NoKeyVersionField
{
    [Encrypted]
    public string? Encrypted { get; set; }
}
