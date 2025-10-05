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
    public void ColumnNameFromNullIdentifier() =>
        Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromNullIdentifier>());
    [Fact]
    public void ColumnNameFromEmptyIdentifier() =>
        Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromEmptyIdentifier>());
    [Fact]
    public void ColumnNameFromInvalidIdentifier() =>
        Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromInvalidIdentifier>());

    [Fact]
    public void ColumnNameFromNullAnnotation() =>
        Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromNullAnnotation>());
    [Fact]
    public void ColumnNameFromEmptyAnnotation() =>
        Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromEmptyAnnotation>());
    [Fact]
    public void ColumnNameFromInvalidAnnotation() =>
        Assert.Throws<TypeInitializationException>(() => _ = new SqlGenerator<ColumnNameFromInvalidAnnotation>());
}
