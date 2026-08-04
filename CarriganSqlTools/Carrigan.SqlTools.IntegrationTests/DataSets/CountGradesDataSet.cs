using Carrigan.SqlTools.IntegrationTests.CompositeModels;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class CountGradesDataSet
{
    public static void Validate(IEnumerable<GradeResults> actualResults) =>
        GradeResultsDataSetValidation.Validate(actualResults, Data);

    public static readonly IEnumerable<GradeResults> Data =
    [
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            AcademicYear = 2004,
            SemesterNumber = 2,
            AverageGPA = 2.300000m,
            MinGPA = 2.30m,
            MaxGPA = 2.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            AcademicYear = 2005,
            SemesterNumber = 1,
            AverageGPA = 3.150000m,
            MinGPA = 3.00m,
            MaxGPA = 3.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            AcademicYear = 2008,
            SemesterNumber = 2,
            AverageGPA = 3.350000m,
            MinGPA = 2.70m,
            MaxGPA = 4.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            AcademicYear = 2009,
            SemesterNumber = 1,
            AverageGPA = 3.500000m,
            MinGPA = 3.00m,
            MaxGPA = 4.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            AcademicYear = 2009,
            SemesterNumber = 2,
            AverageGPA = 3.500000m,
            MinGPA = 3.30m,
            MaxGPA = 3.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            AcademicYear = 2003,
            SemesterNumber = 2,
            AverageGPA = 3.150000m,
            MinGPA = 3.00m,
            MaxGPA = 3.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            AcademicYear = 2004,
            SemesterNumber = 1,
            AverageGPA = 2.200000m,
            MinGPA = 1.70m,
            MaxGPA = 2.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            AcademicYear = 2004,
            SemesterNumber = 2,
            AverageGPA = 2.300000m,
            MinGPA = 2.30m,
            MaxGPA = 2.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            AcademicYear = 2007,
            SemesterNumber = 1,
            AverageGPA = 3.350000m,
            MinGPA = 3.00m,
            MaxGPA = 3.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            AcademicYear = 2007,
            SemesterNumber = 2,
            AverageGPA = 2.800000m,
            MinGPA = 2.30m,
            MaxGPA = 3.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            AcademicYear = 2001,
            SemesterNumber = 2,
            AverageGPA = 2.000000m,
            MinGPA = 1.70m,
            MaxGPA = 2.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            AcademicYear = 2002,
            SemesterNumber = 1,
            AverageGPA = 2.500000m,
            MinGPA = 2.30m,
            MaxGPA = 2.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            AcademicYear = 2008,
            SemesterNumber = 1,
            AverageGPA = 2.500000m,
            MinGPA = 2.30m,
            MaxGPA = 2.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            AcademicYear = 2008,
            SemesterNumber = 2,
            AverageGPA = 2.350000m,
            MinGPA = 2.00m,
            MaxGPA = 2.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            AcademicYear = 2002,
            SemesterNumber = 1,
            AverageGPA = 3.150000m,
            MinGPA = 2.30m,
            MaxGPA = 4.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            AcademicYear = 2001,
            SemesterNumber = 1,
            AverageGPA = 3.650000m,
            MinGPA = 3.30m,
            MaxGPA = 4.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            AcademicYear = 2001,
            SemesterNumber = 2,
            AverageGPA = 2.500000m,
            MinGPA = 1.30m,
            MaxGPA = 3.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            AcademicYear = 2002,
            SemesterNumber = 1,
            AverageGPA = 3.150000m,
            MinGPA = 3.00m,
            MaxGPA = 3.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            AcademicYear = 2004,
            SemesterNumber = 1,
            AverageGPA = 2.850000m,
            MinGPA = 2.70m,
            MaxGPA = 3.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            AcademicYear = 2004,
            SemesterNumber = 2,
            AverageGPA = 2.500000m,
            MinGPA = 2.30m,
            MaxGPA = 2.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            AcademicYear = 2007,
            SemesterNumber = 1,
            AverageGPA = 3.150000m,
            MinGPA = 3.00m,
            MaxGPA = 3.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            AcademicYear = 2007,
            SemesterNumber = 2,
            AverageGPA = 3.150000m,
            MinGPA = 3.00m,
            MaxGPA = 3.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            AcademicYear = 2001,
            SemesterNumber = 1,
            AverageGPA = 3.000000m,
            MinGPA = 2.70m,
            MaxGPA = 3.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            AcademicYear = 2002,
            SemesterNumber = 2,
            AverageGPA = 2.500000m,
            MinGPA = 2.30m,
            MaxGPA = 2.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            AcademicYear = 2003,
            SemesterNumber = 2,
            AverageGPA = 2.500000m,
            MinGPA = 2.30m,
            MaxGPA = 2.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            AcademicYear = 2005,
            SemesterNumber = 1,
            AverageGPA = 2.650000m,
            MinGPA = 2.30m,
            MaxGPA = 3.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            AcademicYear = 2005,
            SemesterNumber = 2,
            AverageGPA = 2.850000m,
            MinGPA = 2.70m,
            MaxGPA = 3.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            AcademicYear = 2003,
            SemesterNumber = 1,
            AverageGPA = 1.700000m,
            MinGPA = 1.70m,
            MaxGPA = 1.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            AcademicYear = 2003,
            SemesterNumber = 2,
            AverageGPA = 2.150000m,
            MinGPA = 2.00m,
            MaxGPA = 2.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            AcademicYear = 2005,
            SemesterNumber = 2,
            AverageGPA = 2.300000m,
            MinGPA = 2.30m,
            MaxGPA = 2.30m,
            Count = 2,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            AcademicYear = 2006,
            SemesterNumber = 1,
            AverageGPA = 3.650000m,
            MinGPA = 3.30m,
            MaxGPA = 4.00m,
            Count = 2,
        },
    ];
}
