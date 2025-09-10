using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class Template
{
    private SqlGenerator<Customer> customerGenerator = new();
    private SqlGenerator<Order> orderGenerator = new();

}
