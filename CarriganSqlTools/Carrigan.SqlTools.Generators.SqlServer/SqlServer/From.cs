using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlServer;

public class From<modelT> : From where modelT: class
{
    public From(TableAlias? tableAlias = null) : base(SqlToolsReflectorCache<modelT>.Table, SelectTagGenerator.GetAll<modelT>(), tableAlias)
    {

    }
}
