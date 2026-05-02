using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;


namespace Carrigan.SqlTools.Tests.GeneratorsTests;

//IGNORE SPELLING: Shhh myschema


public class SqlGenerator_ProcedureTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;
    private readonly SqlGenerator<SqlTypeEntity> _sqlGeneratorForSqlTypeEntity;
    private readonly SqlGenerator<NullableTestEntity> _sqlGeneratorForNullablesTestEntity;
    private readonly SqlGenerator<EntityWithEncryption> _sqlGeneratorForEntityWithEncryption;

    public SqlGenerator_ProcedureTests()
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
    public void Sql_Procedure_GeneratesCorrectSql_WithTableAttribute()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Procedure(testEntity);

        string expectedSql = "[EntityWithTableAttribute]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void Sql_Procedure_GeneratesCorrectParameters()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Procedure(testEntity);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@Id_1").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "@Name_2").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "@DateOf_3").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "@When_4").Single().Value); // When
    }

    [Fact]
    public void Sql_Procedure_ExcludesNotMappedProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            Where = "Here", // Should be excluded
            HideTimeFlag = true, // Should be excluded
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Procedure(testEntity);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@Id_1").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "@Name_2").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "@DateOf_3").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "@When_4").Single().Value); // When

        Assert.DoesNotContain("Where", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Where");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "HideTimeFlag");
    }

    [Fact]
    public void Sql_Procedure_HandlesNullValues()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = null!, // Nullable property
            When = null, // Nullable property
            DateOf = DateTime.UtcNow
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Procedure(testEntity);


        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@Name_2").Single().Value); // Name
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@When_4").Single().Value); // When
    }

    [Fact]
    public void Sql_Procedure_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.Procedure(entityWithoutTableAttribute);

        string expectedSql = "[EntityWithoutTableAttribute]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void Sql_Procedure_HandlesSchemaInTableAttribute()
    {
        EntityWithSchema entityWithSchema = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithSchema.Procedure(entityWithSchema);

        string expectedSql = "[myschema].[EntityWithSchema]";
        Assert.Equal(expectedSql, query.QueryText);
    }


    [Fact]
    public void Sql_Procedure_IgnoresClassTypeProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1),
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Procedure(testEntity);

        string expectedSql = "[EntityWithTableAttribute]";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address");
    }

    [Fact]
    public void Sql_Procedure_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();


        SqlQuery query
            = _sqlGeneratorForSqlTypeEntity.Procedure(entity);

        string expectedSql = "[SqlTypeEntity]";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(16, query.Parameters.Count);

        Assert.Equal(42, query.Parameters.Where(param => param.Key == "@IntValue_1").Single().Value);                                                // IntValue
        Assert.Equal(1234567890L, query.Parameters.Where(param => param.Key == "@LongValue_2").Single().Value);                                      // LongValue
        Assert.Equal((short)32000, query.Parameters.Where(param => param.Key == "@ShortValue_3").Single().Value);                                    // ShortValue
        Assert.Equal((byte)255, query.Parameters.Where(param => param.Key == "@ByteValue_4").Single().Value);                                        // ByteValue
        Assert.Equal(true, query.Parameters.Where(param => param.Key == "@BoolValue_5").Single().Value);                                             // BoolValue
        Assert.Equal(99.99m, query.Parameters.Where(param => param.Key == "@DecimalValue_6").Single().Value);                                        // DecimalValue
        Assert.Equal(3.14f, query.Parameters.Where(param => param.Key == "@FloatValue_7").Single().Value);                                           // FloatValue
        Assert.Equal(123.456, query.Parameters.Where(param => param.Key == "@DoubleValue_8").Single().Value);                                        // DoubleValue
        Assert.Equal("Test String", query.Parameters.Where(param => param.Key == "@StringValue_9").Single().Value);                                  // StringValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "@DateTimeValue_10").Single().Value);    // DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@GuidValue_11").Single().Value); // GuidValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "@ByteArrayValue_12").Single().Value);             // ByteArrayValue
        Assert.Equal('A', query.Parameters.Where(param => param.Key == "@CharValue_13").Single().Value);                                              // CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "@TimeOnlyValue_14").Single().Value);                        // TimeOnlyValue
        Assert.Equal(new DateOnly(1776, 7, 4), query.Parameters.Where(param => param.Key == "@DateOnlyValue_15").Single().Value);                     // DateOnlyValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_16").Single().Value);                // DateTimeOffsetValue
    }

    [Fact]
    public void Sql_Procedure_TestSqlUpdateStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();

        SqlQuery query
            = _sqlGeneratorForNullablesTestEntity.Procedure(entity);

        string expectedSql = "[NullableTestEntity]";
        Assert.Equal(expectedSql, query.QueryText);


        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "@Key_1").Single().Value);                                             // Key
        Assert.Equal((int?)1, query.Parameters.Where(param => param.Key == "@IntValue_2").Single().Value);                                           // Nullable IntValue
        Assert.Equal((long?)123456789L, query.Parameters.Where(param => param.Key == "@LongValue_3").Single().Value);                                // Nullable LongValue
        Assert.Equal((short?)123, query.Parameters.Where(param => param.Key == "@ShortValue_4").Single().Value);                                     // Nullable ShortValue
        Assert.Equal((byte?)255, query.Parameters.Where(param => param.Key == "@ByteValue_5").Single().Value);                                       // Nullable ByteValue
        Assert.Equal((bool?)true, query.Parameters.Where(param => param.Key == "@BoolValue_6").Single().Value);                                      // Nullable BoolValue
        Assert.Equal((decimal?)99.99m, query.Parameters.Where(param => param.Key == "@DecimalValue_7").Single().Value);                              // Nullable DecimalValue
        Assert.Equal((float?)3.14f, query.Parameters.Where(param => param.Key == "@FloatValue_8").Single().Value);                                   // Nullable FloatValue
        Assert.Equal((double?)1.618, query.Parameters.Where(param => param.Key == "@DoubleValue_9").Single().Value);                                 // Nullable DoubleValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "@DateTimeValue_10").Single().Value);    // Nullable DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@GuidValue_11").Single().Value); // Nullable GuidValue
        Assert.Equal((char?)'A', query.Parameters.Where(param => param.Key == "@CharValue_12").Single().Value);                                       // Nullable CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "@TimeOnlyValue_13").Single().Value);                        // Nullable TimeOnlyValue
        Assert.Equal(new DateOnly(1, 12, 25), query.Parameters.Where(param => param.Key == "@DateOnlyValue_14").Single().Value);                      // Nullable DateOnlyValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "@ByteArrayValue_15").Single().Value);             // Nullable ByteArrayValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_16").Single().Value);                // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void Sql_Procedure_TestSqlUpdateStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();


        SqlQuery query = _sqlGeneratorForNullablesTestEntity.Procedure(entity);

        string expectedSql = "[NullableTestEntity]";
        Assert.Equal(expectedSql, query.QueryText);


        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "@Key_1").Single().Value);             // Key
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@IntValue_2").Single().Value);      // Nullable IntValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@LongValue_3").Single().Value);     // Nullable LongValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ShortValue_4").Single().Value);    // Nullable ShortValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ByteValue_5").Single().Value);     // Nullable ByteValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@BoolValue_6").Single().Value);     // Nullable BoolValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DecimalValue_7").Single().Value);  // Nullable DecimalValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@FloatValue_8").Single().Value);    // Nullable FloatValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DoubleValue_9").Single().Value);   // Nullable DoubleValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateTimeValue_10").Single().Value); // Nullable DateTimeValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@GuidValue_11").Single().Value);     // Nullable GuidValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@CharValue_12").Single().Value);     // Nullable CharValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@TimeOnlyValue_13").Single().Value); // Nullable TimeOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateOnlyValue_14").Single().Value); // Nullable DateOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ByteArrayValue_15").Single().Value);    // Nullable ByteArrayValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_16").Single().Value); // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void Sql_Procedure_Encrypted()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = "Shhh..."
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.Procedure(entity);

        string expectedSql = "[EntityWithEncryption]";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(1337, query.Parameters.Where(param => param.Key == "@Id_1").Single().Value); // Id
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "@NotSensitiveData_2").Single().Value);
        Assert.NotEqual("Shhh...", query.Parameters.Where(param => param.Key == "@SensitiveData_3").Single().Value);
        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(query.Parameters.Where(param => param.Key == "@SensitiveData_3").Single().Value.ToString()));
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "@KeyVersion_4").Single().Value);
    }

    [Fact]
    public void Sql_Procedure_Encrypted_Null()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = null
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.Procedure(entity);

        string expectedSql = "[EntityWithEncryption]";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(1337, query.Parameters.Where(param => param.Key == "@Id_1").Single().Value); // Id
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "@NotSensitiveData_2").Single().Value);
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@SensitiveData_3").Single().Value);
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "@KeyVersion_4").Single().Value);
    }
}