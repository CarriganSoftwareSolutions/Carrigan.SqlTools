using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.SqlGenerators;

public interface IAliasedTable
{
    public TableAlias? TableAlias { get; }
}
