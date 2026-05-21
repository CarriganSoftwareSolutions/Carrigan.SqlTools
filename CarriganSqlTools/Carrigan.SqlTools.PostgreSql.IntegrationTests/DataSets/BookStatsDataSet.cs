using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Generators.PostgreSql;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.DataSets;

public static class BookStatsDataSet
{
    public static IEnumerable<BookStats> Data =>
    [
        new()
        {
            BookId = 1,
            StockCount = 42,
            AverageReview = 4.7m,
        },
        new()
        {
            BookId = 2,
            StockCount = 18,
            AverageReview = 4.6m,
        },
        new()
        {
            BookId = 3,
            StockCount = 35,
            AverageReview = 4.5m,
        },
        new()
        {
            BookId = 4,
            StockCount = 50,
            AverageReview = 4.8m,
        },
        new()
        {
            BookId = 5,
            StockCount = 44,
            AverageReview = 4.6m,
        },
        new()
        {
            BookId = 6,
            StockCount = 31,
            AverageReview = 4.4m,
        },
        new()
        {
            BookId = 7,
            StockCount = 27,
            AverageReview = 4.5m,
        },
        new()
        {
            BookId = 8,
            StockCount = 39,
            AverageReview = 4.3m,
        },
        new()
        {
            BookId = 9,
            StockCount = 24,
            AverageReview = 4.2m,
        },
        new()
        {
            BookId = 10,
            StockCount = 12,
            AverageReview = 4.7m,
        }
    ];

    public static IEnumerable<SqlQuery> InsertStatement =>
        Data.Chunk(100).Select(dataSet => new SqlGenerator<BookStats>().Insert(null, null, dataSet));
}

/*
Footer - Dataset Methodology
This book statistics dataset contains exactly one statistics record for each Book record. Stock counts and average review values were generated as deterministic synthetic values with plausible ranges for integration testing. Any resemblance to a real store inventory, review history, or commercial catalog is purely coincidental.
*/
