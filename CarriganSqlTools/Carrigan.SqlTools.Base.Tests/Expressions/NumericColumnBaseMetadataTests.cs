using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

#pragma warning disable CS0618 // NumericColumnBase is retained for backwards compatibility.
public class NumericColumnBaseMetadataTests
{
    private sealed class TestNumericColumn : NumericColumnBase<Grades>
    {
        internal TestNumericColumn() : base(new ColumnBase<Grades>(new HashSet<Type> { typeof(decimal) }, nameof(Grades.GradePoint)))
        {
        }
    }

    [Fact]
    public void HasColumns_ReturnsTrue() =>
        Assert.True(new TestNumericColumn().HasColumns());

    [Fact]
    public void LeafTables_ContainsWrappedColumnTable()
    {
        TestNumericColumn column = new();
        TableTag leafTable = Assert.Single(column.LeafTables);
        TableTag descendantLeafTable = Assert.Single(column.DescendantLeafTables);

        Assert.Equal(column.ColumnInfo.ColumnTag.TableTag, leafTable);
        Assert.Equal(column.ColumnInfo.ColumnTag.TableTag, descendantLeafTable);
    }
}
#pragma warning restore CS0618
