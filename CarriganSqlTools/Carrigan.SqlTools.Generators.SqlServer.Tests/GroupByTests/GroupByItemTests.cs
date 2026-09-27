using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

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
    public void Equals_SameReference()
    {
        GroupBy item = new GroupBy<Address>("Street");

        Assert.True(item.Equals(item));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(item == item);
        Assert.False(item != item);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        GroupBy left = new GroupBy<Address>("Street");
        GroupBy right = new GroupBy<Address>("Street");

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        GroupBy first = new GroupBy<Address>("Street");
        GroupBy second = new GroupBy<Address>("Street");
        GroupBy third = new GroupBy<Address>("Street");

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_DifferentColumn()
    {
        GroupBy street = new GroupBy<Address>("Street");
        GroupBy city = new GroupBy<Address>("City");

        Assert.False(street.Equals(city));
        Assert.False(city.Equals(street));
        Assert.NotEqual(street, city);
    }

    [Fact]
    public void Equals_DifferentEntityType()
    {
        GroupBy addressItem = new GroupBy<Address>("Street");
        GroupBy personItem = new GroupBy<Person>("Name");

        Assert.False(addressItem.Equals(personItem));
        Assert.False(personItem.Equals(addressItem));
        Assert.NotEqual(addressItem, personItem);
    }

    [Fact]
    public void Equals_UsesColumnTagCaseInsensitiveIdentity()
    {
        GroupBy left = CreateTestGroupBy("Schema", "Address", nameof(Address.Street));
        GroupBy right = CreateTestGroupBy("schema", "address", nameof(Address.Street));

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_NullAndEmptySchema()
    {
        GroupBy noSchema = CreateTestGroupBy(null, "Address", nameof(Address.Street));
        GroupBy emptySchema = CreateTestGroupBy(string.Empty, "Address", nameof(Address.Street));

        Assert.True(noSchema.Equals(emptySchema));
        Assert.True(emptySchema.Equals(noSchema));
        Assert.True(noSchema == emptySchema);
        Assert.False(noSchema != emptySchema);
        Assert.Equal(noSchema.GetHashCode(), emptySchema.GetHashCode());
    }

    [Fact]
    public void Equals_UsesStructuralIdentity_NotFormattedText()
    {
        GroupBy left = CreateTestGroupBy("A.B", "C", nameof(Address.Street));
        GroupBy right = CreateTestGroupBy("A", "B.C", nameof(Address.Street));

        Assert.Equal(left.ToString(), right.ToString());
        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_Null()
    {
        GroupBy item = new GroupBy<Address>("Street");
        GroupBy? other = null;

        Assert.False(item.Equals(other));
        Assert.False(item.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        GroupBy item = new GroupBy<Address>("Street");
        object other = new GroupBy<Address>("Street");

        Assert.True(item.Equals(other));
    }

    [Fact]
    public void Equals_ObjectWrongType()
    {
        GroupBy item = new GroupBy<Address>("Street");
        object other = "Address.Street";

        Assert.False(item.Equals(other));
    }

    [Fact]
    public void EqualOperator_EquivalentInstances()
    {
        GroupBy left = new GroupBy<Address>("Street");
        GroupBy right = new GroupBy<Address>("Street");

        Assert.True(left == right);
        Assert.True(right == left);
        Assert.False(left != right);
        Assert.False(right != left);
    }

    [Fact]
    public void EqualOperator_DifferentInstances()
    {
        GroupBy left = new GroupBy<Address>("Street");
        GroupBy right = new GroupBy<Address>("City");

        Assert.False(left == right);
        Assert.False(right == left);
        Assert.True(left != right);
        Assert.True(right != left);
    }

    [Fact]
    public void EqualOperator_Null()
    {
        GroupBy item = new GroupBy<Address>("Street");
        GroupBy? nullItem = null;
        bool flag;

        Assert.False(item == nullItem);
        Assert.False(nullItem == item);
        Assert.True(item != nullItem);
        Assert.True(nullItem != item);
        flag = nullItem == null;
        Assert.True(flag);
        flag = nullItem != null;
        Assert.False(flag);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances()
    {
        GroupBy left = new GroupBy<Address>("Street");
        GroupBy right = new GroupBy<Address>("Street");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void ListContains_FindsEquivalentItem()
    {
        List<GroupBy> groupByItems =
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
        Dictionary<GroupBy, string> dictionary = [];
        GroupBy key = new GroupBy<Address>("Street");
        GroupBy equivalentKey = new GroupBy<Address>("Street");
        dictionary[key] = "hello";

        Assert.True(dictionary.ContainsKey(equivalentKey));
        Assert.Equal("hello", dictionary[equivalentKey]);
    }

    [Fact]
    public void DictionaryKey_CaseInsensitiveEquivalentItem_WorksAsKey()
    {
        Dictionary<GroupBy, string> dictionary = [];
        GroupBy key = CreateTestGroupBy("Schema", "Address", nameof(Address.Street));
        GroupBy equivalentKey = CreateTestGroupBy("schema", "address", nameof(Address.Street));
        dictionary[key] = "hello";

        Assert.True(dictionary.ContainsKey(equivalentKey));
        Assert.Equal("hello", dictionary[equivalentKey]);
    }

    [Fact]
    public void DictionaryKey_EquivalentAssignmentReplacesValue()
    {
        Dictionary<GroupBy, decimal> dictionary = [];
        GroupBy firstKey = new GroupBy<Address>("Street");
        GroupBy secondKey = new GroupBy<Address>("Street");
        dictionary[firstKey] = 3.14159m;
        dictionary[secondKey] = 2.71828m;

        Assert.Single(dictionary);
        Assert.Equal(2.71828m, dictionary[firstKey]);
        Assert.Equal(2.71828m, dictionary[secondKey]);
    }

    [Fact]
    public void DictionaryKey_DifferentItem_IsNotFound()
    {
        Dictionary<GroupBy, string> dictionary = [];
        GroupBy key = new GroupBy<Address>("Street");
        GroupBy differentKey = new GroupBy<Address>("City");
        dictionary[key] = "hello";

        Assert.False(dictionary.ContainsKey(differentKey));
    }

    [Fact]
    public void HashSet_EquivalentInstance()
    {
        GroupBy storedValue = new GroupBy<Address>("Street");
        GroupBy lookupValue = new GroupBy<Address>("Street");
        HashSet<GroupBy> set = [storedValue];

        Assert.Contains(lookupValue, set);
    }

    [Fact]
    public void HashSet_CaseInsensitiveEquivalentInstance()
    {
        GroupBy storedValue = CreateTestGroupBy("Schema", "Address", nameof(Address.Street));
        GroupBy lookupValue = CreateTestGroupBy("schema", "address", nameof(Address.Street));
        HashSet<GroupBy> set = [storedValue];

        Assert.Contains(lookupValue, set);
    }

    private static GroupBy CreateTestGroupBy(string? schemaName, string tableName, string propertyName)
    {
        PropertyInfo propertyInfo = typeof(Address).GetProperty(propertyName)!;
        ColumnInfo columnInfo = new(schemaName is null ? null : new SchemaName(schemaName), new TableName(tableName), propertyInfo, []);
        return new GroupBy(new ColumnTagExpression(columnInfo.ColumnTag));
    }
}
