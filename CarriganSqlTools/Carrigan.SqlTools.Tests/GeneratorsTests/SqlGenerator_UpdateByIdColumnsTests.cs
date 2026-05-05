using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.Helpers;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

//IGNORE SPELLING: myschema

public class SqlGenerator_UpdateByIdColumnsTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;
    private readonly SqlGenerator<SqlTypeEntity> _sqlGeneratorForSqlTypeEntity;
    private readonly SqlGenerator<NullableTestEntity> _sqlGeneratorForNullablesTestEntity;
    private readonly SqlGenerator<EntityWithEncryption> _sqlGeneratorForEntityWithEncryption;

    public SqlGenerator_UpdateByIdColumnsTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncrypter);
        _sqlGeneratorForSqlTypeEntity = new SqlGenerator<SqlTypeEntity>(_mockEncrypter);
        _sqlGeneratorForNullablesTestEntity = new SqlGenerator<NullableTestEntity>(_mockEncrypter);
        _sqlGeneratorForEntityWithEncryption = new SqlGenerator<EntityWithEncryption>(_mockEncrypter);
    }

    [Fact]
    public void SqlUpdateColumnsString_GeneratesCorrectSql_WithTableAttribute()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };
        ColumnCollection<EntityWithTableAttribute> columns = new(["Name"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1 WHERE [Id] = @Id_2;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateColumnsString_GeneratesCorrectParameters()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };
        ColumnCollection<EntityWithTableAttribute> columns = new(["Name"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1 WHERE [Id] = @Id_2;";
        Assert.Equal(expectedSql, query.QueryText);


        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_2", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"));// Id
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Test Name"); // Name
    }

    [Fact]
    public void SqlUpdateColumnsString_2_GeneratesCorrectSqlAndParameters_WithTableAttribute()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };
        ColumnCollection<EntityWithTableAttribute> columns = new(["Name", "DateOf"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1, [DateOf] = @DateOf_2 WHERE [Id] = @Id_3;";
        Assert.Equal(expectedSql, query.QueryText);


        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Id
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Test Name"); // Name
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_2", new DateTime(2023, 10, 1)); // DateOf
    }

    [Fact]
    public void SqlUpdateColumnsString_NotMappedProperties_Throws()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            Where = "Here", // Should be excluded
            HideTimeFlag = true, // Should be excluded
            When = "Now",
            DateOf = DateTime.UtcNow
        };
        Assert.Throws<InvalidPropertyException<EntityWithTableAttribute>>(() => new ColumnCollection<EntityWithTableAttribute>(["Name", "HideTimeFlag"]));
    }

    [Fact]
    public void SqlUpdateColumnsString_HandlesNullValues()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = null!, // Nullable property
            When = null, // Nullable property
            DateOf = DateTime.UtcNow
        };
        ColumnCollection<EntityWithTableAttribute> columns = new(["Name", "When", "DateOf"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", null!);
        SqlQueryTestHelper.AssertParameterValue(query, "@When_2", null!);
    }

    [Fact]
    public void SqlUpdateColumnsString_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };
        ColumnCollection<EntityWithoutTableAttribute> columns = new(["Description"]);

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.UpdateById(entityWithoutTableAttribute, columns);

        string expectedSql = "UPDATE [EntityWithoutTableAttribute] SET [Description] = @Description_1 WHERE [Id] = @Id_2;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateColumnsString_HandlesSchemaInTableAttribute()
    {
        EntityWithSchema entityWithSchema = new()
        {
            Id = 1,
            Description = "Test Description"
        };
        ColumnCollection<EntityWithSchema> columns = new(["Description"]);

        SqlQuery query = _sqlGeneratorForEntityWithSchema.UpdateById(entityWithSchema, columns);

        string expectedSql = "UPDATE [myschema].[EntityWithSchema] SET [Description] = @Description_1 WHERE [Id] = @Id_2;";
        Assert.Equal(expectedSql, query.QueryText);
    }


    [Fact]
    public void SqlUpdateString_IgnoresClassTypeProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1),
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };
        ColumnCollection<EntityWithTableAttribute> columns = new(["Name"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1 WHERE [Id] = @Id_2;";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 2);
        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain("DateOf", query.QueryText);
        Assert.DoesNotContain("When", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address");
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "DateOf");
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "When");
    }

    [Fact]
    public void SqlUpdatesString_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();

        ColumnCollection<SqlTypeEntity> columns = new(["LongValue", "ShortValue", "ByteValue", "BoolValue", "DecimalValue", "FloatValue", "DoubleValue", "StringValue", "DateTimeValue", "GuidValue", "ByteArrayValue", "CharValue", "TimeOnlyValue", "DateOnlyValue", "DateTimeOffsetValue"]);

        SqlQuery query = _sqlGeneratorForSqlTypeEntity.UpdateById(entity, columns);

        string expectedSql = "UPDATE [TestSqlTypes] SET [LongValue] = @LongValue_1, [ShortValue] = @ShortValue_2, [ByteValue] = @ByteValue_3, [BoolValue] = @BoolValue_4, [DecimalValue] = @DecimalValue_5, [FloatValue] = @FloatValue_6, [DoubleValue] = @DoubleValue_7, [StringValue] = @StringValue_8, [DateTimeValue] = @DateTimeValue_9, [GuidValue] = @GuidValue_10, [ByteArrayValue] = @ByteArrayValue_11, [CharValue] = @CharValue_12, [TimeOnlyValue] = @TimeOnlyValue_13, [DateOnlyValue] = @DateOnlyValue_14, [DateTimeOffsetValue] = @DateTimeOffsetValue_15 WHERE [IntValue] = @IntValue_16;";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_16", 42);                                             // IntValue
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_1", 1234567890L);                                     // LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_2", (short)32000);;                                    // ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_3", (byte)255);                                      // ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_4", true);                                           // BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_5", 99.99m);                                        // DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_6", 3.14f);                                         // FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_7", 123.456);                                       // DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@StringValue_8", "Test String");                                  // StringValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_9", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));  // DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_10", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_11", new byte[] { 0x01, 0x02, 0x03 });            // ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_12", 'A');                                             // CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_13", new TimeOnly(1, 2, 0));                       // TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_14", new DateOnly(1776, 7, 4));                     // DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_15", dateTimeOffsetTestValue);               // DateTimeOffsetValue
    }

    [Fact]
    public void TestSqlUpdateStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();

        ColumnCollection<NullableTestEntity> columns = new(["LongValue", "ShortValue", "ByteValue", "BoolValue", "DecimalValue", "FloatValue", "DoubleValue", "DateTimeValue", "GuidValue", "CharValue", "TimeOnlyValue", "DateOnlyValue", "ByteArrayValue", "DateTimeOffsetValue"]);

        SqlQuery query = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, columns);

        string expectedSql = "UPDATE [NullableTestEntity] SET [LongValue] = @LongValue_1, [ShortValue] = @ShortValue_2, [ByteValue] = @ByteValue_3, [BoolValue] = @BoolValue_4, [DecimalValue] = @DecimalValue_5, [FloatValue] = @FloatValue_6, [DoubleValue] = @DoubleValue_7, [DateTimeValue] = @DateTimeValue_8, [GuidValue] = @GuidValue_9, [CharValue] = @CharValue_10, [TimeOnlyValue] = @TimeOnlyValue_11, [DateOnlyValue] = @DateOnlyValue_12, [ByteArrayValue] = @ByteArrayValue_13, [DateTimeOffsetValue] = @DateTimeOffsetValue_14 WHERE [Key] = @Key_15;";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_15", Guid.Empty);                                             // Key
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_1", (long?)123456789L);                              // Nullable LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_2", (short?)123);                                     // Nullable ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_3", (byte?)255);                                       // Nullable ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_4", (bool?)true);                                     // Nullable BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_5", (decimal?)99.99m);                              // Nullable DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_6", (float?)3.14f);                                  // Nullable FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_7", (double?)1.618);                                // Nullable DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_8", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));   // Nullable DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_9", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Nullable GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_10", (char?)'A');                                       // Nullable CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_11", new TimeOnly(1, 2, 0));                      // Nullable TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_12", new DateOnly(1, 12, 25));                      // Nullable DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_13", new byte[] { 0x01, 0x02, 0x03 });            // Nullable ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_14", dateTimeOffsetTestValue);                // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void TestSqlUpdateStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();
        ColumnCollection<NullableTestEntity> columns = new(["LongValue", "ShortValue", "ByteValue", "BoolValue", "DecimalValue", "FloatValue", "DoubleValue", "DateTimeValue", "GuidValue", "CharValue", "TimeOnlyValue", "DateOnlyValue", "ByteArrayValue", "DateTimeOffsetValue"]);

        SqlQuery query = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, columns);

        string expectedSql = "UPDATE [NullableTestEntity] SET [LongValue] = @LongValue_1, [ShortValue] = @ShortValue_2, [ByteValue] = @ByteValue_3, [BoolValue] = @BoolValue_4, [DecimalValue] = @DecimalValue_5, [FloatValue] = @FloatValue_6, [DoubleValue] = @DoubleValue_7, [DateTimeValue] = @DateTimeValue_8, [GuidValue] = @GuidValue_9, [CharValue] = @CharValue_10, [TimeOnlyValue] = @TimeOnlyValue_11, [DateOnlyValue] = @DateOnlyValue_12, [ByteArrayValue] = @ByteArrayValue_13, [DateTimeOffsetValue] = @DateTimeOffsetValue_14 WHERE [Key] = @Key_15;";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_15", Guid.Empty);                      // Key
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_1", null!);               // Nullable LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_2", null!);              // Nullable ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_3", null!);               // Nullable ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_4", null!);               // Nullable BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_5", null!);            // Nullable DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_6", null!);              // Nullable FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_7", null!);             // Nullable DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_8", null!);           // Nullable DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_9", null!);               // Nullable GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_10", null!);              // Nullable CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_11", null!);          // Nullable TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_12", null!);          // Nullable DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_13", null!);         // Nullable ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_14", null!);    // Nullable DateTimeOffsetValue
    }

}