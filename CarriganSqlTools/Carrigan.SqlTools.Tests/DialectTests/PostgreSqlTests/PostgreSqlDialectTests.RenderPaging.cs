using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Paging;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderPaging_OffsetZeroNextZero_ReturnsEmptyString()
    {
        string actual = Dialect.RenderPaging(new OffsetFetchNext(0, 0)).ToSql(Dialect);
        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void RenderPaging_OffsetZeroNextGreaterThanZero_ReturnsLimitOnly()
    {
        string actual = Dialect.RenderPaging(new OffsetFetchNext(0, 25)).ToSql(Dialect);
        Assert.Equal("LIMIT 25", actual);
    }

    [Fact]
    public void RenderPaging_OffsetGreaterThanZeroNextZero_ReturnsOffsetOnly()
    {
        string actual = Dialect.RenderPaging(new OffsetFetchNext(25, 0)).ToSql(Dialect);
        Assert.Equal("OFFSET 25", actual);
    }

    [Fact]
    public void RenderPaging_OffsetAndNextGreaterThanZero_ReturnsLimitAndOffset()
    {
        string actual = Dialect.RenderPaging(new OffsetFetchNext(25, 10)).ToSql(Dialect);
        Assert.Equal("LIMIT 10 OFFSET 25", actual);
    }
}
