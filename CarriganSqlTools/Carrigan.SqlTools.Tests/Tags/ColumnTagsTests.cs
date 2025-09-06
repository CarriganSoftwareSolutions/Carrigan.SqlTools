using SqlTools.Tags;

namespace SqlToolsTests.Tags;

public class ColumnTagsTests
{

    [Theory]
    [InlineData("Poppies", "Sloppy", "Pizza", "[Poppies].[Sloppy].[Pizza]")]
    [InlineData(null, "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    [InlineData("", "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    [InlineData("Poppies", null, "Pizza", "[Pizza]")]
    [InlineData("", "", "Pizza", "[Pizza]")]
    [InlineData("", null, "Pizza", "[Pizza]")]
    [InlineData(null, "", "Pizza", "[Pizza]")]
    [InlineData(null, null, "Pizza", "[Pizza]")]
    public void Col_Tag_Tests_3_Params(string? schemaName, string? tableName, string columnName, string expected)
    {
        string actual = new ColumnTag(schemaName, tableName, columnName);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Poppies", "Sloppy", "")]
    [InlineData(null, "Sloppy", "")]
    [InlineData("", "Sloppy", "")]
    [InlineData("Poppies", null, "")]
    [InlineData("", "", "")]
    [InlineData("", null, "")]
    [InlineData(null, "", "")]
    [InlineData(null, null, "")]
    [InlineData("Poppies", "Sloppy", null)]
    [InlineData(null, "Sloppy", null)]
    [InlineData("", "Sloppy", null)]
    [InlineData("Poppies", null, null)]
    [InlineData("", "", null)]
    [InlineData("", null, null)]
    [InlineData(null, "", null)]
    [InlineData(null, null, null)]
    public void Col_Tag_Tests_3_Params_null_column(string? schemaName, string? tableName, string? columnName)
    {
        Assert.Throws<ArgumentNullException>(() => new ColumnTag(schemaName, tableName, columnName!));
    }

    [Theory]
    [InlineData("Poppies", "Sloppy", "Pizza", "[Poppies].[Sloppy].[Pizza]")]
    [InlineData(null, "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    [InlineData("", "Sloppy", "Pizza", "[Sloppy].[Pizza]")]
    public void Col_Tag_Tests_2_Params(string? schemaName, string tableName, string columnName, string expected)
    {
        TableTag tg = new(schemaName, tableName);
        string actual = new ColumnTag(tg, columnName);

        Assert.Equal(expected, actual);
    }



    [Theory]
    [InlineData("Poppies", "Sloppy", "")]
    [InlineData(null, "Sloppy", "")]
    [InlineData("", "Sloppy", "")]
    [InlineData("Poppies", "Sloppy", null)]
    [InlineData(null, "Sloppy", null)]
    [InlineData("", "Sloppy", null)]
    public void Col_Tag_Tests_2_Params_argument_exception(string? schemaName, string tableName, string? columnName)
    {
        TableTag tg = new(schemaName, tableName);

        Assert.Throws<ArgumentNullException>(() => new ColumnTag(tg, columnName!));
    }


    //implicit operator → string and ToString()
    [Theory]
    [InlineData("S", "T", "C", "[S].[T].[C]")]
    [InlineData(null, "T", "C", "[T].[C]")]
    [InlineData("", "T", "C", "[T].[C]")]
    public void ImplicitStringAndToString_AreEquivalent(string? schema, string table, string column, string expected)
    {
        ColumnTag colTag = string.IsNullOrEmpty(schema)
            ? new ColumnTag(new TableTag(schema, table), column)
            : new ColumnTag(schema, table, column);

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
        ColumnTag a = new ("S", "T", "C");
        ColumnTag b = new ("S", "T", "C");

        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
    }

    [Fact]
    public void Equals_DifferentUnderlyingTag_ReturnsFalse()
    {
        ColumnTag a = new ("S", "T", "C1");
        ColumnTag b = new ("S", "T", "C2");

        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));
    }

    // == and != operators
    [Fact]
    public void EqualityOperator_WorksLikeEquals()
    {
        ColumnTag x = new ("S", "T", "C");
        ColumnTag y = new ("S", "T", "C");
        ColumnTag z = new ("S", "T", "Different");

        Assert.True(x == y);
        Assert.False(x == z);
        Assert.True(x != z);
    }

    // GetHashCode consistency
    [Fact]
    public void GetHashCode_EqualInstances_HaveSameHash()
    {
        ColumnTag a = new ("S", "T", "C");
        ColumnTag b = new ("S", "T", "C");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // IEqualityComparer<ColumnTag>
    [Fact]
    public void Comparer_EqualsAndHashCode_ViaIEqualityComparer()
    {
        ColumnTag a = new ("S", "T", "C");
        ColumnTag b = new ("S", "T", "C");
        ColumnTag c = new ("S", "T", "Other");
        ColumnTag comparer = new("S", "T", "D");

        Assert.True(comparer.Equals(a, b));
        Assert.False(comparer.Equals(a, c));
        Assert.Equal(a.GetHashCode(), comparer.GetHashCode(b));
    }

    // IComparable<ColumnTag>.CompareTo()
    [Fact]
    public void CompareTo_SortsByUnderlyingStringOrdinal()
    {
        ColumnTag lower = new ("S", "T", "A");
        ColumnTag higher = new ("S", "T", "B");

        Assert.True(lower.CompareTo(higher) < 0);
        Assert.True(higher.CompareTo(lower) > 0);
        Assert.Equal(0, lower.CompareTo(new ColumnTag("S", "T", "A")));
    }

    // Sorting a list of ColumnTags
    [Fact]
    public void Sort_ListOfColumnTags_OrdersLexicographically()
    {
        ColumnTag charlie = new ("S", "T", "Charlie");
        ColumnTag bravo = new ("S", "T", "Bravo");
        ColumnTag alpha = new ("S", "T", "Alpha");

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
        ColumnTag key1 = new ("S", "T", "C");
        dict[key1] = "value";


        var key2 = new ColumnTag("S", "T", "C");
        Assert.True(dict.ContainsKey(key2));
        Assert.Equal("value", dict[key2]);
    }

    //  Null comparisons
    [Fact]
    public void CompareTo_Null_IsGreaterThanNull()
    {
        ColumnTag ct = new ("S", "T", "C");
        Assert.True(ct.CompareTo(null) > 0);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        ColumnTag ct = new ("S", "T", "C");
        Assert.False(ct.Equals(null));
        Assert.NotNull(ct);
        Assert.NotNull(ct);
        Assert.NotNull(ct);
        Assert.NotNull(ct);
    }
}
