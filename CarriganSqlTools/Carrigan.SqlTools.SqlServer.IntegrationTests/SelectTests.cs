//Ignore Spelling: SqlTools, Localdb, Respawn, Respawner, Carrigan, SqlServer

using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests;

public sealed class SelectTests : IClassFixture<SelectsFixture>
{
    private readonly SelectsFixture _fixture;

    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public SelectTests(SelectsFixture fixture) =>
        _fixture = fixture;


    [Fact]
    public async Task SelectAll()
    {
        SqlQuery query = BookSqlGenerator.SelectAll();
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(11, books.Count());
        for (int i = 1; i < 12; i++)
        {
            BookDataSet.ValidateById(books, i);
        }
    }

    [Fact]
    public async Task SelectById()
    {
        SqlQuery query = BookSqlGenerator.SelectById(new Book() { Id = 5 });
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Single(books);
        BookDataSet.ValidateById(books, 5);
    }

    [Fact]
    public async Task And()
    {
        Predicates year = new ColumnValue<Book>(nameof(Book.YearPublished), 1890);
        Predicates price = new ColumnValue<Book>(nameof(Book.Price), 10.99m);
        Predicates and = new And(year, price);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, and, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Single(books);
        BookDataSet.ValidateById(books, 9);
    }

    [Fact]
    public async Task SelectColumnValue()
    {
        Predicates predicates = new ColumnValue<Book>(nameof(Book.YearPublished), 1865);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Single(books);
        BookDataSet.ValidateById(books, 4);
    }

    //TODO: we need a integration unit test for contains, but are being limited by our current local db server instance.


    [Fact]
    public async Task SelectEqual()
    {
        Column<Book> year = new(nameof(Book.YearPublished));
        Parameter value = new("YearPublished", 1865);
        Predicates predicates = new Equal(year, value);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Single(books);
        BookDataSet.ValidateById(books, 4);
    }

    [Fact]
    public async Task SelectGreaterThan()
    {
        Column<Book> year = new(nameof(Book.YearPublished));
        Parameter value = new("YearPublished", 1865);
        Predicates predicates = new GreaterThan(year, value);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(6, books.Count());
        BookDataSet.ValidateById(books, 3);
        BookDataSet.ValidateById(books, 5);
        BookDataSet.ValidateById(books, 7);
        BookDataSet.ValidateById(books, 8);
        BookDataSet.ValidateById(books, 9);
        BookDataSet.ValidateById(books, 11);
    }

    [Fact]
    public async Task SelectGreaterThanEqual()
    {
        Column<Book> year = new(nameof(Book.YearPublished));
        Parameter value = new("YearPublished", 1865);
        Predicates predicates = new GreaterThanEqual(year, value);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(7, books.Count());
        BookDataSet.ValidateById(books, 3);
        BookDataSet.ValidateById(books, 4);
        BookDataSet.ValidateById(books, 5);
        BookDataSet.ValidateById(books, 7);
        BookDataSet.ValidateById(books, 8);
        BookDataSet.ValidateById(books, 9);
        BookDataSet.ValidateById(books, 11);
    }

    [Fact]
    public async Task SelectIsNotNull()
    {
        Column<Book> price = new(nameof(Book.Price));
        Predicates predicates = new IsNotNull(price);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);


