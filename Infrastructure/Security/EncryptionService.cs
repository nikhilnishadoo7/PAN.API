// Infrastructure/Security/EncryptionService.cs
using System.Security.Cryptography;
using System.Text;

public static class EncryptionService
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

    public static string Encrypt(string plain)
    {
        var nonce = RandomNumberGenerator.GetBytes(12);

        var plaintextBytes = Encoding.UTF8.GetBytes(plain);
        var cipher = new byte[plaintextBytes.Length];
        var tag = new byte[16]; // 128-bit tag

        using var aes = new AesGcm(Key, 16); // ✅ FIX: specify tag size

        aes.Encrypt(nonce, plaintextBytes, cipher, tag);

        return Convert.ToBase64String(nonce.Concat(tag).Concat(cipher).ToArray());
    }

    public static string Decrypt(string encrypted)
    {
        var full = Convert.FromBase64String(encrypted);

        var nonce = full.Take(12).ToArray();
        var tag = full.Skip(12).Take(16).ToArray();
        var cipher = full.Skip(28).ToArray();

        var plaintext = new byte[cipher.Length];

        using var aes = new AesGcm(Key, 16); // ✅ FIX

        aes.Decrypt(nonce, cipher, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}