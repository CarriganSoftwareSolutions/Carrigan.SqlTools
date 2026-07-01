using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GroupByTests;

public class GroupBysTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

    [Fact]
    public void Empty_ReturnsEmptyGroupBy()
    {
        GroupBys groupBy = GroupBys.Empty;

        Assert.True(groupBy.IsEmpty());
        Assert.Empty(groupBy.TableTags);
        Assert.Empty(groupBy.AsEnumerable());
        Assert.Equal(string.Empty, groupBy.ToSql(Dialect));
    }

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
        GroupByBase[] groupByItems = [];

        GroupBys groupBy = new(groupByItems);

        Assert.True(groupBy.IsEmpty());
        Assert.Equal(string.Empty, groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithNullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<GroupByBase>? groupByItems = null;

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
        Assert.Equal("GROUP BY [Address].[City]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void New_WithPropertyName_CreatesExpectedSql()
    {
        PropertyName propertyName = new("City");

        GroupBys groupBy = GroupBys.New<Address>(propertyName);

        List<GroupByBase> groupByItems = [.. groupBy.AsEnumerable()];

        Assert.False(groupBy.IsEmpty());
        Assert.Single(groupByItems);
        Assert.Single(groupBy.TableTags);
        Assert.Equal("GROUP BY [Address].[City]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void New_WithStringPropertyName_CreatesExpectedSql()
    {
        GroupBys groupBy = GroupBys.New<Address>("City");

        List<GroupByBase> groupByItems = [.. groupBy.AsEnumerable()];

        Assert.False(groupBy.IsEmpty());
        Assert.Single(groupByItems);
        Assert.Single(groupBy.TableTags);
        Assert.Equal("GROUP BY [Address].[City]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void New_WithPropertyName_ReturnsEquivalentResultToConstructor()
    {
        PropertyName propertyName = new("Street");

        GroupBys expected = new(new GroupBy<Address>(propertyName));
        GroupBys actual = GroupBys.New<Address>(propertyName);

        Assert.Equal(expected.ToSql(Dialect), actual.ToSql(Dialect));
    }

    [Fact]
    public void New_WithStringPropertyName_ReturnsEquivalentResultToPropertyNameOverload()
    {
        PropertyName propertyName = new("Street");

        GroupBys expected = GroupBys.New<Address>(propertyName);
        GroupBys actual = GroupBys.New<Address>("Street");

        Assert.Equal(expected.ToSql(Dialect), actual.ToSql(Dialect));
    }

    [Fact]
    public void New_WithInvalidPropertyName_ThrowsInvalidPropertyException() =>
        Assert.Throws<InvalidPropertyException<Address>>(() =>
            GroupBys.New<Address>("LiveLongAndProsper"));

    [Fact]
    public void Constructor_WithMultipleItems_CreatesExpectedSql()
    {
        GroupBy<Address> city = new("City");
        GroupBy<Address> street = new("Street");

        GroupBys groupBy = new(city, street);

        Assert.Equal(2, groupBy.TableTags.Count());
        Assert.Equal("GROUP BY [Address].[City], [Address].[Street]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithMultipleTables_CreatesExpectedSql()
    {
        GroupBy<Address> address = new("City");
        GroupBy<ColumnTable> columnTable = new("D000destruct0");
        GroupBy<BooleanColumnTable> booleanColumnTable = new("Id");

        GroupBys groupBy = new(address, columnTable, booleanColumnTable);

        Assert.Equal(3, groupBy.TableTags.Count());
        Assert.Equal("GROUP BY [Address].[City], [ColumnTable].[D000destruct0], [BooleanColumnTable].[Id]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void GroupByItemBasesAsEnumerable_ReturnsItemsInGroup()
    {
        GroupBy<Address> city = new("City");
        GroupBy<Address> street = new("Street");
        GroupBys groupBy = new(city, street);

        List<GroupByBase> groupByItems = [.. groupBy.AsEnumerable()];
        List<GroupByBase> expectedGroupByItems = [city, street];

        Assert.Equal(expectedGroupByItems, groupByItems);
    }

    [Fact]
    public void GroupByItemsAsEnumerable_ReturnsItemsInGroup()
    {
        GroupBy<Address> city = new("City");
        GroupBy<Address> street = new("Street");
        GroupBys groupBy = new(city, street);

        List<GroupByBase> groupByItems = [.. groupBy.AsEnumerable()];
        List<GroupByBase> expectedGroupByItems = [city, street];

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
    public void Contains_WithNullItem_ThrowsArgumentNullException_NullColumnBase()
    {
        GroupBys groupBy = new(new GroupBy<Address>("Street"));

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => groupBy.Contains((ColumnBase)(null!)));
        Assert.Equal("column", exception.ParamName);
    }

    [Fact]
    public void Contains_WithNullItem_ThrowsArgumentNullException_NullGroupByBase()
    {
        GroupBys groupBy = new(new GroupBy<Address>("Street"));

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => groupBy.Contains((GroupByBase)(null!)));
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
        Assert.Equal("GROUP BY [Address].[Street]", appended.ToSql(Dialect));
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

        Assert.Equal("GROUP BY [Address].[City]", original.ToSql(Dialect));
        Assert.Equal("GROUP BY [Address].[City], [Address].[Street]", appended.ToSql(Dialect));
    }

    [Fact]
    public void Append_WithStringPropertyName_AppendsExpectedItemWithoutMutatingOriginal()
    {
        GroupBys original = new(new GroupBy<Address>("City"));

        GroupBys appended = original.Append<Address>("Street");

        Assert.Equal("GROUP BY [Address].[City]", original.ToSql(Dialect));
        Assert.Equal("GROUP BY [Address].[City], [Address].[Street]", appended.ToSql(Dialect));
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
        GroupByBase[] additionalItems = [street, columnTable];

        GroupBys appended = original.Concat(additionalItems);

        List<GroupByBase> originalItems = [.. original.AsEnumerable()];
        List<GroupByBase> appendedItems = [.. appended.AsEnumerable()];
        List<GroupByBase> expectedOriginalItems = [initial];
        List<GroupByBase> expectedAppendedItems = [initial, street, columnTable];
        Assert.Equal(expectedOriginalItems, originalItems);
        Assert.Equal(expectedAppendedItems, appendedItems);
        Assert.Equal("GROUP BY [Address].[City]", original.ToSql(Dialect));
        Assert.Equal("GROUP BY [Address].[City], [Address].[Street], [ColumnTable].[D000destruct0]", appended.ToSql(Dialect));
    }

    [Fact]
    public void Concat_WithEmptyCollection_ReturnsEquivalentGroupBy()
    {
        GroupBys original = new(new GroupBy<Address>("City"));
        GroupByBase[] additionalItems = [];

        GroupBys appended = original.Concat(additionalItems);

        Assert.NotSame(original, appended);
        Assert.Equal(original.ToSql(Dialect), appended.ToSql(Dialect));
    }

    [Fact]
    public void Concat_WithNullCollection_ThrowsArgumentNullException()
    {
        GroupBys groupBy = GroupBys.Empty;
        IEnumerable<GroupByBase>? groupByItems = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => groupBy.Concat(groupByItems!));
        Assert.Equal("groupByItems", exception.ParamName);
    }

    [Fact]
    public void AsGroupBy_ReturnsSameInstance()
    {
        GroupBys groupBy = new(new GroupBy<Address>("City"));

        GroupBys actual = groupBy.AsGroupBy();

        Assert.Same(groupBy, actual);
    }
}
