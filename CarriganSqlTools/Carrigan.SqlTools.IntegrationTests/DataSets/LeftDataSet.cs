using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class LeftDataSet
{
    public static void Validate(IEnumerable<LeftWords> actualLefts, int expectedId)
    {
        LeftWords actual = actualLefts.Where(left => left.Id == expectedId).Single();
        Validate(actual, expectedId);
    }

    public static void Validate(LeftWords actual, int expectedId)
    {
        LeftWords expected = Data.Where(left => left.Id == expectedId).Single();

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.LeftWord, actual.LeftWord);
    }

    public static IEnumerable<LeftWords> Data =>
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
}