using Carrigan.Core.Attributes;
using Carrigan.SqlTools.SqlServer;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.SqlServer.Tests;

public class ClientReflectorCacheTests
{
    private sealed class TestModel
    {
        public int Included { get; set; }

        [NotMapped]
        public int Ignored { get; set; }

        [Encrypted]
        public string? Secret { get; set; }

        [Encrypted]
        [NotMapped]
        public string? SecretIgnored { get; set; }

        [KeyVersion]
        public int Version { get; set; }
    }

    [Fact]
    public void Properties_ExcludesNotMapped()
    {
        IEnumerable<PropertyInfo> properties = GetStaticEnumerableProperty("Properties");

        Assert.Contains(properties, static p => p.Name == nameof(TestModel.Included));
        Assert.Contains(properties, static p => p.Name == nameof(TestModel.Secret));
        Assert.Contains(properties, static p => p.Name == nameof(TestModel.Version));

        Assert.DoesNotContain(properties, static p => p.Name == nameof(TestModel.Ignored));
        Assert.DoesNotContain(properties, static p => p.Name == nameof(TestModel.SecretIgnored));
    }

    [Fact]
    public void EncryptedProperties_IncludesEncrypted_AndExcludesNotMapped()
    {
        IEnumerable<PropertyInfo> encrypted = GetStaticEnumerableProperty("EncryptedProperties");

        Assert.Contains(encrypted, static p => p.Name == nameof(TestModel.Secret));
        Assert.DoesNotContain(encrypted, static p => p.Name == nameof(TestModel.SecretIgnored));
    }

    [Fact]
    public void KeyVersionProperty_ReturnsKeyVersionProperty()
    {
        PropertyInfo? keyVersion = GetStaticNullableProperty("KeyVersionProperty");

        Assert.NotNull(keyVersion);
        Assert.Equal(nameof(TestModel.Version), keyVersion!.Name);
    }

    private static IEnumerable<PropertyInfo> GetStaticEnumerableProperty(string propertyName)
    {
        Type cacheType = GetClosedCacheType();
        PropertyInfo property = cacheType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Property '{propertyName}' not found.");

        object? value = property.GetValue(null);
        Assert.NotNull(value);

        return (IEnumerable<PropertyInfo>)value!;
    }

    private static PropertyInfo? GetStaticNullableProperty(string propertyName)
    {
        Type cacheType = GetClosedCacheType();
        PropertyInfo property = cacheType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Property '{propertyName}' not found.");

        return (PropertyInfo?)property.GetValue(null);
    }

    private static Type GetClosedCacheType()
    {
        Assembly sqlServerAssembly = typeof(Commands).Assembly;
        Type openGeneric = sqlServerAssembly.GetType("Carrigan.SqlTools.SqlServer.ClientReflectorCache`1")
            ?? throw new InvalidOperationException("ClientReflectorCache`1 type not found.");

        return openGeneric.MakeGenericType(typeof(TestModel));
    }
}
