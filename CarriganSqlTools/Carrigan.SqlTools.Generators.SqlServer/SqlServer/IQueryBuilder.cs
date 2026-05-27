using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carrigan.SqlTools.SqlServer;

public interface IQueryBuilder
{
    SqlQuery AsSqlQuery();
}
