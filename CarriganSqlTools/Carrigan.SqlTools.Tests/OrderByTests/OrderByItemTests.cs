using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Base.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class OrderByItemTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

    [Theory]
    [InlineData("Street", SortDirectionEnum.Ascending, "[Address].[Street] ASC")]
    [InlineData("City", SortDirectionEnum.Descending, "[Address].[City] DESC")]
    public void Constructor_WithStringPropertyName_CreatesExpectedSql(string propertyName, SortDirectionEnum sortDirection, string expectedSql)
    {
        OrderBy<Address> orderByItem = new(propertyName, sortDirection);

        Assert.Equal(expectedSql, orderByItem.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithPropertyName_CreatesExpectedSql()
    {
        PropertyName propertyName = new("City");

        OrderBy<Address> orderByItem = new(propertyName, SortDirectionEnum.Descending);

        Assert.Equal("[Address].[City] DESC", orderByItem.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithDefaultSortDirection_UsesAscending() =>
        Assert.Equal("[Address].[City] ASC", new OrderBy<Address>("City").ToSql(Dialect));

    [Fact]
    public void Constructor_WithInvalidPropertyName_ThrowsInvalidPropertyException() =>
        Assert.Throws<InvalidPropertyException<Address>>(() => new OrderBy<Address>("LiveLongAndProsper"));

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        OrderByBase item = new OrderBy<Address>("Street");

        Assert.False(item.Equals(null));
    }

    [Fact]
    public void Equals_ObjectWithDifferentType_ReturnsFalse()
    {
        OrderByBase item = new OrderBy<Address>("Street");

        Assert.False(item.Equals((object)"Street"));
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        OrderByBase item = new OrderBy<Address>("Street");

        Assert.True(item.Equals(item));
    }

    [Fact]
    public void Equals_SameTableAndColumn_IgnoresSortDirection()
    {
        OrderByBase ascending = new OrderBy<Address>("City", SortDirectionEnum.Ascending);
        OrderByBase descending = new OrderBy<Address>("City", SortDirectionEnum.Descending);

        Assert.True(ascending.Equals(descending));
        Assert.True(descending.Equals(ascending));
        Assert.True(ascending.Equals((object)descending));
        Assert.Equal(ascending.GetHashCode(), descending.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentColumn_ReturnsFalse()
    {
        OrderByBase street = new OrderBy<Address>("Street");
        OrderByBase city = new OrderBy<Address>("City");

        Assert.False(street.Equals(city));
        Assert.False(city.Equals(street));
    }

    [Fact]
    public void Equals_DifferentEntityType_ReturnsFalse()
    {
        OrderByBase addressItem = new OrderBy<Address>("Street");
        OrderByBase personItem = new OrderBy<Person>("Name");

        Assert.False(addressItem.Equals(personItem));
        Assert.False(personItem.Equals(addressItem));
    }

    [Fact]
    public void ListContains_FindsEquivalentItem()
    {
        List<OrderByBase> orderByItems =
        [
            new OrderBy<Address>("Street"),
            new OrderBy<Address>("City")
        ];

        OrderBy<Address> candidate = new("City");
        OrderBy<Address> missing = new("PostalCode");

        Assert.Contains(candidate, orderByItems);
        Assert.DoesNotContain(missing, orderByItems);
    }

    [Fact]
    public void DictionaryKey_EquivalentItem_WorksAsKey()
    {
        Dictionary<OrderByBase, string> dictionary = [];
        OrderBy<Address> key = new("Street");
        OrderBy<Address> equivalentKey = new("Street", SortDirectionEnum.Descending);

        dictionary[key] = "hello";

        Assert.True(dictionary.ContainsKey(equivalentKey));
        Assert.Equal("hello", dictionary[equivalentKey]);
    }
}
