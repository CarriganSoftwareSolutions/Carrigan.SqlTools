using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
public class OrderByItemTests
{
    [Theory]
    [InlineData("Street", SortDirectionEnum.Ascending, "[Address]", "[Address].[Street]", "ASC", "[Address].[Street] ASC")]
    [InlineData("City", SortDirectionEnum.Descending, "[Address]", "[Address].[City]", "DESC", "[Address].[City] DESC")]
    public void OrderByItem_Constructor(string columnName, SortDirectionEnum direction, string tableTag, string columnTag, string directionString, string sql)
    {
        OrderByItem<Address> orderByItem = new(columnName, direction);

        Assert.Equal(tableTag, orderByItem.TableTag);
        Assert.Equal(columnTag, orderByItem.ColumnInfo);
        Assert.Equal(directionString, orderByItem.SortDirection.ToSql());
        Assert.Equal(sql, orderByItem.ToSql());
    }

    [Fact]
    public void OrderByItem_Constructor_ArgumentException() => 
        Assert.Throws<InvalidPropertyException<Address>>(() => new OrderByItem<Address>("LiveLongAndProsper", SortDirectionEnum.Descending));


    // IEquatable<IOrderByItem>.Equals against null
    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        OrderByItemBase item = new OrderByItem<Address>("Street");
        Assert.False(item.Equals(null));
    }

    // Same TableTag + ColumnTag but different SortDirection → still equal
    [Fact]
    public void Equals_SameTableAndColumn_IgnoresSortDirection()
    {
        OrderByItemBase ascending = new OrderByItem<Address>("City", SortDirectionEnum.Ascending);
        OrderByItemBase descending = new OrderByItem<Address>("City", SortDirectionEnum.Descending);

        Assert.True(ascending.Equals(descending));
        Assert.True(descending.Equals(ascending));
  
        Assert.True(ascending.Equals((object)descending));
    }

    //  Different ColumnName → not equal
    [Fact]
    public void Equals_DifferentColumn_ReturnsFalse()
    {
        OrderByItemBase street = new OrderByItem<Address>("Street");
        OrderByItemBase city = new OrderByItem<Address>("City");

        Assert.False(street.Equals(city));
        Assert.False(city.Equals(street));
    }

    // Different T (hence different TableTag) → not equal
    [Fact]
    public void Equals_DifferentEntityType_ReturnsFalse()
    {
        OrderByItemBase addressItem = new OrderByItem<Address>("Street");

        OrderByItem<Person> personItem = new("Name");

        Assert.False(addressItem.Equals(personItem));
        Assert.False(personItem.Equals(addressItem));
    }

    // List<T>.Contains uses IEquatable<IOrderByItem>
    [Fact]
    public void ListContains_FindsEquivalentItem()
    {
        List<OrderByItemBase> list = 
            [
                new OrderByItem<Address>("Street"),
                new OrderByItem<Address>("City")
            ];

        OrderByItem<Address> candidate = new ("City");
        Assert.Contains(candidate, list);

        OrderByItem<Address> missing = new ("PostalCode");
        Assert.DoesNotContain(missing, list);
    }

    // Dictionary<IOrderByItem, …> uses Equals + GetHashCode
    [Fact]
    public void DictionaryKey_EquivalentItem_WorksAsKey()
    {
        Dictionary<OrderByItemBase, string> dictionary = [];
        OrderByItem<Address> key1 = new ("Street");
        dictionary[key1] = "hello";

        OrderByItem<Address> key2 = new ("Street");
        Assert.True(dictionary.ContainsKey(key2));
        Assert.Equal("hello", dictionary[key2]);
    }
    [Fact]
    public void WithAppend()
    {
        OrderByItem<Address> streetSort = new("Street");
        OrderByItem<Address> citySort = new("City");

        OrderBy returned = streetSort.WithAppend(citySort);

        List<OrderByItemBase> twoItems =
            [.. returned.OrderByItemsAsEnumerable()];

        //immutable
        Assert.Equal(streetSort, new("Street"));
        Assert.Equal(citySort, new("City"));

        // not empty?
        Assert.False(returned.IsEmpty());
        Assert.Equal(2, twoItems.Count);

        // TableTags sequence
        List<TableTag> newTags =
            [.. returned.TableTags];
        Assert.Equal(2, newTags.Count);
        Assert.Equal("[Address]", newTags[0]);
        Assert.Equal("[Address]", newTags[1]);

        // ToSql should produce ORDER BY clause
        Assert.Equal("ORDER BY [Address].[Street] ASC, [Address].[City] ASC", returned.ToSql());
    }

    [Fact]
    public void With_Concat()
    {
        OrderByItem<Address> initial = new("City", SortDirectionEnum.Ascending);
        OrderByItem<Address> more1 = new("Street", SortDirectionEnum.Descending);
        OrderByItem<Address> more2 = new("City", SortDirectionEnum.Descending); // same column, different direction

        OrderBy order = new(initial);

        OrderBy orderWithThreeItems = initial.WithConcat(more1, more2);
        List<OrderByItemBase> oldItems =
            [.. order.OrderByItemsAsEnumerable()];

        List<OrderByItemBase> threeItems =
            [.. orderWithThreeItems.OrderByItemsAsEnumerable()];

        // immutable
        Assert.Equal(initial, new("City", SortDirectionEnum.Ascending));
        Assert.Equal(more1, new("Street", SortDirectionEnum.Descending));
        Assert.Equal(more2, new("City", SortDirectionEnum.Descending));


        // Should have three items now
        Assert.Equal(3, threeItems.Count);

        // Insertion order must be preserved
        Assert.Equal(initial.ToSql(), threeItems[0].ToSql());
        Assert.Equal(more1.ToSql(), threeItems[1].ToSql());
        Assert.Equal(more2.ToSql(), threeItems[2].ToSql());

        // TableTags sequence
        List<TableTag> newTags =
            [.. orderWithThreeItems.TableTags];
        Assert.Equal("[Address]", newTags[0]);
        Assert.Equal("[Address]", newTags[1]);
        Assert.Equal("[Address]", newTags[2]);

        // ToSql must join all three
        string expectedSql = string.Join(", ", oldItems.Select(i => i.ToSql()));
        Assert.Equal($"ORDER BY {expectedSql}", order.ToSql());

        // ToSql must join all three
        expectedSql = string.Join(", ", threeItems.Select(i => i.ToSql()));
        Assert.Equal($"ORDER BY {expectedSql}", orderWithThreeItems.ToSql());
    }

    [Fact]
    public void Contains_ReturnsTrue_ForExistingItemReference()
    {
        OrderByItem<Address> item = new("City");
        Assert.True(item.Contains(item));
    }

    [Fact]
    public void Contains_ReturnsTrue_ForDifferentInstanceWithSameTableAndColumn()
    {
        OrderByItem<Address> cityDescending = new("City", SortDirectionEnum.Descending);

        // Different instance but same TableTag + ColumnTag
        OrderByItem<Address> cityAscending = new("City", SortDirectionEnum.Ascending);

        Assert.True(cityDescending.Contains(cityAscending));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForDifferentColumnOrTable()
    {
        OrderByItem<Address> streetItem = new("Street");
        // Different column
        OrderByItem<Address> cityItem = new("City"
            
            );
        Assert.False(streetItem.Contains(cityItem));
    }

    [Fact]
    public void IsEmptyFalse()
    {
        OrderByItem<Address> streetItem = new("Street");
        Assert.False(streetItem.IsEmpty());
    }

    [Fact]
    public void Empty() =>
        //There is currently no way to test IsEmpty is true, as that should always generate an exception.
        Assert.Throws<InvalidPropertyException<Address>>(() => { OrderByItem<Address> streetItem = new(string.Empty); });

    [Fact]
    public void Contains_Null_ArgumentNullException()
    {
        OrderByItem<Address> item = new("City");
        Assert.Throws<ArgumentNullException>(() => item.Contains(null!));
    }

    [Fact]
    public void WithAppend_Null_ArgumentNullException()
    {
        OrderByItem<Address> item = new("City");
        Assert.Throws<ArgumentNullException>(() => item.WithAppend(null!));
    }

    [Fact]
    public void WithConcat_Null_ArgumentNullException()
    {
        OrderByItem<Address> item = new("City");
        Assert.Throws<ArgumentNullException>(() => item.WithConcat((IEnumerable<OrderByItemBase>)null!));
    }

}

#pragma warning restore CA1859 // Use concrete types when possible for improved performance
#pragma warning restore IDE0079 // Remove unnecessary suppression
