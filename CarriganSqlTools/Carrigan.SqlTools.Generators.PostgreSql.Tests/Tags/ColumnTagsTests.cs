using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Tags;

public class ColumnTagsTests
{
    private static readonly PostgreSqlDialect Dialect = new();

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "Pizza")]
    [InlineData(null, "Sloppy", "Pizza", "Pizza")]
    [InlineData("", "Sloppy", "Pizza", "Pizza")]
    public void ColumnNameTest(string? schemaName, string tableName, string columnName, string expected)
    {
        TableNameTag tableTag = new(schemaName, tableName);
        ColumnNameTag actual = new(tableTag, new ColumnName(columnName));

        Assert.Equal(expected, actual.ColumnName.ToString());
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "\"Franks\".\"Sloppy\"")]
    [InlineData(null, "Sloppy", "Pizza", "\"Sloppy\"")]
    [InlineData("", "Sloppy", "Pizza", "\"Sloppy\"")]
    public void ColumnTagTable(string? schemaName, string tableName, string columnName, string expected)
    {
        TableNameTag tableTag = new(schemaName, tableName);
        ColumnNameTag actual = new (tableTag, new ColumnName(columnName));

        Assert.Equal(expected, actual.TableTag.ToSql(Dialect));
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "\"Franks\".\"Sloppy\".\"Pizza\"")]
    [InlineData(null, "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    [InlineData("", "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    public void Col_Tag_Tests_3_Params(string? schemaName, string tableName, string columnName, string expected)
    {
        TableNameTag tableTag = new(schemaName, tableName);
        string actual = new ColumnNameTag(tableTag, new ColumnName(columnName)).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "\"Franks\".\"Sloppy\".\"Pizza\"")]
    [InlineData(null, "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    [InlineData("", "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    public void Col_Tag_Tests_3_Params_ExplicitToString(string? schemaName, string tableName, string columnName, string expected)
    {
        TableNameTag tableTag = new(schemaName, tableName);
        string actual = (new ColumnNameTag(tableTag, new ColumnName(columnName))).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "\"Franks\".\"Sloppy\".\"Pizza\"")]
    [InlineData(null, "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    [InlineData("", "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    public void Col_Tag_Tests_3_Params_ExplicitToString_UseTable (string? schemaName, string tableName, string columnName, string expected)
    {
        TableNameTag tableTag = new(schemaName, tableName);
        string actual = (new ColumnNameTag(tableTag, new ColumnName(columnName))).ToSql(Dialect,true);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "\"Pizza\"")]
    [InlineData(null, "Sloppy", "Pizza", "\"Pizza\"")]
    [InlineData("", "Sloppy", "Pizza", "\"Pizza\"")]
    public void Col_Tag_Tests_3_Params_ExplicitToString_DoNotUseTable(string? schemaName, string tableName, string columnName, string expected)
    {
        TableNameTag tableTag = new(schemaName, tableName);
        string actual = (new ColumnNameTag(tableTag, new ColumnName(columnName))).ToSql(Dialect, false);

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
        => _ = new ColumnNameTag(new TableNameTag(schemaName, tableName!), new ColumnName(columnName!));

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
        => _ = new ColumnNameTag(new TableNameTag(schemaName, tableName!), new ColumnName(columnName!));

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
        => _ = new ColumnNameTag(new TableNameTag(schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("", "", "")]
    [InlineData(null, "", "")]
    [InlineData("", "", null)]
    [InlineData(null, "", null)]
    //These unit tests originally enforced exceptions being throw when you create a column tag
    //However, this is now checked in the SqlGenerator's constructor.
    //I kept the tests, in case I forget I moved them on purpose.
    public void Col_Tag_Tests_3_Table_Empty(string? schemaName, string? tableName, string? columnName)
        => _ = new ColumnNameTag(new TableNameTag(schemaName, tableName!), new ColumnName(columnName!));

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
        => _ = new ColumnNameTag(new TableNameTag(schemaName, tableName!), new ColumnName(columnName!));

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
         _ = new ColumnNameTag(new TableNameTag(schemaName, tableName!), new ColumnName(columnName!));

    [Theory]
    [InlineData("Franks", "Sloppy", "Pizza", "\"Franks\".\"Sloppy\".\"Pizza\"")]
    [InlineData(null, "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    [InlineData("", "Sloppy", "Pizza", "\"Sloppy\".\"Pizza\"")]
    public void Col_Tag_Tests_2_Params(string? schemaName, string tableName, string columnName, string expected)
    {
        TableNameTag tg = new(schemaName, tableName);
        string actual = new ColumnNameTag(tg, new ColumnName(columnName)).ToSql(Dialect);

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
        TableNameTag tg = new(schemaName, tableName);

        _ = new ColumnNameTag(tg, new ColumnName(columnName!));
    }


    // ToString()
    [Theory]
    [InlineData("S", "T", "C", "S.T.C")]
    [InlineData(null, "T", "C", "T.C")]
    [InlineData("", "T", "C", "T.C")]
    public void ToString_ReturnsExpectedValue(string? schema, string table, string column, string expected)
    {
        TableNameTag tableTag = new(schema, table);
        ColumnNameTag columnTag = new(tableTag, new ColumnName(column));

        Assert.Equal(expected, columnTag.ToString());
        Assert.Equal(expected, $"{columnTag}");
    }

    [Fact]
    public void ToString_UnqualifiedColumn_ReturnsColumnName()
    {
        ColumnNameTag columnTag = new(new ColumnName("Column"));

        Assert.Equal("Column", columnTag.ToString());
    }

    [Fact]
    public void Equals_SameReference()
    {
        ColumnNameTag columnTag = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));

        Assert.True(columnTag.Equals(columnTag));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(columnTag == columnTag);
        Assert.False(columnTag != columnTag);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equals_EquivalentInstances()
    {
        ColumnNameTag left = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag right = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));

        Assert.True(left.Equals(right));
        Assert.True(right.Equals(left));
        Assert.Equal(left, right);
        Assert.Equal(right, left);
    }

    [Fact]
    public void Equals_Transitive()
    {
        ColumnNameTag first = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag second = new(new TableNameTag("schema", "table"), new ColumnName("column"));
        ColumnNameTag third = new(new TableNameTag("SCHEMA", "TABLE"), new ColumnName("COLUMN"));

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
    }

    [Fact]
    public void Equals_NullAndEmptySchema()
    {
        ColumnNameTag noSchema = new(new TableNameTag((SchemaName?)null, new TableName("Table")), new ColumnName("Column"));
        ColumnNameTag emptySchema = new(new TableNameTag(new SchemaName(string.Empty), new TableName("Table")), new ColumnName("Column"));

        Assert.True(noSchema.Equals(emptySchema));
        Assert.True(emptySchema.Equals(noSchema));
        Assert.True(noSchema == emptySchema);
        Assert.False(noSchema != emptySchema);
        Assert.Equal(noSchema.GetHashCode(), emptySchema.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentSchema()
    {
        ColumnNameTag left = new(new TableNameTag("SchemaOne", "Table"), new ColumnName("Column"));
        ColumnNameTag right = new(new TableNameTag("SchemaTwo", "Table"), new ColumnName("Column"));

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_DifferentTable()
    {
        ColumnNameTag left = new(new TableNameTag("Schema", "TableOne"), new ColumnName("Column"));
        ColumnNameTag right = new(new TableNameTag("Schema", "TableTwo"), new ColumnName("Column"));

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_DifferentColumn()
    {
        ColumnNameTag left = new(new TableNameTag("Schema", "Table"), new ColumnName("ColumnOne"));
        ColumnNameTag right = new(new TableNameTag("Schema", "Table"), new ColumnName("ColumnTwo"));

        Assert.False(left.Equals(right));
        Assert.False(right.Equals(left));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_SchemaTableAndColumnAreCaseInsensitive()
    {
        ColumnNameTag lower = new(new TableNameTag("schema", "table"), new ColumnName("column"));
        ColumnNameTag upper = new(new TableNameTag("SCHEMA", "TABLE"), new ColumnName("COLUMN"));

        Assert.True(lower.Equals(upper));
        Assert.True(upper.Equals(lower));
        Assert.True(lower == upper);
        Assert.False(lower != upper);
        Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
    }

    [Fact]
    public void Equals_PreservesWhitespace()
    {
        ColumnNameTag left = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag right = new(new TableNameTag("Schema", "Table"), new ColumnName(" Column "));

        Assert.False(left.Equals(right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equals_UsesStructuralIdentity_NotFormattedText()
    {
        ColumnNameTag left = new(new TableNameTag("A.B", "C"), new ColumnName("D"));
        ColumnNameTag right = new(new TableNameTag("A", "B.C"), new ColumnName("D"));

        Assert.Equal(left.ToString(), right.ToString());
        Assert.False(left.Equals(right));
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_Null()
    {
        ColumnNameTag columnTag = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag? other = null;

        Assert.False(columnTag.Equals(other));
        Assert.False(columnTag.Equals((object?)null));
    }

    [Fact]
    public void Equals_ObjectEquivalent()
    {
        ColumnNameTag columnTag = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        object other = new ColumnNameTag(new TableNameTag("schema", "table"), new ColumnName("column"));

        Assert.True(columnTag.Equals(other));
    }

    [Fact]
    public void Equals_ObjectDifferentType()
    {
        ColumnNameTag columnTag = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));

        Assert.False(columnTag.Equals("Schema.Table.Column"));
    }

    [Fact]
    public void EqualityOperators_EquivalentAndDifferentInstances()
    {
        ColumnNameTag left = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag equivalent = new(new TableNameTag("schema", "table"), new ColumnName("column"));
        ColumnNameTag different = new(new TableNameTag("Schema", "Table"), new ColumnName("Different"));

        Assert.True(left == equivalent);
        Assert.False(left != equivalent);
        Assert.False(left == different);
        Assert.True(left != different);
    }

    [Fact]
    public void EqualityOperators_NullHandling()
    {
        ColumnNameTag? left = null;
        ColumnNameTag? right = null;
        ColumnNameTag value = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));

        Assert.True(left == right);
        Assert.False(left != right);
        Assert.False(left == value);
        Assert.False(value == right);
        Assert.True(left != value);
        Assert.True(value != right);
    }

    [Fact]
    public void GetHashCode_EquivalentInstances_HaveSameHash()
    {
        ColumnNameTag left = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag right = new(new TableNameTag("schema", "table"), new ColumnName("column"));

        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DictionaryKey_RetrievalByEquivalentColumnTag_Works()
    {
        Dictionary<ColumnNameTag, string> dictionary = [];
        ColumnNameTag key = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag equivalentKey = new(new TableNameTag("schema", "table"), new ColumnName("column"));

        dictionary[key] = "value";

        Assert.True(dictionary.ContainsKey(equivalentKey));
        Assert.Equal("value", dictionary[equivalentKey]);
    }

    [Fact]
    public void DictionaryKey_EquivalentKey_ReplacesExistingValue()
    {
        Dictionary<ColumnNameTag, string> dictionary = [];
        ColumnNameTag first = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag second = new(new TableNameTag("schema", "table"), new ColumnName("column"));

        dictionary[first] = "first";
        dictionary[second] = "second";

        Assert.Single(dictionary);
        Assert.Equal("second", dictionary[first]);
    }

    [Fact]
    public void DictionaryKey_DifferentColumn_IsNotFound()
    {
        Dictionary<ColumnNameTag, string> dictionary = [];
        ColumnNameTag key = new(new TableNameTag("Schema", "Table"), new ColumnName("Column"));
        ColumnNameTag different = new(new TableNameTag("Schema", "Table"), new ColumnName("Other"));

        dictionary[key] = "value";

        Assert.False(dictionary.ContainsKey(different));
    }

    [Fact]
    public void HashSet_EquivalentColumnTag_IsRecognized()
    {
        HashSet<ColumnNameTag> tags =
        [
            new(new TableNameTag("Schema", "Table"), new ColumnName("Column"))
        ];

        Assert.Contains(new ColumnNameTag(new TableNameTag("schema", "table"), new ColumnName("column")), tags);
        Assert.DoesNotContain(new ColumnNameTag(new TableNameTag("Schema", "Table"), new ColumnName("Other")), tags);
    }

    [Theory]
    [InlineData("", true, true)]
    [InlineData(" ", true, false)]
    [InlineData("Column", false, false)]
    public void WhiteSpaceAndEmptyContracts(string columnName, bool isWhiteSpace, bool isEmpty)
    {
        ColumnNameTag columnTag = new(new TableNameTag("Schema", "Table"), new ColumnName(columnName));

        Assert.Equal(isWhiteSpace, columnTag.IsWhiteSpace());
        Assert.Equal(isWhiteSpace == false, columnTag.IsNotWhiteSpace());
        Assert.Equal(isEmpty, columnTag.IsEmpty());
        Assert.Equal(isEmpty == false, columnTag.IsNotEmpty());
    }
}
