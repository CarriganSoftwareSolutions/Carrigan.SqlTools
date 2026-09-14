using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class CoalesceTests
{
    [Fact]
    public void DescendantLeafTables_ContainsNestedColumnTable()
    {
        Column<Customer> customerId = new(nameof(Customer.Id));
        Add nestedExpression = new(customerId, new Parameter(1, "Offset"));
        Coalesce coalesce = new(nestedExpression, new Parameter(0, "Fallback"));
        TableTag expected = customerId.ColumnInfo.ColumnTag.TableTag;

        TableTag actual = Assert.Single(coalesce.DescendantLeafTables);

        Assert.Equal(expected, actual);
    }
}
