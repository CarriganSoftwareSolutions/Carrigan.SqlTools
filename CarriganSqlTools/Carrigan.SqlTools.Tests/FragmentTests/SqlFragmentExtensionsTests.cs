using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentExtensionsTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void ToSql_Empty_ReturnsEmptyString()
    {
        IEnumerable<ISqlFragment> sqlFragments = [];

        string actualValue = sqlFragments.ToSql(Dialect);

        Assert.Equal(string.Empty, actualValue);
    }

    [Fact]
    public void ToSql_MultipleFragments_ConcatenatesInOrder()
    {
        IEnumerable<ISqlFragment> sqlFragments =
            [
                new SqlFragmentText("SELECT "),
                new SqlFragmentText("1"),
                new SqlFragmentText(";")
            ];

        string actualValue = sqlFragments.ToSql(Dialect);

        Assert.Equal("SELECT 1;", actualValue);
    }

    [Fact]
    public void ToSql_NullSource_Exception()
    {
        IEnumerable<ISqlFragment> sqlFragments = null!;

        Assert.Throws<ArgumentNullException>(() => sqlFragments.ToSql(Dialect));
    }

    [Fact]
    public void GetParameters_NullSource_Exception()
    {
        IEnumerable<ISqlFragment> sqlFragments = null!;

        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = sqlFragments.GetSqlFragmentParameters(Dialect);
        });
    }

    [Fact]
    public void JoinFragments_WithNullSeparator_ReturnsFragmentsWithoutSeparator()
    {
        ISqlFragment[] fragments =
        [
            new SqlFragmentText("A"),
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragments
            .JoinFragments()
            .ToSql(Dialect);

        Assert.Equal("ABC", sql);
    }

    [Fact]
    public void JoinFragments_WithTextSeparator_InsertsSeparatorBetweenFragments()
    {
        ISqlFragment[] fragments =
        [
            new SqlFragmentText("A"),
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragments
            .JoinFragments(new SqlFragmentText(", "))
            .ToSql(Dialect);

        Assert.Equal("A, B, C", sql);
    }

    [Fact]
    public void JoinFragments_WithSingleFragment_ReturnsSingleFragment()
    {
        ISqlFragment[] fragments =
        [
            new SqlFragmentText("A")
        ];

        string sql = fragments
            .JoinFragments(new SqlFragmentText(", "))
            .ToSql(Dialect);

        Assert.Equal("A", sql);
    }

    [Fact]
    public void JoinFragments_WithEmptySequence_ReturnsEmptySql()
    {
        ISqlFragment[] fragments = [];

        string sql = fragments
            .JoinFragments(new SqlFragmentText(", "))
            .ToSql(Dialect);

        Assert.Equal(string.Empty, sql);
    }

    [Fact]
    public void JoinFragments_WhenFragmentsIsNull_ThrowsArgumentNullException()
    {
        IEnumerable<ISqlFragment>? fragments = null;

        Assert.Throws<ArgumentNullException>(() =>
            fragments!.JoinFragments(new SqlFragmentText(", ")).ToList());
    }



    [Fact]
    public void Append_ReturnsBothFragmentsInOrder()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");
        ISqlFragment fragment2 = new SqlFragmentText("B");

        string sql = fragment1
            .Append(fragment2)
            .ToSql(Dialect);

        Assert.Equal("AB", sql);
    }

    [Fact]
    public void Concat_ReturnsFragmentThenSequenceInOrder()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");

        ISqlFragment[] fragments =
        [
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragment1
            .Concat(fragments)
            .ToSql(Dialect);

        Assert.Equal("ABC", sql);
    }

    [Fact]
    public void Concat_WithEmptySequence_ReturnsOnlyFirstFragment()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");

        ISqlFragment[] fragments = [];

        string sql = fragment1
            .Concat(fragments)
            .ToSql(Dialect);

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

        IEnumerable<ISqlFragment> fragments =
        [
            new SqlFragmentText("A"),
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragments
            .JoinFragments(separator)
            .ToSql(Dialect);

        Assert.Equal("A AND B AND C", sql);
    }

    [Fact]
    public void Append_ReturnsSameFragmentInstancesInOrder()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");
        ISqlFragment fragment2 = new SqlFragmentText("B");

        ISqlFragment[] fragments = fragment1.Append(fragment2).ToArray();

        Assert.Equal([fragment1, fragment2], fragments);
    }

    [Fact]
    public void Concat_ReturnsSameFragmentInstancesInOrder()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");
        ISqlFragment fragment2 = new SqlFragmentText("B");
        ISqlFragment fragment3 = new SqlFragmentText("C");

        ISqlFragment[] fragments = fragment1
            .Concat([fragment2, fragment3])
            .ToArray();

        Assert.Equal([fragment1, fragment2, fragment3], fragments);
    }
}