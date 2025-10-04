using Carrigan.SqlTools.Exceptions;
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
}
