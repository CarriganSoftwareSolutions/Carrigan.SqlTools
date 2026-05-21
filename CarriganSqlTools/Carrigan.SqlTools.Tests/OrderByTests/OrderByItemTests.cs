using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class OrderByItemTests
{
    private readonly static ISqlDialects Dialect = new SqlServerDialect();
    [Theory]
    [InlineData("Street", SortDirectionEnum.Ascending, "Address.Street", "[Address].[Street]", "ASC", "[Address].[Street] ASC")]
    [InlineData("City", SortDirectionEnum.Descending, "Address.City", "[Address].[City]", "DESC", "[Address].[City] DESC")]
    public void OrderByItem_Constructor(string columnName, SortDirectionEnum direction, string columnString, string columnTag, string directionString, string sql)
    {
        OrderByItem<Address> orderByItem = new(columnName, direction);

        Assert.Equal(columnString, orderByItem.ColumnInfo.ColumnTag);
        Assert.Equal(columnTag, orderByItem.ColumnInfo.ColumnTag.ToSql(Dialect));
        Assert.Equal(directionString, orderByItem.SortDirection.ToSql());
        Assert.Equal(sql, orderByItem.ToSql(Dialect));
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
}
