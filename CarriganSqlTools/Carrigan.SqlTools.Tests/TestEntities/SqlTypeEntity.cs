using Carrigan.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SqlToolsTests.TestEntities;

[Table("TestSqlTypes")]
[Procedure("TestProcedureSqlTypes")]
public class SqlTypeEntity
{

    public static readonly DateTimeOffset DateTimeOffsetTestValue;
    static SqlTypeEntity()
    {

        DateTimeOffsetTestValue = DateTimeOffset.Now;
    }

    [Key]
    public int IntValue { get; set; }                   // SQL INT
    public long LongValue { get; set; }                 // SQL BIGINT
    public short ShortValue { get; set; }               // SQL SMALLINT
    public byte ByteValue { get; set; }                 // SQL TINYINT
    public bool BoolValue { get; set; }                 // SQL BIT
    public decimal DecimalValue { get; set; }           // SQL DECIMAL
    public float FloatValue { get; set; }               // SQL FLOAT
    public double DoubleValue { get; set; }             // SQL REAL
    public string StringValue { get; set; } = null!;    // SQL NVARCHAR, VARCHAR, TEXT
    public DateTime DateTimeValue { get; set; }         // SQL DATETIME, DATETIME2
    public Guid GuidValue { get; set; }                 // SQL UNIQUEIDENTIFIER
    public byte[] ByteArrayValue { get; set; } = null!; // SQL VARBINARY
    public char CharValue { get; set; }                 // SQL CHAR
    public TimeOnly TimeOnlyValue { get; set; }         // SQL Time
    public DateOnly DateOnlyValue { get; set; }         // SQL Date
    public DateTimeOffset DateTimeOffsetValue { get; set; }    // DateTimeOffset


    public static SqlTypeEntity GetStandardTestSet() => new()
    {
            IntValue = 42,
            LongValue = 1234567890L,
            ShortValue = 32000,
            ByteValue = 255,
            BoolValue = true,
            DecimalValue = 99.99m,
            FloatValue = 3.14f,
            DoubleValue = 123.456,
            StringValue = "Test String",
            DateTimeValue = new DateTime(2024, 11, 6, 1, 14, 1, 2, 3),
            GuidValue = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            ByteArrayValue = [0x01, 0x02, 0x03],
            CharValue = 'A',
            TimeOnlyValue = new TimeOnly(1, 2, 0),
            DateOnlyValue = new DateOnly(1776, 7, 4),
            DateTimeOffsetValue = DateTimeOffsetTestValue
        };
}


