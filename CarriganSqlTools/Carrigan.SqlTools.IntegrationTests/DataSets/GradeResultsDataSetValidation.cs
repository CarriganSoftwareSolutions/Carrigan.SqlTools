using Carrigan.SqlTools.IntegrationTests.CompositeModels;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

internal static class GradeResultsDataSetValidation
{
    public static void Validate(
        IEnumerable<GradeResults> actualResults,
        IEnumerable<GradeResults> expectedResults)
    {
        ArgumentNullException.ThrowIfNull(actualResults);
        ArgumentNullException.ThrowIfNull(expectedResults);

        List<GradeResults> actualData = [.. actualResults];
        List<GradeResults> expectedData = [.. expectedResults];

        Assert.Equal(expectedData.Count, actualData.Count);

        foreach (GradeResults expected in expectedData)
        {
            List<GradeResults> matches =
            [
                .. actualData.Where
                (
                    result =>
                        result.StudentId == expected.StudentId &&
                        result.AcademicYear == expected.AcademicYear &&
                        result.SemesterNumber == expected.SemesterNumber
                )
            ];

            Assert.True
            (
                matches.Count > 0,
                $"No grade result was found for StudentId '{expected.StudentId}', " +
                $"AcademicYear '{expected.AcademicYear}', and SemesterNumber '{expected.SemesterNumber}'."
            );

            Assert.True
            (
                matches.Count == 1,
                $"Expected one grade result for StudentId '{expected.StudentId}', " +
                $"AcademicYear '{expected.AcademicYear}', and SemesterNumber '{expected.SemesterNumber}', " +
                $"but found {matches.Count}."
            );

            Validate(matches[0], expected);
        }
    }

    private static void Validate(GradeResults actual, GradeResults expected)
    {
        Assert.Equal(expected.StudentId, actual.StudentId);
        Assert.Equal(expected.AcademicYear, actual.AcademicYear);
        Assert.Equal(expected.SemesterNumber, actual.SemesterNumber);
        Assert.Equal(expected.AverageGPA, actual.AverageGPA, 6);
        Assert.Equal(expected.MinGPA, actual.MinGPA, 2);
        Assert.Equal(expected.MaxGPA, actual.MaxGPA, 2);
        Assert.Equal(expected.Count, actual.Count);
    }
}
