using SqlTools.Exceptions;
using SqlTools.OrderByItems;
using SqlToolsTests.TestEntities;

namespace SqlToolsTests.OrderByTests;

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

        Assert.Equal(columnName, orderByItem.ColumnName);
        Assert.Equal(tableTag, orderByItem.TableTag);
        Assert.Equal(columnTag, orderByItem.ColumnTag);
        Assert.Equal(directionString, orderByItem.SortDirection.ToSql());
        Assert.Equal(sql, orderByItem.ToSql());
    }

    [Fact]
    public void OrderByItem_Constructor_ArgumentException()
    {
        Assert.Throws<SqlIdentifierException>(() => new OrderByItem<Address>("LiveLongAndProsper", SortDirectionEnum.Descending));
    }


    // IEquatable<IOrderByItem>.Equals against null
    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        IOrderByItem item = new OrderByItem<Address>("Street");
        Assert.False(item.Equals(null));
    }

    // Same TableTag + ColumnTag but different SortDirection → still equal
    [Fact]
    public void Equals_SameTableAndColumn_IgnoresSortDirection()
    {
        IOrderByItem asc = new OrderByItem<Address>("City", SortDirectionEnum.Ascending);
        IOrderByItem desc = new OrderByItem<Address>("City", SortDirectionEnum.Descending);

        Assert.True(asc.Equals(desc));
        Assert.True(desc.Equals(asc));
  
        Assert.True(asc.Equals((object)desc));
    }

    //  Different ColumnName → not equal
    [Fact]
    public void Equals_DifferentColumn_ReturnsFalse()
    {
        IOrderByItem street = new OrderByItem<Address>("Street");
        IOrderByItem city = new OrderByItem<Address>("City");

        Assert.False(street.Equals(city));
        Assert.False(city.Equals(street));
    }

    // Different T (hence different TableTag) → not equal
    [Fact]
    public void Equals_DifferentEntityType_ReturnsFalse()
    {
        IOrderByItem addressItem = new OrderByItem<Address>("Street");

        OrderByItem<Person> personItem = new("Name");

        Assert.False(addressItem.Equals(personItem));
        Assert.False(personItem.Equals(addressItem));
    }

    // List<T>.Contains uses IEquatable<IOrderByItem>
    [Fact]
    public void ListContains_FindsEquivalentItem()
    {
        List<IOrderByItem> list = 
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
        Dictionary<IOrderByItem, string> dictionary = [];
        OrderByItem<Address> key1 = new ("Street");
        dictionary[key1] = "hello";

        OrderByItem<Address> key2 = new ("Street");
        Assert.True(dictionary.ContainsKey(key2));
        Assert.Equal("hello", dictionary[key2]);
    }
}

#pragma warning restore CA1859 // Use concrete types when possible for improved performance
#pragma warning restore IDE0079 // Remove unnecessary suppression
