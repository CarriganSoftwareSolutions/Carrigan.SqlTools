using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentExtensionsTests
{
    [Fact]
    public void ToSql_Empty_ReturnsEmptyString()
    {
        IEnumerable<SqlFragment> sqlFragments = [];

        string actualValue = sqlFragments.ToSql();

        Assert.Equal(string.Empty, actualValue);
    }

    [Fact]
    public void ToSql_MultipleFragments_ConcatenatesInOrder()
    {
        IEnumerable<SqlFragment> sqlFragments =
            [
                new SqlFragmentText("SELECT "),
                new SqlFragmentText("1"),
                new SqlFragmentText(";")
            ];

        string actualValue = sqlFragments.ToSql();

        Assert.Equal("SELECT 1;", actualValue);
    }

    [Fact]
    public void ToSql_NullSource_Exception()
    {
        IEnumerable<SqlFragment> sqlFragments = null!;

        Assert.Throws<ArgumentNullException>(() => sqlFragments.ToSql());
    }

    [Fact]
    public void GetParameters_NullSource_Exception()
    {
        IEnumerable<SqlFragment> sqlFragments = null!;

        Assert.Throws<ArgumentNullException>(() => sqlFragments.GetParameters());
    }

    [Fact]
    public void GetParameters_DuplicateKeys_Exception()
    {
        IEnumerable<SqlFragment> first = new Parameter("Name", 1).ToSqlFragments("Parameter");
        IEnumerable<SqlFragment> second = new Parameter("Name", 2).ToSqlFragments("Parameter");

        IEnumerable<SqlFragment> sqlFragments = [.. first, .. second];

        Assert.Throws<ArgumentException>(() => sqlFragments.GetParameters());
    }

    [Fact]
    public void JoinFragments_WithNullSeparator_ReturnsFragmentsWithoutSeparator()
    {
        SqlFragment[] fragments =
        [
            new SqlFragmentText("A"),
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragments
            .JoinFragments()
            .ToSql();

        Assert.Equal("ABC", sql);
    }

    [Fact]
    public void JoinFragments_WithTextSeparator_InsertsSeparatorBetweenFragments()
    {
        SqlFragment[] fragments =
        [
            new SqlFragmentText("A"),
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragments
            .JoinFragments(new SqlFragmentText(", "))
            .ToSql();

        Assert.Equal("A, B, C", sql);
    }

    [Fact]
    public void JoinFragments_WithSingleFragment_ReturnsSingleFragment()
    {
        SqlFragment[] fragments =
        [
            new SqlFragmentText("A")
        ];

        string sql = fragments
            .JoinFragments(new SqlFragmentText(", "))
            .ToSql();

        Assert.Equal("A", sql);
    }

    [Fact]
    public void JoinFragments_WithEmptySequence_ReturnsEmptySql()
    {
        SqlFragment[] fragments = [];

        string sql = fragments
            .JoinFragments(new SqlFragmentText(", "))
            .ToSql();

        Assert.Equal(string.Empty, sql);
    }

    [Fact]
    public void JoinFragments_WhenFragmentsIsNull_ThrowsArgumentNullException()
    {
        IEnumerable<SqlFragment>? fragments = null;

        Assert.Throws<ArgumentNullException>(() =>
            fragments!.JoinFragments(new SqlFragmentText(", ")).ToList());
    }



    [Fact]
    public void Append_ReturnsBothFragmentsInOrder()
    {
        SqlFragment fragment1 = new SqlFragmentText("A");
        SqlFragment fragment2 = new SqlFragmentText("B");

        string sql = fragment1
            .Append(fragment2)
            .ToSql();

        Assert.Equal("AB", sql);
    }

    [Fact]
    public void Concat_ReturnsFragmentThenSequenceInOrder()
    {
        SqlFragment fragment1 = new SqlFragmentText("A");

        SqlFragment[] fragments =
        [
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragment1
            .Concat(fragments)
            .ToSql();

        Assert.Equal("ABC", sql);
    }

    [Fact]
    public void Concat_WithEmptySequence_ReturnsOnlyFirstFragment()
    {
        SqlFragment fragment1 = new SqlFragmentText("A");

        SqlFragment[] fragments = [];

        string sql = fragment1
            .Concat(fragments)
            .ToSql();

        Assert.Equal("A", sql);
    }

    [Fact]
    public void JoinFragments_WithFragmentGroupSeparator_InsertsGroupBetweenFragments()
    {
        SqlFragmentGroup separator = new(
        [
            new SqlFragmentText(" "),
            new SqlFragmentText("AND"),
            new SqlFragmentText(" ")
        ]);

        IEnumerable<SqlFragment> fragments =
        [
            new SqlFragmentText("A"),
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragments
            .JoinFragments(separator)
            .ToSql();

        Assert.Equal("A AND B AND C", sql);
    }

    [Fact]
    public void Append_ReturnsSameFragmentInstancesInOrder()
    {
        SqlFragment fragment1 = new SqlFragmentText("A");
        SqlFragment fragment2 = new SqlFragmentText("B");

        SqlFragment[] fragments = fragment1.Append(fragment2).ToArray();

        Assert.Equal([fragment1, fragment2], fragments);
    }

    [Fact]
    public void Concat_ReturnsSameFragmentInstancesInOrder()
    {
        SqlFragment fragment1 = new SqlFragmentText("A");
        SqlFragment fragment2 = new SqlFragmentText("B");
        SqlFragment fragment3 = new SqlFragmentText("C");

        SqlFragment[] fragments = fragment1
            .Concat([fragment2, fragment3])
            .ToArray();

        Assert.Equal([fragment1, fragment2, fragment3], fragments);
    }
}
