using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
using Carrigan.SqlTools.Tests.TestEntities.NotExceptional;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;
public class SqlGenerator_ExceptionTests
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
}
