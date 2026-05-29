using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Paging;
using Xunit;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderPaging_OffsetFetchNextWithNoOffsetAndNoNext_ReturnsEmptyString()
    {
        OffsetFetchNext paging = new(0, 0);

        ISqlFragment actual = Dialect.RenderPaging(paging);

        Assert.Equal(string.Empty, actual.ToSql(Dialect));
    }

    [Fact]
    public void RenderPaging_OffsetFetchNextWithNextOnly_ReturnsOffsetZeroFetchNext()
    {
        OffsetFetchNext paging = new(0, 10);

        ISqlFragment actual = Dialect.RenderPaging(paging);

        Assert.Equal("OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", actual.ToSql(Dialect));
    }

    [Fact]
    public void RenderPaging_OffsetFetchNextWithOffsetAndNext_ReturnsOffsetFetchNext()
    {
        OffsetFetchNext paging = new(20, 10);

        ISqlFragment actual = Dialect.RenderPaging(paging);

        Assert.Equal("OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY", actual.ToSql(Dialect));
    }

    [Fact]
    public void RenderPaging_OffsetFetchNextWithOffsetOnly_ReturnsOffsetRows()
    {
        OffsetFetchNext paging = new(20, 0);

        ISqlFragment actual = Dialect.RenderPaging(paging);

        Assert.Equal("OFFSET 20 ROWS", actual.ToSql(Dialect));
    }

    [Fact]
    public void RenderPaging_DefinePageFirstPage_ReturnsOffsetZeroFetchNextPageSize()
    {
        DefinePage paging = new(1, 25);

        ISqlFragment actual = Dialect.RenderPaging(paging);

        Assert.Equal("OFFSET 0 ROWS FETCH NEXT 25 ROWS ONLY", actual.ToSql(Dialect));
    }

    [Fact]
    public void RenderPaging_DefinePageSecondPage_ReturnsOffsetPageSizeFetchNextPageSize()
    {
        DefinePage paging = new(2, 25);

        ISqlFragment actual = Dialect.RenderPaging(paging);

        Assert.Equal("OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY", actual.ToSql(Dialect));
    }

    [Fact]
    public void RenderPaging_DefinePageThirdPage_ReturnsOffsetTwoPagesFetchNextPageSize()
    {
        DefinePage paging = new(3, 25);

        ISqlFragment actual = Dialect.RenderPaging(paging);

        Assert.Equal("OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY", actual.ToSql(Dialect));
    }
}