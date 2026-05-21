
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Generators.SqlServer;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.Helpers;


namespace Carrigan.SqlTools.Tests.GeneratorsTests;


//IGNORE SPELLING: SHHH myschema

public class SqlGenerator_UpdateByIdTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;
    private readonly SqlGenerator<SqlTypeEntity> _sqlGeneratorForSqlTypeEntity;
    private readonly SqlGenerator<NullableTestEntity> _sqlGeneratorForNullablesTestEntity;
    private readonly SqlGenerator<EntityWithEncryption> _sqlGeneratorForEntityWithEncryption;
    private readonly SqlGenerator<CompositePrimaryKeyTable> _sqlGeneratorCompositeKeyTable;
    private readonly ColumnCollection<CompositePrimaryKeyTable> _leftCompositeKeyTable = new("NotKey1", "NotKey2");
    private readonly SqlGenerator<Address> _sqlGeneratorAddress;

    public SqlGenerator_UpdateByIdTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncrypter);
        _sqlGeneratorForSqlTypeEntity = new SqlGenerator<SqlTypeEntity>(_mockEncrypter);
        _sqlGeneratorForNullablesTestEntity = new SqlGenerator<NullableTestEntity>(_mockEncrypter);
        _sqlGeneratorForEntityWithEncryption = new SqlGenerator<EntityWithEncryption>(_mockEncrypter);
        _sqlGeneratorCompositeKeyTable = new SqlGenerator<CompositePrimaryKeyTable>(_mockEncrypter);
        _sqlGeneratorAddress = new();
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectSql_WithTableAttribute()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1, [DateOf] = @DateOf_2, [When] = @When_3 WHERE [Id] = @Id_4;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectSql_WithTableAttribute_No_Optional_Columns_Param()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1, [DateOf] = @DateOf_2, [When] = @When_3 WHERE [Id] = @Id_4;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectParameters()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_4", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Id
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Test Name"); // Name
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_2", new DateTime(2023, 10, 1)); // DateOf
        SqlQueryTestHelper.AssertParameterValue(query, "@When_3", "Now"); // When
    }

    [Fact]
    public void SqlUpdateString_ExcludesNotMappedProperties()
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

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_4", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Id
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Test Name"); // Name
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_2", new DateTime(2023, 10, 1)); // DateOf
        SqlQueryTestHelper.AssertParameterValue(query, "@When_3", "Now"); // When

        Assert.DoesNotContain("Where", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Where");
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "HideTimeFlag");
    }

    [Fact]
    public void SqlUpdateString_HandlesNullValues()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = null!, // Nullable property
            When = null, // Nullable property
            DateOf = DateTime.UtcNow
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);


        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", null!); // Name
        SqlQueryTestHelper.AssertParameterValue(query, "@When_3", null!); // When
    }

    [Fact]
    public void SqlUpdateString_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.UpdateById(entityWithoutTableAttribute, null);

        string expectedSql = "UPDATE [EntityWithoutTableAttribute] SET [Description] = @Description_1 WHERE [Id] = @Id_2;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateString_HandlesSchemaInTableAttribute()
    {
        EntityWithSchema entityWithSchema = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithSchema.UpdateById(entityWithSchema, null);

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


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1, [DateOf] = @DateOf_2, [When] = @When_3 WHERE [Id] = @Id_4;";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        Assert.DoesNotContain("Address", query.QueryText);
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address");
    }

    [Fact]
    public void SqlUpdatesString_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();


        SqlQuery query = _sqlGeneratorForSqlTypeEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [TestSqlTypes] SET [LongValue] = @LongValue_1, [ShortValue] = @ShortValue_2, [ByteValue] = @ByteValue_3, [BoolValue] = @BoolValue_4, [DecimalValue] = @DecimalValue_5, [FloatValue] = @FloatValue_6, [DoubleValue] = @DoubleValue_7, [StringValue] = @StringValue_8, [DateTimeValue] = @DateTimeValue_9, [GuidValue] = @GuidValue_10, [ByteArrayValue] = @ByteArrayValue_11, [CharValue] = @CharValue_12, [TimeOnlyValue] = @TimeOnlyValue_13, [DateOnlyValue] = @DateOnlyValue_14, [DateTimeOffsetValue] = @DateTimeOffsetValue_15 WHERE [IntValue] = @IntValue_16;";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 16);

        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_16", 42);                                               // IntValue
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_1", 1234567890L);                                      // LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_2", (short)32000);                                    // ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_3", (byte)255);                                        // ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_4", true);                                             // BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_5", 99.99m);                                        // DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_6", 3.14f);                                           // FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_7", 123.456);                                        // DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@StringValue_8", "Test String");                                  // StringValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_9", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));    // DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_10", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"));// GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_11", new byte[] { 0x01, 0x02, 0x03 });            // ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_12", 'A');                                             // CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_13", new TimeOnly(1, 2, 0));                       // TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_15", dateTimeOffsetTestValue);               // DateTimeOffsetValue
    }

    [Fact]
    public void TestSqlUpdateStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();


        SqlQuery query
            = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [NullableTestEntity] SET [IntValue] = @IntValue_1, [LongValue] = @LongValue_2, [ShortValue] = @ShortValue_3, [ByteValue] = @ByteValue_4, [BoolValue] = @BoolValue_5, [DecimalValue] = @DecimalValue_6, [FloatValue] = @FloatValue_7, [DoubleValue] = @DoubleValue_8, [DateTimeValue] = @DateTimeValue_9, [GuidValue] = @GuidValue_10, [CharValue] = @CharValue_11, [TimeOnlyValue] = @TimeOnlyValue_12, [DateOnlyValue] = @DateOnlyValue_13, [ByteArrayValue] = @ByteArrayValue_14, [DateTimeOffsetValue] = @DateTimeOffsetValue_15 WHERE [Key] = @Key_16;";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_16", Guid.Empty);                                             // Key
        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_1", (int?)1);                                           // Nullable IntValue
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_2", (long?)123456789L);                                // Nullable LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_3", (short?)123);                                     // Nullable ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_4", (byte?)255);                                       // Nullable ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_5", (bool?)true);                                      // Nullable BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_6", (decimal?)99.99m);                              // Nullable DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_7", (float?)3.14f);                                   // Nullable FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_8", (double?)1.618);                                 // Nullable DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_9", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));    // Nullable DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_10", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"));// Nullable GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_11", (char?)'A');                                      // Nullable CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_12", new TimeOnly(1, 2, 0));                       // Nullable TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_13", new DateOnly(1, 12, 25));                     // Nullable DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_14", new byte[] { 0x01, 0x02, 0x03 });            // Nullable ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_15", dateTimeOffsetTestValue);               // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void TestSqlUpdateStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();


        SqlQuery query = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [NullableTestEntity] SET [IntValue] = @IntValue_1, [LongValue] = @LongValue_2, [ShortValue] = @ShortValue_3, [ByteValue] = @ByteValue_4, [BoolValue] = @BoolValue_5, [DecimalValue] = @DecimalValue_6, [FloatValue] = @FloatValue_7, [DoubleValue] = @DoubleValue_8, [DateTimeValue] = @DateTimeValue_9, [GuidValue] = @GuidValue_10, [CharValue] = @CharValue_11, [TimeOnlyValue] = @TimeOnlyValue_12, [DateOnlyValue] = @DateOnlyValue_13, [ByteArrayValue] = @ByteArrayValue_14, [DateTimeOffsetValue] = @DateTimeOffsetValue_15 WHERE [Key] = @Key_16;";
        Assert.Equal(expectedSql, query.QueryText);


        // Assert that parameters have the correct values and are correctly mapped
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_16", Guid.Empty);            // Key
        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_1", null!);      // Nullable IntValue
        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_2", null!);     // Nullable LongValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_3", null!);    // Nullable ShortValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_4", null!);     // Nullable ByteValue
        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_5", null!);     // Nullable BoolValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_6", null!);  // Nullable DecimalValue
        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_7", null!);    // Nullable FloatValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_8", null!);   // Nullable DoubleValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_9", null!); // Nullable DateTimeValue
        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_10", null!);     // Nullable GuidValue
        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_11", null!);     // Nullable CharValue
        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_12", null!); // Nullable TimeOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_13", null!); // Nullable DateOnlyValue
        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_14", null!);    // Nullable ByteArrayValue
        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_15", null!); // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void SqlUpdateString_Encrypted()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = "Shhh..."
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.UpdateById(entity, null);

        string expectedSql = "UPDATE [Test] SET [NotSensitiveData] = @NotSensitiveData_1, [SensitiveData] = @SensitiveData_2, [KeyVersion] = @KeyVersion_3 WHERE [Id] = @Id_4;";

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_4", 1337); // Id
        SqlQueryTestHelper.AssertParameterValue(query, "@NotSensitiveData_1", "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...");
        Assert.NotEqual("Shhh...", SqlQueryTestHelper.GetParameterValue(query, "@SensitiveData_2"));
        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(SqlQueryTestHelper.GetParameterValue(query, "@SensitiveData_2")?.ToString()));
        SqlQueryTestHelper.AssertParameterValue(query, "@KeyVersion_3", 1);
    }

    [Fact]
    public void SqlUpdateString_Encrypted_Null()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = null
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.UpdateById(entity, null);

        string expectedSql = "UPDATE [Test] SET [NotSensitiveData] = @NotSensitiveData_1, [SensitiveData] = @SensitiveData_2, [KeyVersion] = @KeyVersion_3 WHERE [Id] = @Id_4;";

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_4", 1337);
        SqlQueryTestHelper.AssertParameterValue(query, "@NotSensitiveData_1", "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...");
        SqlQueryTestHelper.AssertParameterValue(query, "@SensitiveData_2", null!);
        SqlQueryTestHelper.AssertParameterValue(query, "@KeyVersion_3", 1);
    }

    [Fact]
    public void SqlUpdate_WithInnerJoin_WithJoinsAndPredicates()
    {

        CompositePrimaryKeyTable entity3 = new()
        {
            Id1 = 11,
            Id2 = 12,

            NotKey1 = 13,
            NotKey2 = 14
        };

        CompositePrimaryKeyTable entity1 = new()
        {
            Id1 = 1,
            Id2 = 2,

            NotKey1 = 3,
            NotKey2 = 4
        };

        CompositePrimaryKeyTable entity2 = new()
        {
            Id1 = 5,
            Id2 = 6,

            NotKey1 = 7,
            NotKey2 = 8
        };

        Predicates predicateId = new Equal(new Column<JoinLeftTable>("Id"), new Parameter("Id", 3));
        SqlQuery query = _sqlGeneratorCompositeKeyTable.UpdateByIds(entity3, _leftCompositeKeyTable, entity1, entity2);

        string expectedSql = "UPDATE [Ck] SET [NotKey1] = @NotKey1_1, [NotKey2] = @NotKey2_2 WHERE " +
            "((([Ck].[Id1] = @Id1_3) AND ([Ck].[Id2] = @Id2_4)) OR (([Ck].[Id1] = @Id1_5) AND ([Ck].[Id2] = @Id2_6)))";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 6);

        SqlQueryTestHelper.AssertParameterValue(query, "@NotKey1_1", 13);

        SqlQueryTestHelper.AssertParameterValue(query, "@NotKey2_2", 14);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id1_3", 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id2_4", 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id1_5", 5);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id2_6", 6);
    }

    [Fact]
    public void Throws_NoPrimaryKeyException()
    {
        Address value = new() { City = "Clarksville", PostalCode = "37043", Street = "Madison" };
        IEnumerable<Address> entities =
        [
            new() { City = "Clarksville", PostalCode = "37043", Street = "Madison" }
        ];
        Assert.Throws<NoPrimaryKeyPropertyException<Address>>(() => _sqlGeneratorAddress.UpdateById(entities.Single()));
        Assert.Throws<NoPrimaryKeyPropertyException<Address>>(() => _sqlGeneratorAddress.UpdateByIds(value, null, entities));
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectSql_WithCompositePrimaryKey_UsesAndInWhereClause()
    {
        CompositePrimaryKeyTable entity = new()
        {
            Id1 = 1,
            Id2 = 2,
            NotKey1 = 10,
            NotKey2 = 20,
            NotKey3 = 30
        };

        SqlQuery query = _sqlGeneratorCompositeKeyTable.UpdateById(entity);

        string expectedSql =
            "UPDATE [Ck] SET [NotKey1] = @NotKey1_1, [NotKey2] = @NotKey2_2, [NotKey3] = @NotKey3_3 WHERE [Id1] = @Id1_4 AND [Id2] = @Id2_5;";

        Assert.Equal(expectedSql, query.QueryText);
    }
}