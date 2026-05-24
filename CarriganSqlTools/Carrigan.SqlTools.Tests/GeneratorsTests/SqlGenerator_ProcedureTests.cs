using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.SqlServer;


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

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Id
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", "Test Name");// Name
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_3", new DateTime(2023, 10, 1)); // DateOf
        SqlQueryTestHelper.AssertParameterValue(query, "@When_4", "Now"); // When
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

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Id
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", "Test Name"); // Name
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_3", new DateTime(2023, 10, 1)); // DateOf
        SqlQueryTestHelper.AssertParameterValue(query, "@When_4", "Now"); // When

        Assert.DoesNotContain("Where", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Where");
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "HideTimeFlag");
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


        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", null!);  // Name
        SqlQueryTestHelper.AssertParameterValue(query, "@When_4", null!);  // When
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

        SqlQueryTestHelper.AssertParameterCount(query, 4); 
        Assert.DoesNotContain("Address", query.QueryText);
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address"); 
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

        SqlQueryTestHelper.AssertParameterCount(query, 16); 

        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_1", 42);                                               // IntValue
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_2", 1234567890L);                                      // LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_3", (short)32000);                                     // ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_4", (byte)255);                                        // ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_5", true);                                             // BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_6", 99.99m);                                         // DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_7", 3.14f);                                            // FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_8", 123.456);                                         // DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@StringValue_9", "Test String");                                   // StringValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_10", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));    // DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_11", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_12", new byte[] { 0x01, 0x02, 0x03 });            // ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_13", 'A');                                              // CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_14", new TimeOnly(1, 2, 0));                       // TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_15", new DateOnly(1776, 7, 4));                     // DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_16", dateTimeOffsetTestValue);                // DateTimeOffsetValue
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
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_1", Guid.Empty);                                            // Key
        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_2", (int?)1);                                          // Nullable IntValue
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_3", (long?)123456789L);                               // Nullable LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_4", (short?)123);                                     // Nullable ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_5", (byte?)255);                                       // Nullable ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_6", (bool?)true);                                      // Nullable BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_7", (decimal?)99.99m);                             // Nullable DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_8", (float?)3.14f);                                   // Nullable FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_9", (double?)1.618);                                 // Nullable DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_10", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));    // Nullable DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_11", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Nullable GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_12", (char?)'A');                                      // Nullable CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_13", new TimeOnly(1, 2, 0));                        // Nullable TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_14", new DateOnly(1, 12, 25));                      // Nullable DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_15", new byte[] { 0x01, 0x02, 0x03 });             // Nullable ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_16", dateTimeOffsetTestValue);                // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void Sql_Procedure_TestSqlUpdateStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();


        SqlQuery query = _sqlGeneratorForNullablesTestEntity.Procedure(entity);

        string expectedSql = "[NullableTestEntity]";
        Assert.Equal(expectedSql, query.QueryText);


        // Assert that parameters have the correct values and are correctly mapped
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_1", Guid.Empty);             // Key
        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_2", null!);      // Nullable IntValue
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_3", null!);     // Nullable LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_4", null!);    // Nullable ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_5", null!);     // Nullable ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_6", null!);     // Nullable BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_7", null!);  // Nullable DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_8", null!);    // Nullable FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_9", null!);   // Nullable DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_10", null!); // Nullable DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_11", null!);    // Nullable GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_12", null!);     // Nullable CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_13", null!); // Nullable TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_14", null!); // Nullable DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_15", null!);    // Nullable ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_16", null!); // Nullable DateTimeOffsetValue
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

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", 1337); // Id
        SqlQueryTestHelper.AssertParameterValue(query, "@NotSensitiveData_2", "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...");
        Assert.NotEqual("Shhh...", SqlQueryTestHelper.GetParameterValue(query, "@SensitiveData_3"));
        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(SqlQueryTestHelper.GetParameterValue(query, "@SensitiveData_3")?.ToString()));
        SqlQueryTestHelper.AssertParameterValue(query, "@KeyVersion_4", 1);
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

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", 1337);// Id
        SqlQueryTestHelper.AssertParameterValue(query, "@NotSensitiveData_2", "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...");
        SqlQueryTestHelper.AssertParameterValue(query, "@SensitiveData_3", null!);
        SqlQueryTestHelper.AssertParameterValue(query, "@KeyVersion_4", 1);
    }
}