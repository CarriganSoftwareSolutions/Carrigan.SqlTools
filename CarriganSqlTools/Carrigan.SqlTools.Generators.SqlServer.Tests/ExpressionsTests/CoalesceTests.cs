using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class CoalesceTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Coalesce(null!, Value)),
        (() => new Coalesce(Value, null!)),
        (() => new Coalesce(null!, Value, First)),
        (() => new Coalesce(Value, null!, First)),
        (() => new Coalesce(Value, First, null!)),
    ];


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
