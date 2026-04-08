// Infrastructure/Security/HashingService.cs
using System.Security.Cryptography;
using System.Text;

namespace PAN.API.Infrastructure.Security;

public static class HashingService
{
    public static string Hash(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}