        Assert.Equal(10, books.Count());
        for (int i = 1; i < 11; i++)
        {
            BookDataSet.ValidateById(books, i);
        }
    }

    [Fact]
    public async Task SelectIsNull()
    {
        Column<Book> price = new(nameof(Book.Price));
        Predicates predicates = new IsNull(price);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);


        Assert.Single(books);
        BookDataSet.ValidateById(books, 11);
    }

    [Fact]
    public async Task SelectLessThan()
    {
        Column<Book> year = new(nameof(Book.YearPublished));
        Parameter value = new("YearPublished", 1851);
        Predicates predicates = new LessThan(year, value);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(3, books.Count());
        BookDataSet.ValidateById(books, 1);
        BookDataSet.ValidateById(books, 6);
        BookDataSet.ValidateById(books, 10);
    }

    [Fact]
    public async Task SelectLessThanEqual()
    {
        Column<Book> year = new(nameof(Book.YearPublished));
        Parameter value = new("YearPublished", 1851);
        Predicates predicates = new LessThanEqual(year, value);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(4, books.Count());
        BookDataSet.ValidateById(books, 1);
        BookDataSet.ValidateById(books, 2);
        BookDataSet.ValidateById(books, 6);
        BookDataSet.ValidateById(books, 10);
    }

    [Fact]
    public async Task SelectLike()
    {
        Column<Book> title = new(nameof(Book.Title));
        Parameter value = new("Title", "%oF%");
        Predicates predicates = new Like(title, value);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(4, books.Count());
        BookDataSet.ValidateById(books, 3);
        BookDataSet.ValidateById(books, 5);
        BookDataSet.ValidateById(books, 9);
        BookDataSet.ValidateById(books, 10);
    }

    [Fact]
    public async Task SelectLikeCaseSensitive_Match()
    {
        Column<Book> title = new(nameof(Book.Title));
        Parameter value = new("Title", "%of%");
        Predicates predicates = new Like(title, value, true);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(4, books.Count());
        BookDataSet.ValidateById(books, 3);
        BookDataSet.ValidateById(books, 5);
        BookDataSet.ValidateById(books, 9);
        BookDataSet.ValidateById(books, 10);
    }

    [Fact]
    public async Task SelectLikeCaseInsensitive_Match()
    {
        Column<Book> title = new(nameof(Book.Title));
        Parameter value = new("Title", "%oF%");
        Predicates predicates = new Like(title, value, false);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(4, books.Count());
        BookDataSet.ValidateById(books, 3);
        BookDataSet.ValidateById(books, 5);
        BookDataSet.ValidateById(books, 9);
        BookDataSet.ValidateById(books, 10);
    }

    [Fact]
    public async Task SelectLikeCaseSensitive_Miss()
    {
        Column<Book> title = new(nameof(Book.Title));
        Parameter value = new("Title", "%oF%");
        Predicates predicates = new Like(title, value, true);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Empty(books);
    }

    [Fact]
    public async Task SelectNotLessThan()
    {
        Column<Book> year = new(nameof(Book.YearPublished));
        Parameter value = new("YearPublished", 1897);
        Predicates predicates = new Not(new LessThan(year, value));


        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(2, books.Count());
        BookDataSet.ValidateById(books, 7);
        BookDataSet.ValidateById(books, 11);
    }

    [Fact]
    public async Task SelectNotEqual()
    {
        Column<Book> year = new(nameof(Book.YearPublished));
        Parameter value = new("YearPublished", 1897);
        Predicates predicates = new NotEqual(year, value);


        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, predicates, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(10, books.Count());
        BookDataSet.ValidateById(books, 1);
        BookDataSet.ValidateById(books, 2);
        BookDataSet.ValidateById(books, 3);
        BookDataSet.ValidateById(books, 4);
        BookDataSet.ValidateById(books, 5);
        BookDataSet.ValidateById(books, 6);
        BookDataSet.ValidateById(books, 8);
        BookDataSet.ValidateById(books, 9);
        BookDataSet.ValidateById(books, 10);
        BookDataSet.ValidateById(books, 11);
    }

    [Fact]
    public async Task SelectOr()
    {
        Predicates predicate1 = new ColumnValue<Book>(nameof(Book.Id), 1);
        Predicates predicate2 = new ColumnValue<Book>(nameof(Book.Id), 4);
        Predicates predicate3 = new ColumnValue<Book>(nameof(Book.Id), 6);
        Predicates predicate4 = new ColumnValue<Book>(nameof(Book.Id), 8);
        Predicates or = new Or(predicate1, predicate2, predicate3, predicate4);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, or, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(4, books.Count());
        BookDataSet.ValidateById(books, 1);
        BookDataSet.ValidateById(books, 4);
        BookDataSet.ValidateById(books, 6);
        BookDataSet.ValidateById(books, 8);
    }

    [Fact]
    public async Task SelectXOr()
    {
        Column<Book> id = new(nameof(Book.Id));
        Parameter value1 = new("valueOne", 5);
        Parameter value2 = new("valueTwo", 3);
        Xor xor = new(value1, value2);
        Equal equal = new(id, xor);

        SqlQuery query = BookSqlGenerator.Select(null, null, null, null, equal, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Single(books);
        BookDataSet.ValidateById(books, 6);
    }

    [Fact]
    public async Task SelectJoin()
    {
        ColumnEqualsColumn<Book, BookStats> joinPredicate = new(nameof(Book.Id), nameof(BookStats.BookId));
        JoinBase join = new Join<BookStats>(joinPredicate);
        Column<BookStats> ratingColumn = new(nameof(BookStats.AverageReview));
        Parameter ratingParameter = new("Rating", 4.6m);
        Predicates wherePredicate = new GreaterThan(ratingColumn, ratingParameter);
        SqlQuery query = BookSqlGenerator.Select(null, null, null, join, wherePredicate, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(3, books.Count());
        BookDataSet.ValidateById(books, 1);
        BookDataSet.ValidateById(books, 4);
        BookDataSet.ValidateById(books, 10);
    }

    [Fact]
    public async Task SelectJoinWithOrderBys()
    {
        ColumnEqualsColumn<Book, BookStats> joinPredicate = new(nameof(Book.Id), nameof(BookStats.BookId));
        JoinBase join = new Join<BookStats>(joinPredicate);
        Column<BookStats> ratingColumn = new(nameof(BookStats.AverageReview));
        Parameter ratingParameter = new("Rating", 4.5m);
        Predicates wherePredicate = new GreaterThan(ratingColumn, ratingParameter);
        OrderBys orderBy = new (new OrderBy<BookStats>(nameof(BookStats.AverageReview)), new OrderBy<Book>(nameof(Book.YearPublished), SortDirectionEnum.Descending));
        SqlQuery query = BookSqlGenerator.Select(null, null, null, join, wherePredicate, orderBy, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(5, books.Count());
        BookDataSet.Validate(books.ElementAt(0), 5);
        BookDataSet.Validate(books.ElementAt(1), 2);
        BookDataSet.Validate(books.ElementAt(2), 10);
        BookDataSet.Validate(books.ElementAt(3), 1);
        BookDataSet.Validate(books.ElementAt(4), 4);
    }

    [Fact]
    public async Task SelectJoinWithOrderBysAndPaging()
    {
        ColumnEqualsColumn<Book, BookStats> joinPredicate = new(nameof(Book.Id), nameof(BookStats.BookId));
        JoinBase join = new Join<BookStats>(joinPredicate);
        Column<BookStats> ratingColumn = new(nameof(BookStats.AverageReview));
        Parameter ratingParameter = new("Rating", 4.3m);
        Predicates wherePredicate = new GreaterThan(ratingColumn, ratingParameter);
        OrderBys orderBy = new (new OrderBy<BookStats>(nameof(BookStats.AverageReview)), new OrderBy<Book>(nameof(Book.YearPublished), SortDirectionEnum.Descending));
        PagingBase paging = new DefinePage(2, 3);
        SqlQuery query = BookSqlGenerator.Select(null, null, null, join, wherePredicate, orderBy, paging);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);

        Assert.Equal(3, books.Count());
        BookDataSet.Validate(books.ElementAt(0), 5);
        BookDataSet.Validate(books.ElementAt(1), 2);
        BookDataSet.Validate(books.ElementAt(2), 10);
    }

    [Fact]
    public async Task SelectJoinWithOrderBysAndPagingAndSelectTags()
    {
        ColumnEqualsColumn<Book, BookStats> joinPredicate = new(nameof(Book.Id), nameof(BookStats.BookId));
        JoinBase join = new Join<BookStats>(joinPredicate);

        Column<BookStats> ratingColumn = new(nameof(BookStats.AverageReview));
        Parameter ratingParameter = new("Rating", 4.3m);
        Predicates wherePredicate = new GreaterThan(ratingColumn, ratingParameter);

        OrderBys orderBy = new
        (
            new OrderBy<BookStats>(nameof(BookStats.AverageReview)),
            new OrderBy<Book>(nameof(Book.YearPublished), SortDirectionEnum.Descending)
        );

        PagingBase paging = new DefinePage(2, 3);

        SelectTags selectTags = SelectTags.GetMany<Book>
        (
            nameof(Book.Title),
            nameof(Book.Author)
        ).Append<BookStats>(nameof(BookStats.AverageReview));

        SqlQuery query = BookSqlGenerator.Select(null, null, selectTags, join, wherePredicate, orderBy, paging);

        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        //How to do this final projection needs more visible documentation
        IEnumerable<BookStatsSelects> books = await CommandsAsync.ExecuteReaderAsync<BookStatsSelects>(query, null, unitTestConnection);

        Assert.Equal(3, books.Count());

        BookStatsSelects book = books.ElementAt(1);
        Assert.Equal("Moby-Dick", book.Title);
        Assert.Equal("Herman Melville", book.Author);
        Assert.Equal(4.6m, book.AverageReview);
        Assert.Equal(string.Empty, book.Description);
    }
}