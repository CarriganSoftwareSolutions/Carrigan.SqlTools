using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.DataSets;

public static class LeftDataSet
{
    public static void Validate(IEnumerable<Left> actualLefts, int expectedId)
    {
        Left actual = actualLefts.Where(left => left.Id == expectedId).Single();
        Validate(actual, expectedId);
    }

    public static void Validate(Left actual, int expectedId)
    {
        Left expected = Data.Where(left => left.Id == expectedId).Single();

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.LeftWord, actual.LeftWord);
    }

    public static IEnumerable<Left> Data =>
    [
        new()
        {
            Id = 1,
            LeftWord = "Apple",
        },
        new()
        {
            Id = 2,
            LeftWord = "River",
        },
        new()
        {
            Id = 3,
            LeftWord = "Cloud",
        },
        new()
        {
            Id = 4,
            LeftWord = "Garden",
        },
        new()
        {
            Id = 5,
            LeftWord = "Forest",
        }
    ];

    public static IEnumerable<SqlQuery> InsertStatement =>
        Data.Chunk(100).Select(dataSet => new SqlGenerator<Left>().Insert(null, null, dataSet));
}