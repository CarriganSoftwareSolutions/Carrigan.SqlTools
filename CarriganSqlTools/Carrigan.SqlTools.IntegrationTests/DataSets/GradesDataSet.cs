using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class GradesDataSet
{
    public static void Validate(Grades actual, Grades expected)
    {
        ArgumentNullException.ThrowIfNull(actual);
        ArgumentNullException.ThrowIfNull(expected);

        Assert.Equal(expected.StudentId, actual.StudentId);
        Assert.Equal(expected.CourseCode, actual.CourseCode);
        Assert.Equal(expected.AcademicYear, actual.AcademicYear);
        Assert.Equal(expected.SemesterNumber, actual.SemesterNumber);
        Assert.Equal(expected.GradePoint, actual.GradePoint);
        Assert.Equal(expected.CreditHours, actual.CreditHours);
    }

    public static readonly IEnumerable<Grades> Data =
    [
        // Student 1: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI204",
            AcademicYear = 2000,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI211",
            AcademicYear = 2000,
            SemesterNumber = 2,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI220",
            AcademicYear = 2000,
            SemesterNumber = 3,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI242",
            AcademicYear = 2001,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI312",
            AcademicYear = 2001,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI322",
            AcademicYear = 2001,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI340",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            CourseCode = "SI411",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        // Student 2: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI204",
            AcademicYear = 2001,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI211",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI220",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI242",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI312",
            AcademicYear = 2002,
            SemesterNumber = 3,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI322",
            AcademicYear = 2003,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI340",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d2977696-5928-4efc-a6b2-6f68b291c9da"),
            CourseCode = "SI411",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        // Student 3: Random withdrawal.
        new()
        {
            StudentId = new("e655580d-d1ba-4be5-abe0-a18956bb7a9d"),
            CourseCode = "SI204",
            AcademicYear = 2009,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("e655580d-d1ba-4be5-abe0-a18956bb7a9d"),
            CourseCode = "SI211",
            AcademicYear = 2009,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        // Student 4: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI204",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 3.00m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI211",
            AcademicYear = 2003,
            SemesterNumber = 3,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI220",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI242",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI312",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI322",
            AcademicYear = 2004,
            SemesterNumber = 3,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI340",
            AcademicYear = 2005,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("2597eae3-cb0d-448e-9c7a-087d7473b654"),
            CourseCode = "SI411",
            AcademicYear = 2005,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        // Student 5: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI204",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 1.30m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI204",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI211",
            AcademicYear = 2002,
            SemesterNumber = 3,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI220",
            AcademicYear = 2003,
            SemesterNumber = 1,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI242",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI312",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI322",
            AcademicYear = 2003,
            SemesterNumber = 3,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI340",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            CourseCode = "SI411",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        // Student 6: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI204",
            AcademicYear = 2000,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI211",
            AcademicYear = 2000,
            SemesterNumber = 3,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI220",
            AcademicYear = 2001,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI242",
            AcademicYear = 2001,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI312",
            AcademicYear = 2001,
            SemesterNumber = 2,
            GradePoint = 1.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI322",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI340",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("9f1e8ee6-2a22-463d-aa0e-6548a7376f4b"),
            CourseCode = "SI411",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 2.00m,
            CreditHours = 3,
        },
        // Student 7: Introductory-course withdrawal after failure.
        new()
        {
            StudentId = new("11ff6f85-8a27-4a30-b48a-2c2181c884c4"),
            CourseCode = "SI204",
            AcademicYear = 2010,
            SemesterNumber = 2,
            GradePoint = 1.30m,
            CreditHours = 4,
        },
        // Student 8: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI204",
            AcademicYear = 2006,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI211",
            AcademicYear = 2006,
            SemesterNumber = 2,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI220",
            AcademicYear = 2007,
            SemesterNumber = 1,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI242",
            AcademicYear = 2007,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI312",
            AcademicYear = 2007,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI322",
            AcademicYear = 2007,
            SemesterNumber = 2,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI340",
            AcademicYear = 2008,
            SemesterNumber = 1,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            CourseCode = "SI411",
            AcademicYear = 2008,
            SemesterNumber = 2,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        // Student 9: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI204",
            AcademicYear = 2000,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI211",
            AcademicYear = 2000,
            SemesterNumber = 3,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI220",
            AcademicYear = 2001,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI242",
            AcademicYear = 2001,
            SemesterNumber = 1,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI312",
            AcademicYear = 2001,
            SemesterNumber = 2,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI322",
            AcademicYear = 2001,
            SemesterNumber = 2,
            GradePoint = 1.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI322",
            AcademicYear = 2001,
            SemesterNumber = 3,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI340",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            CourseCode = "SI411",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        // Student 10: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI204",
            AcademicYear = 2006,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI211",
            AcademicYear = 2006,
            SemesterNumber = 2,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI220",
            AcademicYear = 2007,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI242",
            AcademicYear = 2007,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI312",
            AcademicYear = 2007,
            SemesterNumber = 2,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI322",
            AcademicYear = 2007,
            SemesterNumber = 2,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI340",
            AcademicYear = 2007,
            SemesterNumber = 3,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            CourseCode = "SI411",
            AcademicYear = 2008,
            SemesterNumber = 1,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        // Student 11: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI204",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 1.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI211",
            AcademicYear = 2003,
            SemesterNumber = 1,
            GradePoint = 2.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI220",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI242",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 1.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI312",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI322",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI340",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI411",
            AcademicYear = 2004,
            SemesterNumber = 3,
            GradePoint = 0.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            CourseCode = "SI411",
            AcademicYear = 2005,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        // Student 12: Department GPA below C- after three courses.
        new()
        {
            StudentId = new("e7c667bd-641c-46f5-a7e5-415597197313"),
            CourseCode = "SI204",
            AcademicYear = 2008,
            SemesterNumber = 3,
            GradePoint = 1.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("e7c667bd-641c-46f5-a7e5-415597197313"),
            CourseCode = "SI211",
            AcademicYear = 2009,
            SemesterNumber = 1,
            GradePoint = 1.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e7c667bd-641c-46f5-a7e5-415597197313"),
            CourseCode = "SI220",
            AcademicYear = 2009,
            SemesterNumber = 2,
            GradePoint = 1.30m,
            CreditHours = 3,
        },
        // Student 13: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI204",
            AcademicYear = 2007,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI211",
            AcademicYear = 2007,
            SemesterNumber = 2,
            GradePoint = 2.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI220",
            AcademicYear = 2008,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI242",
            AcademicYear = 2008,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI312",
            AcademicYear = 2008,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI322",
            AcademicYear = 2008,
            SemesterNumber = 2,
            GradePoint = 2.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI340",
            AcademicYear = 2008,
            SemesterNumber = 3,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ab34f62f-cf6f-4928-a904-eafb4f14fd4f"),
            CourseCode = "SI411",
            AcademicYear = 2009,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        // Student 14: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI204",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI211",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 1.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI211",
            AcademicYear = 2004,
            SemesterNumber = 3,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI220",
            AcademicYear = 2005,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI242",
            AcademicYear = 2005,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI312",
            AcademicYear = 2005,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI322",
            AcademicYear = 2005,
            SemesterNumber = 3,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI340",
            AcademicYear = 2006,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            CourseCode = "SI411",
            AcademicYear = 2006,
            SemesterNumber = 1,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        // Student 15: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI204",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 1.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI211",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI220",
            AcademicYear = 2002,
            SemesterNumber = 3,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI242",
            AcademicYear = 2003,
            SemesterNumber = 1,
            GradePoint = 1.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI312",
            AcademicYear = 2003,
            SemesterNumber = 1,
            GradePoint = 1.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI322",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI340",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 2.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI411",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 1.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            CourseCode = "SI411",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 1.70m,
            CreditHours = 3,
        },
        // Student 16: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI204",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI211",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI220",
            AcademicYear = 2005,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI242",
            AcademicYear = 2005,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI312",
            AcademicYear = 2005,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI322",
            AcademicYear = 2005,
            SemesterNumber = 2,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI340",
            AcademicYear = 2005,
            SemesterNumber = 3,
            GradePoint = 1.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI340",
            AcademicYear = 2006,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            CourseCode = "SI411",
            AcademicYear = 2006,
            SemesterNumber = 2,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        // Student 17: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI204",
            AcademicYear = 2001,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI211",
            AcademicYear = 2001,
            SemesterNumber = 2,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI220",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI242",
            AcademicYear = 2002,
            SemesterNumber = 1,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI312",
            AcademicYear = 2002,
            SemesterNumber = 2,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI322",
            AcademicYear = 2003,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI340",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            CourseCode = "SI411",
            AcademicYear = 2003,
            SemesterNumber = 3,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        // Student 18: Three unsuccessful attempts at SI312.
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            CourseCode = "SI204",
            AcademicYear = 2006,
            SemesterNumber = 2,
            GradePoint = 3.00m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            CourseCode = "SI211",
            AcademicYear = 2006,
            SemesterNumber = 3,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            CourseCode = "SI220",
            AcademicYear = 2007,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            CourseCode = "SI242",
            AcademicYear = 2007,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            CourseCode = "SI312",
            AcademicYear = 2007,
            SemesterNumber = 3,
            GradePoint = 1.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            CourseCode = "SI312",
            AcademicYear = 2008,
            SemesterNumber = 1,
            GradePoint = 1.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            CourseCode = "SI312",
            AcademicYear = 2008,
            SemesterNumber = 2,
            GradePoint = 0.70m,
            CreditHours = 3,
        },
        // Student 19: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI204",
            AcademicYear = 2003,
            SemesterNumber = 1,
            GradePoint = 3.30m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI211",
            AcademicYear = 2003,
            SemesterNumber = 2,
            GradePoint = 2.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI220",
            AcademicYear = 2003,
            SemesterNumber = 3,
            GradePoint = 2.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI242",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI312",
            AcademicYear = 2004,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI322",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI340",
            AcademicYear = 2004,
            SemesterNumber = 2,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("b58593aa-853d-452a-bc0b-1c63b2ec844a"),
            CourseCode = "SI411",
            AcademicYear = 2004,
            SemesterNumber = 3,
            GradePoint = 2.30m,
            CreditHours = 3,
        },
        // Student 20: Completed the 24-credit-hour major requirement.
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI204",
            AcademicYear = 2007,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 4,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI211",
            AcademicYear = 2008,
            SemesterNumber = 1,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI220",
            AcademicYear = 2008,
            SemesterNumber = 2,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI242",
            AcademicYear = 2008,
            SemesterNumber = 2,
            GradePoint = 2.70m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI312",
            AcademicYear = 2009,
            SemesterNumber = 1,
            GradePoint = 3.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI322",
            AcademicYear = 2009,
            SemesterNumber = 1,
            GradePoint = 4.00m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI340",
            AcademicYear = 2009,
            SemesterNumber = 2,
            GradePoint = 3.30m,
            CreditHours = 3,
        },
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            CourseCode = "SI411",
            AcademicYear = 2009,
            SemesterNumber = 2,
            GradePoint = 3.70m,
            CreditHours = 3,
        },
    ];
}

