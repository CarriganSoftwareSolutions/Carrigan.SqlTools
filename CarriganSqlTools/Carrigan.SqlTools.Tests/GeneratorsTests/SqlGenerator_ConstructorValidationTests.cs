using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
using Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Columns;
using Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Parameters;
using Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Table;
using Carrigan.SqlTools.Tests.TestEntities.NotExceptional;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;
public class SqlGenerator_ConstructorValidationTests
{
    [Fact]
    public void MultiKeyVersionException() =>
        Assert.Throws<MultipleKeyVersionFields<MultiKeyVersions>>(() => new SqlGenerator<MultiKeyVersions>(new MockEncryption("the")));
    [Fact]
    public void NoKeyVersionException() =>
        Assert.Throws<NoKeyVersionField<NoKeyVersionField>>(() => new SqlGenerator<NoKeyVersionField>(new MockEncryption("the")));
    [Fact]
    public void NoEncrypterVersionException() =>
        Assert.Throws<EncrypterNotProvided<EntityWithEncryption>>(() => new SqlGenerator<EntityWithEncryption>());

    [Fact]
    public void NonIntKeyVersions() =>
        Assert.Throws<InvalidKeyVersionFieldType<NonIntKeyVersions>>(() => new SqlGenerator<NonIntKeyVersions>(new MockEncryption("the")));

    [Fact]
    public void NullableIntKeyVersions() =>
        _ = new SqlGenerator<NullableIntKeyVersions>(new MockEncryption("the"));

    [Fact]
    public void ColumnNameFromNullIdentifier()
    {
        TypeInitializationException ex = 
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromNullIdentifier>());
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }
    [Fact]
    public void ColumnNameFromEmptyIdentifier()
    {
        TypeInitializationException ex = 
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromEmptyIdentifier>());
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Fact]
    public void ColumnNameFromInvalidIdentifier()
    {
        AggregateException ex =Assert.Throws<AggregateException>(() => _ = new SqlGenerator<ColumnNameFromInvalidIdentifier>());

        Assert.NotEmpty(ex.InnerExceptions.OfType<InvalidSqlIdentifierException>());
    }

    [Fact]
    public void ColumnNameFromNullAnnotation()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromNullAnnotation>());
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }
    [Fact]
    public void ColumnNameFromEmptyAnnotation()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromEmptyAnnotation>());
        Assert.IsType<ArgumentException>(ex.InnerException);
    }
    [Fact]
    public void ColumnNameFromInvalidAnnotation()
    {
        AggregateException ex = Assert.Throws<AggregateException>(() => _ = new SqlGenerator<ColumnNameFromInvalidAnnotation>());

        Assert.NotEmpty(ex.InnerExceptions.OfType<InvalidSqlIdentifierException>());
    }
    [Fact]
    public void TableNameFromNullIdentifier()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<TableNameFromNullIdentifier>());
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }
    [Fact]
    public void TableNameFromEmptyIdentifier()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<TableNameFromEmptyIdentifier>());
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Fact]
    public void TableNameFromInvalidIdentifier()
    {
        AggregateException ex = Assert.Throws<AggregateException>(() => _ = new SqlGenerator<TableNameFromInvalidIdentifier>());

        Assert.NotEmpty(ex.InnerExceptions.OfType<InvalidSqlIdentifierException>());
    }

    [Fact]
    public void TableNameFromNullAnnotation()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<TableNameFromNullAnnotation>());
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }
    [Fact]
    public void TableNameFromEmptyAnnotation()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<TableNameFromEmptyAnnotation>());
        Assert.IsType<ArgumentException>(ex.InnerException);
    }
    [Fact]
    public void TableNameFromInvalidAnnotation() => 
        Assert.Throws<InvalidSqlIdentifierException>(() => _ = new SqlGenerator<TableNameFromInvalidAnnotation>());


    [Fact]
    public void ParameterNameNull()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ParameterNameNull>());
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }
    [Fact]
    public void ParameterNameEmpty()
    {
        TypeInitializationException ex =
            Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ParameterNameEmpty>());
        Assert.IsType<ArgumentException>(ex.InnerException);
    }
    [Fact]
    public void ParameterNameInvalid() =>
        Assert.Throws<InvalidSqlIdentifierException>(() => _ = new SqlGenerator<ParameterNameInvalid>());
}
