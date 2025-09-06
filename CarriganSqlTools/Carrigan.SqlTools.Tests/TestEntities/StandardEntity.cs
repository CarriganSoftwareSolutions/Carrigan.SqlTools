namespace Carrigan.SqlTools.Tests.TestEntities;

public class StandardEntity
{
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
}