/*
Footer - Dataset Methodology
This deterministic synthetic dataset contains 20 Computer Science students and 147 department-course attempts. 
Exactly four students (20 percent) leave before completing the major: one random withdrawal, one withdrawal after failing an introductory course, 
one department-GPA washout after three courses, and one washout after three unsuccessful attempts at the same course. 
The remaining students stop after earning at least 24 successful department credit hours, and no records are generated after completion or withdrawal.

Student identifiers were generated once from a seeded pseudorandom process and are unique.
Starting academic years and semesters were also seeded, with every start semester represented and all records constrained to academic years 2000 through 2010. 
Fall and spring terms contain at least one course while a student remains active; summer enrollment is optional.

Grade points use the 4.00 plus/minus scale (4.00, 3.70, 3.30, 3.00, 2.70, 2.30, 2.00, 1.70, 1.30, 1.00, 0.70, and 0.00). 
Values were drawn around student-specific normally distributed ability levels, quantized to the grade scale,
and then adjusted only where necessary to enforce the stated completion, retry, withdrawal, and washout rules. A grade below 1.70 earns no major credit.

Course identifiers and semester-credit totals were adapted from the United States Naval Academy Computer Science course listing,
an official U.S. Department of the Navy source. 
Under 17 U.S.C. § 105, works created by federal officers or employees as part of their official duties generally are not protected by U.S. copyright
and are considered public domain in the United States. :
https://www.usna.edu/Academics/Majors-and-Courses/course-description/hold/SI.htm
Accessed 2026-08-03. The dataset uses SI204 at four semester hours and SI211, SI220, SI242, SI312, SI322, SI340, and SI411 at three semester hours each. 
Only the factual course identifiers and credit totals were used; every student, enrollment, term, and grade value is synthetic and randomly generated.
Course information may contain transcription or copying errors. This test dataset should not, in any way, be construed as an official document of any government entity.
*/
