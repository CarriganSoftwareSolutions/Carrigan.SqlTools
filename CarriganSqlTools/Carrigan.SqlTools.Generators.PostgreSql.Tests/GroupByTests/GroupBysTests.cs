using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.GroupByTests;

public class GroupBysTests
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();


    [Fact]
    public void Constructor_WithNoItems_CreatesEmptyGroupBy()
    {
        GroupBys groupBy = new();

        Assert.True(groupBy.IsEmpty());
        Assert.Equal(string.Empty, groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithEmptyCollection_CreatesEmptyGroupBy()
    {
        GroupBy[] groupByItems = [];

        GroupBys groupBy = new(groupByItems);

        Assert.True(groupBy.IsEmpty());
        Assert.Equal(string.Empty, groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithNullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<GroupBy>? groupByItems = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new GroupBys(groupByItems!));
        Assert.Equal("groupByItems", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithSingleItem_CreatesExpectedSql()
    {
        GroupBy<Address> groupByItem = new("City");

        GroupBys groupBy = new(groupByItem);

        Assert.False(groupBy.IsEmpty());
        Assert.Single(groupBy.TableTags);
        Assert.Equal("GROUP BY \"Address\".\"City\"", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithMultipleItems_CreatesExpectedSql()
    {
        GroupBy<Address> city = new("City");
        GroupBy<Address> street = new("Street");

        GroupBys groupBy = new(city, street);

        Assert.Equal(2, groupBy.TableTags.Count());
        Assert.Equal("GROUP BY \"Address\".\"City\", \"Address\".\"Street\"", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithMultipleTables_CreatesExpectedSql()
    {
        GroupBy<Address> address = new("City");
        GroupBy<ColumnTable> columnTable = new("D000destruct0");
        GroupBy<BooleanColumnTable> booleanColumnTable = new("Id");

        GroupBys groupBy = new(address, columnTable, booleanColumnTable);

        Assert.Equal(3, groupBy.TableTags.Count());
        Assert.Equal("GROUP BY \"Address\".\"City\", \"ColumnTable\".\"D000destruct0\", \"BooleanColumnTable\".\"Id\"", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void GroupByItemBasesAsEnumerable_ReturnsItemsInGroup()
    {
        GroupBy<Address> city = new("City");
        GroupBy<Address> street = new("Street");
        GroupBys groupBy = new(city, street);

        List<GroupBy> groupByItems = [.. groupBy.AsEnumerable()];
        List<GroupBy> expectedGroupByItems = [city, street];

        Assert.Equal(expectedGroupByItems, groupByItems);
    }

    [Fact]
    public void GroupByItemsAsEnumerable_ReturnsItemsInGroup()
    {
        GroupBy<Address> city = new("City");
        GroupBy<Address> street = new("Street");
        GroupBys groupBy = new(city, street);

        List<GroupBy> groupByItems = [.. groupBy.AsEnumerable()];
        List<GroupBy> expectedGroupByItems = [city, street];

        Assert.Equal(expectedGroupByItems, groupByItems);
    }

    [Fact]
    public void Contains_ReturnsTrue_ForExistingItemReference()
    {
        GroupBy<Address> item = new("City");
        GroupBys groupBy = new(item);

        Assert.True(groupBy.Contains(item));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        GroupBy<Address> street = new("Street");
        GroupBy<Address> city = new("City");
        GroupBy<ColumnTable> columnTable = new("D000destruct0");
        GroupBys groupBy = new(street);

        Assert.False(groupBy.Contains(city));
        Assert.False(groupBy.Contains(columnTable));
    }

    [Fact]
    public void Contains_Select()
    {
        GroupBy<Address> street = new("Street");
        GroupBy<Address> city = new("City");
        GroupBys groupBy = new(street);

        SelectTag streetSelectTag = SelectTagGenerator.Get<Address>("Street");
        SelectTag citySelectTag = SelectTagGenerator.Get<Address>("City");

        Assert.True(groupBy.ContainsEquivalent(streetSelectTag));
        Assert.False(groupBy.ContainsEquivalent(citySelectTag));
    }

    [Fact]
    public void Contains_WithColumnExpressionSelectTag_ReturnsExpectedResult()
    {
        GroupBys groupBy = new(new GroupBy<Address>(nameof(Address.Street)));
        SelectTag<Address> streetSelectTag = new(nameof(Address.Street));
        SelectTag<Address> citySelectTag = new(nameof(Address.City));

        Assert.True(groupBy.ContainsEquivalent(streetSelectTag));
        Assert.False(groupBy.ContainsEquivalent(citySelectTag));
    }

    [Fact]
    public void Contains_WithNullItem_ThrowsArgumentNullException_NullColumnBase()
    {
        GroupBys groupBy = new(new GroupBy<Address>("Street"));

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => groupBy.Contains((Column)(null!)));
        Assert.Equal("column", exception.ParamName);
    }

    [Fact]
    public void Contains_WithNullItem_ThrowsArgumentNullException_NullGroupByBase()
    {
        GroupBys groupBy = new(new GroupBy<Address>("Street"));

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => groupBy.Contains((GroupBy)(null!)));
        Assert.Equal("groupByItem", exception.ParamName);
    }

    [Fact]
    public void Append_WithGroupByBase_AppendsItemWithoutMutatingOriginal()
    {
        GroupBys original = GroupBys.Empty;
        GroupBy<Address> item = new("Street");

        GroupBys appended = original.Append(item);

        Assert.NotSame(original, appended);
        Assert.True(original.IsEmpty());
        Assert.Empty(original.TableTags);
        Assert.False(appended.IsEmpty());
        Assert.Single(appended.TableTags);
        Assert.Equal("GROUP BY \"Address\".\"Street\"", appended.ToSql(Dialect));
    }

    [Fact]
    public void Append_WithGroupByBaseNull_ThrowsArgumentNullException()
    {
        GroupBys groupBy = GroupBys.Empty;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => groupBy.Append(null!));
        Assert.Equal("groupByItem", exception.ParamName);
    }

    [Fact]
    public void Append_WithPropertyName_AppendsExpectedItemWithoutMutatingOriginal()
    {
        GroupBys original = new(new GroupBy<Address>("City"));
        PropertyName propertyName = new("Street");

        GroupBys appended = original.Append<Address>(propertyName);

        Assert.Equal("GROUP BY \"Address\".\"City\"", original.ToSql(Dialect));
        Assert.Equal("GROUP BY \"Address\".\"City\", \"Address\".\"Street\"", appended.ToSql(Dialect));
    }

    [Fact]
    public void Append_WithStringPropertyName_AppendsExpectedItemWithoutMutatingOriginal()
    {
        GroupBys original = new(new GroupBy<Address>("City"));

        GroupBys appended = original.Append<Address>("Street");

        Assert.Equal("GROUP BY \"Address\".\"City\"", original.ToSql(Dialect));
        Assert.Equal("GROUP BY \"Address\".\"City\", \"Address\".\"Street\"", appended.ToSql(Dialect));
    }

    [Fact]
    public void Append_WithInvalidPropertyName_ThrowsInvalidPropertyException()
    {
        GroupBys groupBy = GroupBys.Empty;

        Assert.Throws<InvalidPropertyException<Address>>(() => groupBy.Append<Address>("LiveLongAndProsper"));
    }

    [Fact]
    public void Concat_AppendsItemsInGroupWithoutMutatingOriginal()
    {
        GroupBy<Address> initial = new("City");
        GroupBy<Address> street = new("Street");
        GroupBy<ColumnTable> columnTable = new("D000destruct0");
        GroupBys original = new(initial);
        GroupBy[] additionalItems = [street, columnTable];

        GroupBys appended = original.Concat(additionalItems);

        List<GroupBy> originalItems = [.. original.AsEnumerable()];
        List<GroupBy> appendedItems = [.. appended.AsEnumerable()];
        List<GroupBy> expectedOriginalItems = [initial];
        List<GroupBy> expectedAppendedItems = [initial, street, columnTable];
        Assert.Equal(expectedOriginalItems, originalItems);
        Assert.Equal(expectedAppendedItems, appendedItems);
        Assert.Equal("GROUP BY \"Address\".\"City\"", original.ToSql(Dialect));
        Assert.Equal("GROUP BY \"Address\".\"City\", \"Address\".\"Street\", \"ColumnTable\".\"D000destruct0\"", appended.ToSql(Dialect));
    }

    [Fact]
    public void Concat_WithEmptyCollection_ReturnsEquivalentGroupBy()
    {
        GroupBys original = new(new GroupBy<Address>("City"));
        GroupBy[] additionalItems = [];

        GroupBys appended = original.Concat(additionalItems);

        Assert.NotSame(original, appended);
        Assert.Equal(original.ToSql(Dialect), appended.ToSql(Dialect));
    }

    [Fact]
    public void Concat_WithNullCollection_ThrowsArgumentNullException()
    {
        GroupBys groupBy = GroupBys.Empty;
        IEnumerable<GroupBy>? groupByItems = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => groupBy.Concat(groupByItems!));
        Assert.Equal("groupByItems", exception.ParamName);
    }

    [Fact]
    public void Contains_ReturnsTrue_ForEquivalentItem()
    {
        GroupBys groupBy = new(new GroupBy<Address>("Street"));
        GroupBy equivalentItem = new GroupBy<Address>("Street");

        Assert.True(groupBy.Contains(equivalentItem));
    }

    [Fact]
    public void ContainsEquivalent_ExactExpressionGroup_ReturnsTrue()
    {
        Add selectedExpression = new(new Column<ColumnTable>(nameof(ColumnTable.ColA)), new Column<ColumnTable>(nameof(ColumnTable.ColB)));
        GroupBys groupBys = new(new GroupBy(selectedExpression));
        SelectTag selectTag = SelectTagGenerator.Get(selectedExpression, new AliasName("Value"));

        Assert.True(groupBys.ContainsEquivalent(selectTag));
    }

    [Fact]
    public void ContainsEquivalent_IndependentlyGroupedColumns_ReturnsTrue()
    {
        Add selectedExpression = new(new Column<ColumnTable>(nameof(ColumnTable.ColA)), new Column<ColumnTable>(nameof(ColumnTable.ColB)));
        GroupBys groupBys = new(new GroupBy<ColumnTable>(nameof(ColumnTable.ColA)), new GroupBy<ColumnTable>(nameof(ColumnTable.ColB)));
        SelectTag selectTag = SelectTagGenerator.Get(selectedExpression, new AliasName("Value"));

        Assert.True(groupBys.ContainsEquivalent(selectTag));
    }

    [Fact]
    public void ContainsEquivalent_ColumnsOnlyParticipateInsideOtherGroupExpressions_ReturnsFalse()
    {
        Column<ColumnTable> a = new(nameof(ColumnTable.ColA));
        Column<ColumnTable> b = new(nameof(ColumnTable.ColB));
        Column<ColumnTable> c = new(nameof(ColumnTable.Col1));
        Add selectedExpression = new(a, b);
        GroupBys groupBys = new(new GroupBy(new Add(a, c)), new GroupBy(new Add(b, c)));
        SelectTag selectTag = SelectTagGenerator.Get(selectedExpression, new AliasName("Value"));

        Assert.False(groupBys.ContainsEquivalent(selectTag));
    }

    [Fact]
    public void ContainsEquivalent_GroupedSubexpressionWithAggregate_ReturnsTrue()
    {
        Add groupedExpression = new(new Column<ColumnTable>(nameof(ColumnTable.ColA)), new Column<ColumnTable>(nameof(ColumnTable.ColB)));
        Add selectedExpression = new(groupedExpression, new Sum(new Column<ColumnTable>(nameof(ColumnTable.Col1))));
        GroupBys groupBys = new(new GroupBy(groupedExpression));
        SelectTag selectTag = SelectTagGenerator.Get(selectedExpression, new AliasName("Value"));

        Assert.True(groupBys.ContainsEquivalent(selectTag));
    }

    [Fact]
    public void ContainsEquivalent_UngroupedSubexpressionWithAggregate_ReturnsFalse()
    {
        Add groupedExpression = new(new Column<ColumnTable>(nameof(ColumnTable.ColA)), new Column<ColumnTable>(nameof(ColumnTable.ColB)));
        Add selectedExpression = new(groupedExpression, new Sum(new Column<ColumnTable>(nameof(ColumnTable.Col1))));
        GroupBys groupBys = new(new GroupBy<ColumnTable>(nameof(ColumnTable.ColA)));
        SelectTag selectTag = SelectTagGenerator.Get(selectedExpression, new AliasName("Value"));

        Assert.False(groupBys.ContainsEquivalent(selectTag));
    }

    [Fact]
    public void ContainsEquivalent_SelectedExpressionNestedInsideLargerGroupExpression_ReturnsFalse()
    {
        Add selectedExpression = new(new Column<ColumnTable>(nameof(ColumnTable.ColA)), new Column<ColumnTable>(nameof(ColumnTable.ColB)));
        Add groupedExpression = new(selectedExpression, new Column<ColumnTable>(nameof(ColumnTable.Col1)));
        GroupBys groupBys = new(new GroupBy(groupedExpression));
        SelectTag selectTag = SelectTagGenerator.Get(selectedExpression, new AliasName("Value"));

        Assert.False(groupBys.ContainsEquivalent(selectTag));
    }


    [Fact]
    public void Append_StringPropertyName_AppendsWithoutRecursion()
    {
        GroupBys groupBys = GroupBys.Empty.Append<ColumnTable>(nameof(ColumnTable.ColA));

        Assert.Single(groupBys.AsEnumerable());
    }

}
