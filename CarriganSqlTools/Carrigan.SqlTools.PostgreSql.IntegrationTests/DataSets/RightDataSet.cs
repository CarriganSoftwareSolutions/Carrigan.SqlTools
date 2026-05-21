using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Generators.PostgreSql;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.DataSets;

public static class RightDataSet
{
    public static void Validate(IEnumerable<Right> actualRights, int expectedId)
    {
        Right actual = actualRights.Where(right => right.Id == expectedId).Single();
        Validate(actual, expectedId);
    }

    public static void Validate(Right actual, int expectedId)
    {
        Right expected = Data.Where(right => right.Id == expectedId).Single();

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.RightWord, actual.RightWord);
    }

    public static IEnumerable<Right> Data =>
    [
        new()
        {
            Id = 4,
            RightWord = "Window",
        },
        new()
        {
            Id = 5,
            RightWord = "Bridge",
        },
        new()
        {
            Id = 6,
            RightWord = "Meadow",
        },
        new()
        {
            Id = 7,
            RightWord = "Lantern",
        },
        new()
        {
            Id = 8,
            RightWord = "Harbor",
        }
    ];

    public static IEnumerable<SqlQuery> InsertStatement =>
        Data.Chunk(100).Select(dataSet => new SqlGenerator<Right>().Insert(null, null, dataSet));
}