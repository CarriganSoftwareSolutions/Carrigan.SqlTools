using Carrigan.Core.Interfaces;

namespace SqlToolsTests;

internal class MockEncryption : IEncryption
{
    private readonly string _key;
    public MockEncryption(string key)
    {
        this._key = key;
    }

    public int? Version => throw new NotImplementedException();

    public byte[] KeyBytes => throw new NotImplementedException();

    public string? Decrypt(string? cipherText)
    {
        if (cipherText is null)
            return null;
        else
            return new string([.. cipherText.ToCharArray().SkipLast(_key.Length)]);
    }

    public string? Encrypt(string? plainText) =>
        plainText is null ? null : plainText + _key;
}
