using Carrigan.SqlTools.IntegrationTests.CompositeModels;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

//IGNORE SPELLING: Moby-Dick

public static class CustomerBookAggregateDataSet
{
    /// <summary>
    /// Validates the complete aggregate result set.
    /// </summary>
    public static void Validate(IEnumerable<AggregateResult> actualResults)
    {
        ArgumentNullException.ThrowIfNull(actualResults);

        List<AggregateResult> actualData = [.. actualResults];
        List<AggregateResult> expectedData = [.. Data];

        Assert.Equal(expectedData.Count, actualData.Count);

        foreach (AggregateResult expected in expectedData)
        {
            ValidateByGenderAndTitle(actualData, expected.Gender, expected.Title);
        }
    }

    /// <summary>
    /// Locates and validates an aggregate result using its grouped
    /// Gender and Title values.
    /// </summary>
    public static void ValidateByGenderAndTitle(IEnumerable<AggregateResult> actualResults, char? gender, string? title)
    {
        ArgumentNullException.ThrowIfNull(actualResults);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        AggregateResult expected = Data.Single(result => result.Gender == gender && result.Title == title);

        List<AggregateResult> matches = [.. actualResults.Where(result => result.Gender == gender && result.Title == title)];

        Assert.True(matches.Count > 0, $"No aggregate result was found for Gender '{gender}' and Title '{title}'.");

        Assert.True(matches.Count == 1, $"Expected one aggregate result for Gender '{gender}' and Title '{title}', but found {matches.Count}.");

        Validate(matches[0], expected);
    }

    /// <summary>
    /// Validates every projected value in a single aggregate result.
    /// </summary>
    public static void Validate(AggregateResult actual, AggregateResult expected)
    {
        ArgumentNullException.ThrowIfNull(actual);
        ArgumentNullException.ThrowIfNull(expected);

        Assert.Equal(expected.Gender, actual.Gender);
        Assert.Equal(expected.Title, actual.Title);
        if (expected.Average is null)
            Assert.Null(actual.Average);
        else
        {
            Assert.NotNull(actual.Average);
            Assert.Equal(expected.Average.Value, actual.Average.Value, 2);
        }
        Assert.Equal(expected.Max, actual.Max);
        Assert.Equal(expected.Min, actual.Min);
        Assert.Equal(expected.Count, actual.Count);
        Assert.Equal(expected.Sum, actual.Sum);
    }

    public static IEnumerable<AggregateResult> Data =>
    [
        new()
        {
            Gender = 'F',
            Title = "Adventures of Huckleberry Finn",
            Average = 48.000000m,
            Max = 72,
            Min = 24,
            Count = 2,
            Sum = 49200,
        },
        new()
        {
            Gender = 'M',
            Title = "Adventures of Huckleberry Finn",
            Average = 46.500000m,
            Max = 64,
            Min = 29,
            Count = 2,
            Sum = 17164,
        },
        new()
        {
            Gender = 'F',
            Title = "Alice's Adventures in Wonderland",
            Average = 48.000000m,
            Max = 72,
            Min = 24,
            Count = 2,
            Sum = 49200,
        },
        new()
        {
            Gender = 'M',
            Title = "Alice's Adventures in Wonderland",
            Average = 54.400000m,
            Max = 64,
            Min = 45,
            Count = 5,
            Sum = 49775,
        },
        new()
        {
            Gender = 'F',
            Title = "Dracula",
            Average = 55.333333m,
            Max = 72,
            Min = 27,
            Count = 3,
            Sum = 53093,
        },
        new()
        {
            Gender = 'M',
            Title = "Dracula",
            Average = 57.666666m,
            Max = 64,
            Min = 45,
            Count = 3,
            Sum = 11147,
        },
        new()
        {
            Gender = 'F',
            Title = "Frankenstein",
            Average = 48.750000m,
            Max = 72,
            Min = 31,
            Count = 4,
            Sum = 70648,
        },
        new()
        {
            Gender = 'M',
            Title = "Frankenstein",
            Average = 52.333333m,
            Max = 64,
            Min = 29,
            Count = 3,
            Sum = 19658,
        },
        new()
        {
            Gender = 'F',
            Title = "Moby-Dick",
            Average = 59.500000m,
            Max = 61,
            Min = 58,
            Count = 2,
            Sum = 30485,
        },
        new()
        {
            Gender = 'M',
            Title = "Moby-Dick",
            Average = 38.000000m,
            Max = 39,
            Min = 37,
            Count = 2,
            Sum = 26300,
        },
        new()
        {
            Gender = 'F',
            Title = "Pride and Prejudice",
            Average = 61.000000m,
            Max = 61,
            Min = 61,
            Count = 1,
            Sum = 25744,
        },
        new()
        {
            Gender = 'M',
            Title = "Pride and Prejudice",
            Average = 48.000000m,
            Max = 51,
            Min = 45,
            Count = 2,
            Sum = 16507,
        },
        new()
        {
            Gender = 'F',
            Title = "The Adventures of Sherlock Holmes",
            Average = 57.000000m,
            Max = 79,
            Min = 34,
            Count = 3,
            Sum = 32677,
        },
        new()
        {
            Gender = 'M',
            Title = "The Adventures of Sherlock Holmes",
            Average = 57.666666m,
            Max = 64,
            Min = 45,
            Count = 3,
            Sum = 9714,
        },
        new()
        {
            Gender = 'F',
            Title = "The Count of Monte Cristo",
            Average = 60.000000m,
            Max = 72,
            Min = 43,
            Count = 4,
            Sum = 40882,
        },
        new()
        {
            Gender = 'M',
            Title = "The Count of Monte Cristo",
            Average = 73.500000m,
            Max = 83,
            Min = 64,
            Count = 2,
            Sum = 37372,
        },
        new()
        {
            Gender = 'F',
            Title = "The Picture of Dorian Gray",
            Average = 53.000000m,
            Max = 58,
            Min = 43,
            Count = 3,
            Sum = 25399,
        },
        new()
        {
            Gender = 'M',
            Title = "The Picture of Dorian Gray",
            Average = 30.500000m,
            Max = 39,
            Min = 22,
            Count = 2,
            Sum = 64149,
        },
        new()
        {
            Gender = 'F',
            Title = "The Time Machine",
            Average = 57.750000m,
            Max = 72,
            Min = 34,
            Count = 4,
            Sum = 35612,
        },
        new()
        {
            Gender = 'M',
            Title = "The Time Machine",
            Average = 60.000000m,
            Max = 64,
            Min = 56,
            Count = 2,
            Sum = 21253,
        },
    ];
}