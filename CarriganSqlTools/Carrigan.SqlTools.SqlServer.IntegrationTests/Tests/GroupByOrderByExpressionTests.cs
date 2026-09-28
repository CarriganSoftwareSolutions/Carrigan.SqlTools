using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class GroupByOrderByExpressionTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public GroupByOrderByExpressionTests(BooksFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task GroupByParameterizedExpression_Executes()
    {
        Count countExpression = new(new Column<Book>(nameof(Book.Id)));

        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag(countExpression, nameof(IntegerValueAndCount.Count))
            ),
            GroupBys = new GroupBys(new GroupBy(PageRemainderExpression()))
        };

        SqlQuery query = BookSqlGenerator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        IEnumerable<IntegerValueAndCount> records =
            await CommandsAsync.ExecuteReaderAsync<IntegerValueAndCount>(query, null, connection);

        int[] expected =
        [
            .. BookDataSet.Data
                .GroupBy(static book => book.Pages!.Value % 100)
                .Select(static group => group.Count())
                .OrderBy(static count => count)
        ];

        int[] actual =
        [
            .. records
                .Select(static record => record.Count)
                .OrderBy(static count => count)
        ];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task SelectedExpression_GroupedByEquivalentExpression_Executes()
    {
        Add selectedExpression = PagesPlusYearExpression();
        Add groupedExpression = PagesPlusYearExpression();
        Add orderedExpression = PagesPlusYearExpression();
        Count countExpression = new(new Column<Book>(nameof(Book.Id)));

        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag(selectedExpression, nameof(IntegerValueAndCount.Value)),
                new SelectTag(countExpression, nameof(IntegerValueAndCount.Count))
            ),
            GroupBys = new GroupBys(new GroupBy(groupedExpression)),
            OrderBys = new OrderBys(new OrderBy(orderedExpression))
        };

        SqlQuery query = BookSqlGenerator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        IEnumerable<IntegerValueAndCount> records =
            await CommandsAsync.ExecuteReaderAsync<IntegerValueAndCount>(query, null, connection);

        (int Value, int Count)[] expected =
        [
            .. BookDataSet.Data
                .GroupBy(static book => book.Pages!.Value + book.YearPublished!.Value)
                .OrderBy(static group => group.Key)
                .Select(static group => (group.Key, group.Count()))
        ];

        (int Value, int Count)[] actual =
        [
            .. records.Select(static record => (record.Value, record.Count))
        ];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task SelectedCompositeExpression_GroupedByParticipatingColumns_Executes()
    {
        Add selectedExpression = PagesPlusYearExpression();
        Add orderedExpression = PagesPlusYearExpression();
        Count countExpression = new(new Column<Book>(nameof(Book.Id)));

        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag(selectedExpression, nameof(IntegerValueAndCount.Value)),
                new SelectTag(countExpression, nameof(IntegerValueAndCount.Count))
            ),
            GroupBys = new GroupBys
            (
                new GroupBy(new Column<Book>(nameof(Book.Pages))),
                new GroupBy(new Column<Book>(nameof(Book.YearPublished)))
            ),
            OrderBys = new OrderBys(new OrderBy(orderedExpression))
        };

        SqlQuery query = BookSqlGenerator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        IEnumerable<IntegerValueAndCount> records =
            await CommandsAsync.ExecuteReaderAsync<IntegerValueAndCount>(query, null, connection);

        (int Value, int Count)[] expected =
        [
            .. BookDataSet.Data
                .GroupBy(static book => (book.Pages!.Value, book.YearPublished!.Value))
                .Select(static group => (group.Key.Item1 + group.Key.Item2, group.Count()))
                .OrderBy(static item => item.Item1)
        ];

        (int Value, int Count)[] actual =
        [
            .. records.Select(static record => (record.Value, record.Count))
        ];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task OrderByParameterizedExpression_Descending_Executes()
    {
        Add orderedExpression = new
        (
            new Column<Book>(nameof(Book.YearPublished)),
            new Parameter(0, "YearOffset")
        );

        SelectBuilder<Book> selectBuilder = new()
        {
            OrderBys = new OrderBys(new OrderBy(orderedExpression, SortDirectionEnum.Descending))
        };

        SqlQuery query = BookSqlGenerator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> records = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, connection);

        int?[] expectedIds =
        [
            .. BookDataSet.Data
                .OrderByDescending(static book => book.YearPublished)
                .Select(static book => book.Id)
        ];

        Assert.Equal(expectedIds, records.Select(static book => book.Id));
    }

    private static Mod PageRemainderExpression() =>
        new
        (
            new Column<Book>(nameof(Book.Pages)),
            new Parameter(100, "PageModulo")
        );

    private static Add PagesPlusYearExpression() =>
        new
        (
            new Column<Book>(nameof(Book.Pages)),
            new Column<Book>(nameof(Book.YearPublished))
        );
}
