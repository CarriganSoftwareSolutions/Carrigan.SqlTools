using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.IntegrationTests.Models;

public sealed class Grades
{
    [PrimaryKey]
    public Guid StudentId { get; set; }

    [PrimaryKey]
    public string CourseCode { get; set; } = string.Empty;

    [PrimaryKey]
    public int AcademicYear { get; set; }

    [PrimaryKey]
    public int SemesterNumber { get; set; }

    public decimal GradePoint { get; set; }
    public int CreditHours { get; set; }

    public static string CreateTableSqlServer =>
        """
        CREATE TABLE [Grades]
        (
            [StudentId] UNIQUEIDENTIFIER NOT NULL,
            [CourseCode] NVARCHAR(10) NOT NULL,
            [AcademicYear] INT NOT NULL,
            [SemesterNumber] INT NOT NULL,
            [GradePoint] DECIMAL(3,2) NOT NULL,
            [CreditHours] INT NOT NULL,
            CONSTRAINT [PK_Grades] PRIMARY KEY
            (
                [StudentId],
                [CourseCode],
                [AcademicYear],
                [SemesterNumber]
            ),
            CONSTRAINT [CK_Grades_AcademicYear] CHECK ([AcademicYear] BETWEEN 2000 AND 2010),
            CONSTRAINT [CK_Grades_SemesterNumber] CHECK ([SemesterNumber] IN (1, 2, 3)),
            CONSTRAINT [CK_Grades_GradePoint] CHECK ([GradePoint] BETWEEN 0.00 AND 4.00),
            CONSTRAINT [CK_Grades_CreditHours] CHECK ([CreditHours] BETWEEN 3 AND 4)
        );
        """;

    public static string CreateTablePostgreSql =>
        """
        CREATE TABLE "Grades"
        (
            "StudentId" UUID NOT NULL,
            "CourseCode" VARCHAR(10) NOT NULL,
            "AcademicYear" INTEGER NOT NULL,
            "SemesterNumber" INTEGER NOT NULL,
            "GradePoint" NUMERIC(3,2) NOT NULL,
            "CreditHours" INTEGER NOT NULL,
            CONSTRAINT "PK_Grades" PRIMARY KEY
            (
                "StudentId",
                "CourseCode",
                "AcademicYear",
                "SemesterNumber"
            ),
            CONSTRAINT "CK_Grades_AcademicYear" CHECK ("AcademicYear" BETWEEN 2000 AND 2010),
            CONSTRAINT "CK_Grades_SemesterNumber" CHECK ("SemesterNumber" IN (1, 2, 3)),
            CONSTRAINT "CK_Grades_GradePoint" CHECK ("GradePoint" BETWEEN 0.00 AND 4.00),
            CONSTRAINT "CK_Grades_CreditHours" CHECK ("CreditHours" BETWEEN 3 AND 4)
        );
        """;
}
