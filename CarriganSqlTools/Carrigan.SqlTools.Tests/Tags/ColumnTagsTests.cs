using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.Tags;

public class ColumnTagsTests
{
    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "Pizza")]
    [InlineData(null, "Sloppy", "Pizza", "Pizza")]
    [InlineData("", "Sloppy", "Pizza", "Pizza")]
    public void ColumnNameTest(string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tableTag = new(new SqlServerDialect(), schemaName, tableName);
        ColumnTag actual = new(tableTag, new ColumnName(columnName));

        Assert.Equal(expected, actual.ColumnName);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "[Franks].[Sloppy]")]
    [InlineData(null, "Sloppy", "Pizza", "[Sloppy]")]
    [InlineData("", "Sloppy", "Pizza", "[Sloppy]")]
    public void ColumnTagTable(string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tableTag = new(new SqlServerDialect(), schemaName, tableName);
        ColumnTag actual = new (tableTag, new ColumnName(columnName));

        Assert.Equal(expected, actual.TableTag);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "[Franks].[Sloppy].[Pizza]")]
    [InlineData(null, "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    [InlineData("", "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    public void Col_Tag_Tests_3_Params(string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tableTag = new(new SqlServerDialect(), schemaName, tableName);
        string actual = new ColumnTag(tableTag, new ColumnName(columnName));

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "[Franks].[Sloppy].[Pizza]")]
    [InlineData(null, "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    [InlineData("", "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    public void Col_Tag_Tests_3_Params_ExplicitToString(string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tableTag = new(new SqlServerDialect(), schemaName, tableName);
        string actual = (new ColumnTag(tableTag, new ColumnName(columnName))).ToString();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "[Franks].[Sloppy].[Pizza]")]
    [InlineData(null, "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    [InlineData("", "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    public void Col_Tag_Tests_3_Params_ExplicitToString_UseTable (string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tableTag = new(new SqlServerDialect(), schemaName, tableName);
        string actual = (new ColumnTag(tableTag, new ColumnName(columnName))).ToString(true);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "[Pizza]")]
    [InlineData(null, "Sloppy", "Pizza", "[Pizza]")]
    [InlineData("", "Sloppy", "Pizza", "[Pizza]")]
    public void Col_Tag_Tests_3_Params_ExplicitToString_DoNotUseTable(string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tableTag = new(new SqlServerDialect(), schemaName, tableName);
        string actual = (new ColumnTag(tableTag, new ColumnName(columnName))).ToString(false);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, "Sloppy", "")]
    [InlineData(null, "", "")]
    [InlineData(null, null, "")]
    [InlineData(null, "Sloppy", null)]
    [InlineData(null, "", null)]
    [InlineData(null, null, null)]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_Schema_Null(string? schemaName, string? tableName, string? columnName)
        => _ = new ColumnTag(new TableTag(new SqlServerDialect(), schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("", "Sloppy", "")]
    [InlineData("", "", "")]
    [InlineData("", null, "")]
    [InlineData("", "Sloppy", null)]
    [InlineData("", "", null)]
    [InlineData("", null, null)]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_Schema_Empty(string? schemaName, string? tableName, string? columnName)
        => _ = new ColumnTag(new TableTag(new SqlServerDialect(), schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("", null, "")]
    [InlineData(null, null, "")]
    [InlineData("Franks", null, null)]
    [InlineData("", null, null)]
    [InlineData(null, null, null)]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_3_Table_Null(string? schemaName, string? tableName, string? columnName)
        => _ = new ColumnTag(new TableTag(new SqlServerDialect(), schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("", "", "")]
    [InlineData(null, "", "")]
    [InlineData("", "", null)]
    [InlineData(null, "", null)]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_3_Table_Empty(string? schemaName, string? tableName, string? columnName)
        => _ = new ColumnTag(new TableTag(new SqlServerDialect(), schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("Franks", "Sloppy", null)]
    [InlineData(null, "Sloppy", null)]
    [InlineData("", "Sloppy", null)]
    [InlineData("Franks", null, null)]
    [InlineData("", "", null)]
    [InlineData("", null, null)]
    [InlineData(null, "", null)]
    [InlineData(null, null, null)]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_3_Column_Null(string? schemaName, string? tableName, string? columnName)
        => _ = new ColumnTag(new TableTag(new SqlServerDialect(), schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("Franks", "Sloppy", "")]
    [InlineData(null, "Sloppy", "")]
    [InlineData("", "Sloppy", "")]
    [InlineData("Franks", null, "")]
    [InlineData("", "", "")]
    [InlineData("", null, "")]
    [InlineData(null, "", "")]
    [InlineData(null, null, "")]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_3_Params_Column_Empty(string? schemaName, string? tableName, string? columnName)  =>
         _ = new ColumnTag(new TableTag(new SqlServerDialect(), schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "[Franks].[Sloppy].[Pizza]")]
    [InlineData(null, "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    [InlineData("", "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    public void Col_Tag_Tests_2_Params(string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tg = new(new SqlServerDialect(), schemaName, tableName);
        string actual = new ColumnTag(tg, new ColumnName(columnName));

        Assert.Equal(expected, actual);
    }



    [Theory]
    [InlineData("Franks", "Sloppy", "")]
    [InlineData(null, "Sloppy", "")]
    [InlineData("", "Sloppy", "")]
    [InlineData("Franks", "Sloppy", null)]
    [InlineData(null, "Sloppy", null)]
    [InlineData("", "Sloppy", null)]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_2_Params_no_longer_throws_exception(string? schemaName, string tableName, string? columnName)
    {
        TableTag tg = new(new SqlServerDialect(), schemaName, tableName);

        _ = new ColumnTag(tg, new ColumnName(columnName!));
    }


    //implicit operator → string and ToString()
    [Theory]
    [InlineData("S", "T", "C", "[S].[T].[C]")]
    [InlineData(null, "T", "C", "[T].[C]")]
    [InlineData("", "T", "C", "[T].[C]")]
    public void ImplicitStringAndToString_AreEquivalent(string? schema, string table, string column, string expected)
    {
        TableTag tableTag = new(new SqlServerDialect(), schema, table);
        ColumnTag colTag = new(tableTag, new ColumnName(column));

        // implicit cast
        string viaImplicit = colTag;
        Assert.Equal(expected, viaImplicit);

        // ToString()
        Assert.Equal(expected, colTag.ToString());
    }

    //IEquatable<ColumnTag>.Equals()
    [Fact]
    public void Equals_SameUnderlyingTag_ReturnsTrue()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag a = new(tableTag, new ColumnName("C"));
        ColumnTag b = new(tableTag, new ColumnName("C"));

        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
    }

    [Fact]
    public void Equals_DifferentUnderlyingTag_ReturnsFalse()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag a = new(tableTag, new ColumnName("C1"));
        ColumnTag b = new(tableTag, new ColumnName("C2"));

        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));
    }

    // == and != operators
    [Fact]
    public void EqualityOperator_WorksLikeEquals()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag x = new(tableTag, new ColumnName("C"));
        ColumnTag y = new(tableTag, new ColumnName("C"));
        ColumnTag z = new(tableTag, new ColumnName("Different"));

        Assert.True(x == y);
        Assert.False(x == z);
        Assert.True(x != z);
    }

    // GetHashCode consistency
    [Fact]
    public void GetHashCode_EqualInstances_HaveSameHash()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag a = new(tableTag, new ColumnName("C"));
        ColumnTag b = new(tableTag, new ColumnName("C"));

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // IEqualityComparer<ColumnTag>
    [Fact]
    public void Comparer_EqualsAndHashCode_ViaIEqualityComparer()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag a = new(tableTag, new ColumnName("C"));
        ColumnTag b = new(tableTag, new ColumnName("C"));
        ColumnTag c = new(tableTag, new ColumnName("Other"));
        ColumnTag comparer = new(tableTag, new ColumnName("D"));

        Assert.True(comparer.Equals(a, b));
        Assert.False(comparer.Equals(a, c));
        Assert.Equal(a.GetHashCode(), comparer.GetHashCode(b));
    }

    // IComparable<ColumnTag>.CompareTo()
    [Fact]
    public void CompareTo_SortsByUnderlyingStringOrdinal()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag lower = new(tableTag, new ColumnName("A"));
        ColumnTag higher = new(tableTag, new ColumnName("B"));

        Assert.True(lower.CompareTo(higher) < 0);
        Assert.True(higher.CompareTo(lower) > 0);
        Assert.Equal(0, lower.CompareTo(new ColumnTag(tableTag, new ColumnName("A"))));
    }

    // Sorting a list of ColumnTags
    [Fact]
    public void Sort_ListOfColumnTags_OrdersLexicographically()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag charlie = new(tableTag, new ColumnName("Charlie"));
        ColumnTag bravo = new(tableTag, new ColumnName("Bravo"));
        ColumnTag alpha = new(tableTag, new ColumnName("Alpha"));
            
        List<ColumnTag> tags =
            [
                charlie,
                bravo,
                alpha
            ];

        tags.Sort();

        Assert.Equal(alpha, tags[0]);
        Assert.Equal(bravo, tags[1]);
        Assert.Equal(charlie, tags[2]);
    }

    // Using ColumnTag as dictionary key
    [Fact]
    public void DictionaryKey_RetrievalByEquivalentColumnTag_Works()
    {
        Dictionary<ColumnTag, string> dict = [];
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag key1 = new(tableTag, new ColumnName("C"));
        dict[key1] = "value";


        ColumnTag key2 = new(tableTag, new ColumnName("C"));
        Assert.True(dict.ContainsKey(key2));
        Assert.Equal("value", dict[key2]);
    }

    //  Null comparisons
    [Fact]
    public void CompareTo_Null_IsGreaterThanNull()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag columnTag = new(tableTag, new ColumnName("C"));
        Assert.True(columnTag.CompareTo(null) > 0);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        TableTag tableTag = new(new SqlServerDialect(), "S", "T");
        ColumnTag columnTag = new(tableTag, new ColumnName("C"));
        Assert.False(columnTag.Equals(null));
        Assert.NotNull(columnTag);
        Assert.NotNull(columnTag);
        Assert.NotNull(columnTag);
        Assert.NotNull(columnTag);
    }

    [Fact]
    public void Equals_DifferentCase_ReturnsTrue()
    {
        ColumnTag lower = new(new TableTag(new SqlServerDialect(), "s", "t"), new ColumnName("c"));
        ColumnTag upper = new(new TableTag(new SqlServerDialect(), "S", "T"), new ColumnName("C"));

        Assert.True(lower.Equals(upper));
        Assert.True(upper.Equals(lower));
    }

    [Fact]
    public void GetHashCode_DifferentCase_EqualInstances_HaveSameHash()
    {
        ColumnTag lower = new(new TableTag(new SqlServerDialect(), "s", "t"), new ColumnName("c"));
        ColumnTag upper = new(new TableTag(new SqlServerDialect(), "S", "T"), new ColumnName("C"));

        Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_RetrievalByDifferentCaseColumnTag_Works()
    {
        Dictionary<ColumnTag, string> dict = [];
        ColumnTag keyLower = new(new TableTag(new SqlServerDialect(), "s", "t"), new ColumnName("c"));
        dict[keyLower] = "value";

        ColumnTag keyUpper = new(new TableTag(new SqlServerDialect(), "S", "T"), new ColumnName("C"));

        Assert.True(dict.ContainsKey(keyUpper));
        Assert.Equal("value", dict[keyUpper]);
    }

}
