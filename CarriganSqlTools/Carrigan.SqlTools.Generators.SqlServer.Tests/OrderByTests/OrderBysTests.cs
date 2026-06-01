using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.OrderByClause;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.OrderByTests;

public class OrderBysTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

    [Fact]
    public void Empty_ReturnsEmptyOrderBy()
    {
        OrderBys orderBy = OrderBys.Empty;

        Assert.True(orderBy.IsEmpty());
        Assert.Empty(orderBy.TableTags);
        Assert.Empty(orderBy.AsEnumerable());
        Assert.Equal(string.Empty, orderBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithNoItems_CreatesEmptyOrderBy()
    {
        OrderBys orderBy = new();

        Assert.True(orderBy.IsEmpty());
        Assert.Equal(string.Empty, orderBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithEmptyCollection_CreatesEmptyOrderBy()
    {
        OrderByBase[] orderByItems = [];

        OrderBys orderBy = new(orderByItems);

        Assert.True(orderBy.IsEmpty());
        Assert.Equal(string.Empty, orderBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithNullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<OrderByBase>? orderByItems = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new OrderBys(orderByItems!));
        Assert.Equal("orderByItems", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithSingleItem_CreatesExpectedSql()
    {
        OrderBy<Address> orderByItem = new("City", SortDirectionEnum.Ascending);

        OrderBys orderBy = new(orderByItem);

        Assert.False(orderBy.IsEmpty());
        Assert.Single(orderBy.TableTags);
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void New_WithPropertyName_CreatesExpectedSql()
    {
        PropertyName propertyName = new("City");

        OrderBys orderBy = OrderBys.New<Address>(propertyName, SortDirectionEnum.Descending);

        List<OrderByBase> orderByItems = [.. orderBy.AsEnumerable()];

        Assert.False(orderBy.IsEmpty());
        Assert.Single(orderByItems);
        Assert.Single(orderBy.TableTags);
        Assert.Equal("ORDER BY [Address].[City] DESC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void New_WithStringPropertyName_CreatesExpectedSql()
    {
        OrderBys orderBy = OrderBys.New<Address>("City", SortDirectionEnum.Ascending);

        List<OrderByBase> orderByItems = [.. orderBy.AsEnumerable()];

        Assert.False(orderBy.IsEmpty());
        Assert.Single(orderByItems);
        Assert.Single(orderBy.TableTags);
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void New_WithPropertyName_ReturnsEquivalentResultToConstructor()
    {
        PropertyName propertyName = new("Street");

        OrderBys expected = new(new OrderBy<Address>(propertyName, SortDirectionEnum.Descending));
        OrderBys actual = OrderBys.New<Address>(propertyName, SortDirectionEnum.Descending);

        Assert.Equal(expected.ToSql(Dialect), actual.ToSql(Dialect));
    }

    [Fact]
    public void New_WithStringPropertyName_ReturnsEquivalentResultToPropertyNameOverload()
    {
        PropertyName propertyName = new("Street");

        OrderBys expected = OrderBys.New<Address>(propertyName, SortDirectionEnum.Descending);
        OrderBys actual = OrderBys.New<Address>("Street", SortDirectionEnum.Descending);

        Assert.Equal(expected.ToSql(Dialect), actual.ToSql(Dialect));
    }

    [Fact]
    public void New_WithInvalidPropertyName_ThrowsInvalidPropertyException() =>
        Assert.Throws<InvalidPropertyException<Address>>(() =>
            OrderBys.New<Address>("LiveLongAndProsper", SortDirectionEnum.Ascending));

    [Fact]
    public void Constructor_WithMultipleItems_CreatesExpectedSql()
    {
        OrderBy<Address> city = new("City", SortDirectionEnum.Ascending);
        OrderBy<Address> street = new("Street", SortDirectionEnum.Descending);

        OrderBys orderBy = new(city, street);

        Assert.Equal(2, orderBy.TableTags.Count());
        Assert.Equal("ORDER BY [Address].[City] ASC, [Address].[Street] DESC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithMultipleTables_CreatesExpectedSql()
    {
        OrderBy<Address> address = new("City", SortDirectionEnum.Ascending);
        OrderBy<ColumnTable> columnTable = new("D000destruct0", SortDirectionEnum.Descending);
        OrderBy<BooleanColumnTable> booleanColumnTable = new("Id", SortDirectionEnum.Ascending);

        OrderBys orderBy = new(address, columnTable, booleanColumnTable);

        Assert.Equal(3, orderBy.TableTags.Count());
        Assert.Equal("ORDER BY [Address].[City] ASC, [ColumnTable].[D000destruct0] DESC, [BooleanColumnTable].[Id] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void OrderByItemBasesAsEnumerable_ReturnsItemsInOrder()
    {
        OrderBy<Address> city = new("City");
        OrderBy<Address> street = new("Street", SortDirectionEnum.Descending);
        OrderBys orderBy = new(city, street);

        List<OrderByBase> orderByItems = [.. orderBy.AsEnumerable()];
        List<OrderByBase> expectedOrderByItems = [city, street];

        Assert.Equal(expectedOrderByItems, orderByItems);
    }

    [Fact]
    public void OrderByItemsAsEnumerable_ReturnsItemsInOrder()
    {
        OrderBy<Address> city = new("City");
        OrderBy<Address> street = new("Street", SortDirectionEnum.Descending);
        OrderBys orderBy = new(city, street);

        List<OrderByBase> orderByItems = [.. orderBy.AsEnumerable()];
        List<OrderByBase> expectedOrderByItems = [city, street];

        Assert.Equal(expectedOrderByItems, orderByItems);
    }

    [Fact]
    public void Contains_ReturnsTrue_ForExistingItemReference()
    {
        OrderBy<Address> item = new("City");
        OrderBys orderBy = new(item);

        Assert.True(orderBy.Contains(item));
    }

    [Fact]
    public void Contains_ReturnsTrue_ForEquivalentItemWithDifferentSortDirection()
    {
        OrderBy<Address> original = new("City", SortDirectionEnum.Descending);
        OrderBy<Address> lookup = new("City", SortDirectionEnum.Ascending);
        OrderBys orderBy = new(original);

        Assert.True(orderBy.Contains(lookup));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        OrderBy<Address> street = new("Street");
        OrderBy<Address> city = new("City");
        OrderBy<ColumnTable> columnTable = new("D000destruct0");
        OrderBys orderBy = new(street);

        Assert.False(orderBy.Contains(city));
        Assert.False(orderBy.Contains(columnTable));
    }

    [Fact]
    public void Contains_WithNullItem_ThrowsArgumentNullException()
    {
        OrderBys orderBy = new(new OrderBy<Address>("Street"));

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => orderBy.Contains(null!));
        Assert.Equal("orderByItem", exception.ParamName);
    }

    [Fact]
    public void Append_WithOrderByBase_AppendsItemWithoutMutatingOriginal()
    {
        OrderBys original = OrderBys.Empty;
        OrderBy<Address> item = new("Street");

        OrderBys appended = original.Append(item);

        Assert.NotSame(original, appended);
        Assert.True(original.IsEmpty());
        Assert.Empty(original.TableTags);
        Assert.False(appended.IsEmpty());
        Assert.Single(appended.TableTags);
        Assert.Equal("ORDER BY [Address].[Street] ASC", appended.ToSql(Dialect));
    }

    [Fact]
    public void Append_WithOrderByBaseNull_ThrowsArgumentNullException()
    {
        OrderBys orderBy = OrderBys.Empty;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => orderBy.Append(null!));
        Assert.Equal("orderByItem", exception.ParamName);
    }

    [Fact]
    public void Append_WithPropertyName_AppendsExpectedItemWithoutMutatingOriginal()
    {
        OrderBys original = new(new OrderBy<Address>("City"));
        PropertyName propertyName = new("Street");

        OrderBys appended = original.Append<Address>(propertyName, SortDirectionEnum.Descending);

        Assert.Equal("ORDER BY [Address].[City] ASC", original.ToSql(Dialect));
        Assert.Equal("ORDER BY [Address].[City] ASC, [Address].[Street] DESC", appended.ToSql(Dialect));
    }

    [Fact]
    public void Append_WithStringPropertyName_AppendsExpectedItemWithoutMutatingOriginal()
    {
        OrderBys original = new(new OrderBy<Address>("City"));

        OrderBys appended = original.Append<Address>("Street", SortDirectionEnum.Descending);

        Assert.Equal("ORDER BY [Address].[City] ASC", original.ToSql(Dialect));
        Assert.Equal("ORDER BY [Address].[City] ASC, [Address].[Street] DESC", appended.ToSql(Dialect));
    }

    [Fact]
    public void Append_WithInvalidPropertyName_ThrowsInvalidPropertyException()
    {
        OrderBys orderBy = OrderBys.Empty;

        Assert.Throws<InvalidPropertyException<Address>>(() => orderBy.Append<Address>("LiveLongAndProsper"));
    }

    [Fact]
    public void Concat_AppendsItemsInOrderWithoutMutatingOriginal()
    {
        OrderBy<Address> initial = new("City", SortDirectionEnum.Ascending);
        OrderBy<Address> street = new("Street", SortDirectionEnum.Descending);
        OrderBy<ColumnTable> columnTable = new("D000destruct0", SortDirectionEnum.Ascending);
        OrderBys original = new(initial);
        OrderByBase[] additionalItems = [street, columnTable];

        OrderBys appended = original.Concat(additionalItems);

        List<OrderByBase> originalItems = [.. original.AsEnumerable()];
        List<OrderByBase> appendedItems = [.. appended.AsEnumerable()];
        List<OrderByBase> expectedOriginalItems = [initial];
        List<OrderByBase> expectedAppendedItems = [initial, street, columnTable];
        Assert.Equal(expectedOriginalItems, originalItems);
        Assert.Equal(expectedAppendedItems, appendedItems);
        Assert.Equal("ORDER BY [Address].[City] ASC", original.ToSql(Dialect));
        Assert.Equal("ORDER BY [Address].[City] ASC, [Address].[Street] DESC, [ColumnTable].[D000destruct0] ASC", appended.ToSql(Dialect));
    }

    [Fact]
    public void Concat_WithEmptyCollection_ReturnsEquivalentOrderBy()
    {
        OrderBys original = new(new OrderBy<Address>("City"));
        OrderByBase[] additionalItems = [];

        OrderBys appended = original.Concat(additionalItems);

        Assert.NotSame(original, appended);
        Assert.Equal(original.ToSql(Dialect), appended.ToSql(Dialect));
    }

    [Fact]
    public void Concat_WithNullCollection_ThrowsArgumentNullException()
    {
        OrderBys orderBy = OrderBys.Empty;
        IEnumerable<OrderByBase>? orderByItems = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => orderBy.Concat(orderByItems!));
        Assert.Equal("orderByItems", exception.ParamName);
    }

    [Fact]
    public void AsOrderBy_ReturnsSameInstance()
    {
        OrderBys orderBy = new(new OrderBy<Address>("City"));

        OrderBys actual = orderBy.AsOrderBy();

        Assert.Same(orderBy, actual);
    }
}
