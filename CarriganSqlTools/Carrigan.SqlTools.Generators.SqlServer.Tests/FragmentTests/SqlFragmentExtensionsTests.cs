using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.FragmentTests;

public class SqlFragmentExtensionsTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void ToSql_Empty()
    {
        IEnumerable<ISqlFragment> sqlFragments = [];

        string actualValue = sqlFragments.ToSql(Dialect);

        Assert.Equal(string.Empty, actualValue);
    }

    [Fact]
    public void ToSql_MultipleFragments()
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
    public void JoinFragments_WithNullSeparator()
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
    public void JoinFragments_WithTextSeparator()
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
    public void JoinFragments_WithStringSeparator()
    {
        ISqlFragment[] fragments =
        [
            new SqlFragmentText("A"),
            new SqlFragmentText("B"),
            new SqlFragmentText("C")
        ];

        string sql = fragments
            .JoinFragments(", ")
            .ToSql(Dialect);

        Assert.Equal("A, B, C", sql);
    }

    [Fact]
    public void JoinFragments_WithSingleFragment()
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
    public void JoinFragments_WithEmptySequence()
    {
        ISqlFragment[] fragments = [];

        string sql = fragments
            .JoinFragments(new SqlFragmentText(", "))
            .ToSql(Dialect);

        Assert.Equal(string.Empty, sql);
    }

    [Fact]
    public void JoinFragments_WhenFragmentsIsNull_Exception()
    {
        IEnumerable<ISqlFragment>? fragments = null;

        Assert.Throws<ArgumentNullException>(() =>
            fragments!.JoinFragments(new SqlFragmentText(", ")).ToList());
    }

    [Fact]
    public void Append_BothFragments()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");
        ISqlFragment fragment2 = new SqlFragmentText("B");

        string sql = fragment1
            .Append(fragment2)
            .ToSql(Dialect);

        Assert.Equal("AB", sql);
    }

    [Fact]
    public void Concat_FragmentThenSequence()
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
    public void Concat_WithEmptySequence()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");

        ISqlFragment[] fragments = [];

        string sql = fragment1
            .Concat(fragments)
            .ToSql(Dialect);

        Assert.Equal("A", sql);
    }

    [Fact]
    public void JoinFragments_WithFragmentGroupSeparator()
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
    public void Append_SameFragmentInstances()
    {
        ISqlFragment fragment1 = new SqlFragmentText("A");
        ISqlFragment fragment2 = new SqlFragmentText("B");

        ISqlFragment[] fragments = fragment1.Append(fragment2).ToArray();

        Assert.Equal([fragment1, fragment2], fragments);
    }

    [Fact]
    public void Concat_SameFragmentInstances()
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
