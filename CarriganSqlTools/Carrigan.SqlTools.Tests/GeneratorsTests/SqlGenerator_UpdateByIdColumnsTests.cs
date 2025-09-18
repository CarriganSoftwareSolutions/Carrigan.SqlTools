using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

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
        SetColumns<EntityWithTableAttribute> columns = new(["Name"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name WHERE [Id] = @Id;";
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
        SetColumns<EntityWithTableAttribute> columns = new(["Name"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);


        Assert.Equal(2, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "Id").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
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
        SetColumns<EntityWithTableAttribute> columns = new(["Name", "DateOf"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name, [DateOf] = @DateOf WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);


        Assert.Equal(3, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "Id").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "DateOf").Single().Value); // DateOf
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
        Assert.Throws<ArgumentException>(() => new  SetColumns<EntityWithTableAttribute>(["Name", "HideTimeFlag"]));
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
        SetColumns<EntityWithTableAttribute> columns = new(["Name", "When", "DateOf"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "When").Single().Value); // When
    }

    [Fact]
    public void SqlUpdateColumnsString_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };
        SetColumns<EntityWithoutTableAttribute> columns = new(["Description"]);

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.UpdateById(entityWithoutTableAttribute, columns);

        string expectedSql = "UPDATE [EntityWithoutTableAttribute] SET [Description] = @Description WHERE [Id] = @Id;";
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
        SetColumns<EntityWithSchema> columns = new(["Description"]);

        SqlQuery query = _sqlGeneratorForEntityWithSchema.UpdateById(entityWithSchema, columns);

        string expectedSql = "UPDATE [myschema].[EntityWithSchema] SET [Description] = @Description WHERE [Id] = @Id;";
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
        SetColumns<EntityWithTableAttribute> columns = new(["Name"]);

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, columns);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(2, query.Parameters.Count);
        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain("DateOf", query.QueryText);
        Assert.DoesNotContain("When", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "When");
    }

    [Fact]
    public void SqlUpdatesString_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();

        SetColumns<SqlTypeEntity> columns = new(["LongValue", "ShortValue", "ByteValue", "BoolValue", "DecimalValue", "FloatValue", "DoubleValue", "StringValue", "DateTimeValue", "GuidValue", "ByteArrayValue", "CharValue", "TimeOnlyValue", "DateOnlyValue", "DateTimeOffsetValue"]);

        SqlQuery query = _sqlGeneratorForSqlTypeEntity.UpdateById(entity, columns);

        string expectedSql = "UPDATE [TestSqlTypes] SET [LongValue] = @LongValue, [ShortValue] = @ShortValue, [ByteValue] = @ByteValue, [BoolValue] = @BoolValue, [DecimalValue] = @DecimalValue, [FloatValue] = @FloatValue, [DoubleValue] = @DoubleValue, [StringValue] = @StringValue, [DateTimeValue] = @DateTimeValue, [GuidValue] = @GuidValue, [ByteArrayValue] = @ByteArrayValue, [CharValue] = @CharValue, [TimeOnlyValue] = @TimeOnlyValue, [DateOnlyValue] = @DateOnlyValue, [DateTimeOffsetValue] = @DateTimeOffsetValue WHERE [IntValue] = @IntValue;";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(42, query.Parameters.Where(param => param.Key == "IntValue").Single().Value);                                                // IntValue
        Assert.Equal(1234567890L, query.Parameters.Where(param => param.Key == "LongValue").Single().Value);                                      // LongValue
        Assert.Equal((short)32000, query.Parameters.Where(param => param.Key == "ShortValue").Single().Value);                                    // ShortValue
        Assert.Equal((byte)255, query.Parameters.Where(param => param.Key == "ByteValue").Single().Value);                                        // ByteValue
        Assert.Equal(true, query.Parameters.Where(param => param.Key == "BoolValue").Single().Value);                                             // BoolValue
        Assert.Equal(99.99m, query.Parameters.Where(param => param.Key == "DecimalValue").Single().Value);                                        // DecimalValue
        Assert.Equal(3.14f, query.Parameters.Where(param => param.Key == "FloatValue").Single().Value);                                           // FloatValue
        Assert.Equal(123.456, query.Parameters.Where(param => param.Key == "DoubleValue").Single().Value);                                        // DoubleValue
        Assert.Equal("Test String", query.Parameters.Where(param => param.Key == "StringValue").Single().Value);                                  // StringValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "DateTimeValue").Single().Value);    // DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "GuidValue").Single().Value); // GuidValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "ByteArrayValue").Single().Value);             // ByteArrayValue
        Assert.Equal('A', query.Parameters.Where(param => param.Key == "CharValue").Single().Value);                                              // CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "TimeOnlyValue").Single().Value);                        // TimeOnlyValue
        Assert.Equal(new DateOnly(1776, 7, 4), query.Parameters.Where(param => param.Key == "DateOnlyValue").Single().Value);                     // DateOnlyValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value);                // DateTimeOffsetValue
    }

    [Fact]
    public void TestSqlUpdateStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();

        SetColumns<NullableTestEntity> columns = new(["LongValue", "ShortValue", "ByteValue", "BoolValue", "DecimalValue", "FloatValue", "DoubleValue", "DateTimeValue", "GuidValue", "CharValue", "TimeOnlyValue", "DateOnlyValue", "ByteArrayValue", "DateTimeOffsetValue"]);

        SqlQuery query = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, columns);

        string expectedSql = "UPDATE [NullableTestEntity] SET [LongValue] = @LongValue, [ShortValue] = @ShortValue, [ByteValue] = @ByteValue, [BoolValue] = @BoolValue, [DecimalValue] = @DecimalValue, [FloatValue] = @FloatValue, [DoubleValue] = @DoubleValue, [DateTimeValue] = @DateTimeValue, [GuidValue] = @GuidValue, [CharValue] = @CharValue, [TimeOnlyValue] = @TimeOnlyValue, [DateOnlyValue] = @DateOnlyValue, [ByteArrayValue] = @ByteArrayValue, [DateTimeOffsetValue] = @DateTimeOffsetValue WHERE [Key] = @Key;";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "Key").Single().Value);                                             // Key
        Assert.Equal((long?)123456789L, query.Parameters.Where(param => param.Key == "LongValue").Single().Value);                                // Nullable LongValue
        Assert.Equal((short?)123, query.Parameters.Where(param => param.Key == "ShortValue").Single().Value);                                     // Nullable ShortValue
        Assert.Equal((byte?)255, query.Parameters.Where(param => param.Key == "ByteValue").Single().Value);                                       // Nullable ByteValue
        Assert.Equal((bool?)true, query.Parameters.Where(param => param.Key == "BoolValue").Single().Value);                                      // Nullable BoolValue
        Assert.Equal((decimal?)99.99m, query.Parameters.Where(param => param.Key == "DecimalValue").Single().Value);                              // Nullable DecimalValue
        Assert.Equal((float?)3.14f, query.Parameters.Where(param => param.Key == "FloatValue").Single().Value);                                   // Nullable FloatValue
        Assert.Equal((double?)1.618, query.Parameters.Where(param => param.Key == "DoubleValue").Single().Value);                                 // Nullable DoubleValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "DateTimeValue").Single().Value);    // Nullable DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "GuidValue").Single().Value); // Nullable GuidValue
        Assert.Equal((char?)'A', query.Parameters.Where(param => param.Key == "CharValue").Single().Value);                                       // Nullable CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "TimeOnlyValue").Single().Value);                        // Nullable TimeOnlyValue
        Assert.Equal(new DateOnly(1, 12, 25), query.Parameters.Where(param => param.Key == "DateOnlyValue").Single().Value);                      // Nullable DateOnlyValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "ByteArrayValue").Single().Value);             // Nullable ByteArrayValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value);                // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void TestSqlUpdateStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();
        SetColumns<NullableTestEntity> columns = new(["LongValue", "ShortValue", "ByteValue", "BoolValue", "DecimalValue", "FloatValue", "DoubleValue", "DateTimeValue", "GuidValue", "CharValue", "TimeOnlyValue", "DateOnlyValue", "ByteArrayValue", "DateTimeOffsetValue"]);

        SqlQuery query = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, columns);

        string expectedSql = "UPDATE [NullableTestEntity] SET [LongValue] = @LongValue, [ShortValue] = @ShortValue, [ByteValue] = @ByteValue, [BoolValue] = @BoolValue, [DecimalValue] = @DecimalValue, [FloatValue] = @FloatValue, [DoubleValue] = @DoubleValue, [DateTimeValue] = @DateTimeValue, [GuidValue] = @GuidValue, [CharValue] = @CharValue, [TimeOnlyValue] = @TimeOnlyValue, [DateOnlyValue] = @DateOnlyValue, [ByteArrayValue] = @ByteArrayValue, [DateTimeOffsetValue] = @DateTimeOffsetValue WHERE [Key] = @Key;";
        Assert.Equal(expectedSql, query.QueryText);


        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "Key").Single().Value);                 // Key
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "LongValue").Single().Value);         // Nullable LongValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ShortValue").Single().Value);        // Nullable ShortValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ByteValue").Single().Value);         // Nullable ByteValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "BoolValue").Single().Value);         // Nullable BoolValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DecimalValue").Single().Value);      // Nullable DecimalValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "FloatValue").Single().Value);        // Nullable FloatValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DoubleValue").Single().Value);       // Nullable DoubleValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateTimeValue").Single().Value);     // Nullable DateTimeValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "GuidValue").Single().Value);         // Nullable GuidValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "CharValue").Single().Value);         // Nullable CharValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "TimeOnlyValue").Single().Value);     // Nullable TimeOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateOnlyValue").Single().Value);     // Nullable DateOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ByteArrayValue").Single().Value);    // Nullable ByteArrayValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value); // Nullable DateTimeOffsetValue
    }

}
