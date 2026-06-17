using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.GroupByClause;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GroupByTests;

public class GroupByItemTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

    [Theory]
    [InlineData("Street", "[Address].[Street]")]
    [InlineData("City", "[Address].[City]")]
    public void Constructor_WithStringPropertyName_CreatesExpectedSql(string propertyName, string expectedSql)
    {
        GroupBy<Address> groupByItem = new(propertyName);

        Assert.Equal(expectedSql, groupByItem.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithPropertyName_CreatesExpectedSql()
    {
        PropertyName propertyName = new("City");

        GroupBy<Address> groupByItem = new(propertyName);

        Assert.Equal("[Address].[City]", groupByItem.ToSql(Dialect));
    }

    [Fact]
    public void Constructor() =>
        Assert.Equal("[Address].[City]", new GroupBy<Address>("City").ToSql(Dialect));

    [Fact]
    public void Constructor_WithInvalidPropertyName_ThrowsInvalidPropertyException() =>
        Assert.Throws<InvalidPropertyException<Address>>(() => new GroupBy<Address>("LiveLongAndProsper"));

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        GroupBy<Address> item = new("Street");

        Assert.False(item.Equals(null));
    }

    [Fact]
    public void Equals_ObjectWithDifferentType_ReturnsFalse()
    {
        GroupBy<Address> item = new("Street");

        Assert.False(item.Equals((object)"Street"));
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        GroupBy<Address> item = new("Street");

        Assert.True(item.Equals(item));
    }

    [Fact]
    public void Equals_DifferentColumn_ReturnsFalse()
    {
        GroupBy<Address> street = new("Street");
        GroupBy<Address> city = new("City");

        Assert.False(street.Equals(city));
        Assert.False(city.Equals(street));
    }

    [Fact]
    public void Equals_DifferentEntityType_ReturnsFalse()
    {
        GroupBy<Address> addressItem = new("Street");
        GroupBy<Person> personItem = new("Name");

        Assert.False(addressItem.Equals(personItem));
        Assert.False(personItem.Equals(addressItem));
    }

    [Fact]
    public void ListContains_FindsEquivalentItem()
    {
        List<GroupByBase> groupByItems =
        [
            new GroupBy<Address>("Street"),
            new GroupBy<Address>("City")
        ];

        GroupBy<Address> candidate = new("City");
        GroupBy<Address> missing = new("PostalCode");

        Assert.Contains(candidate, groupByItems);
        Assert.DoesNotContain(missing, groupByItems);
    }

    [Fact]
    public void DictionaryKey_EquivalentItem_WorksAsKey()
    {
        Dictionary<GroupByBase, string> dictionary = [];
        GroupBy<Address> key = new("Street");
        GroupBy<Address> equivalentKey = new("Street");

        dictionary[key] = "hello";

        Assert.True(dictionary.ContainsKey(equivalentKey));
        Assert.Equal("hello", dictionary[equivalentKey]);
    }
}
