using System.Security.Cryptography;
using SIMS.Application.Interfaces.Services;

namespace SIMS.Infrastructure.Security;

/// <summary>
/// PBKDF2-SHA256 password hasher.
/// Parameters: 100,000 iterations · 16-byte salt · 32-byte derived key.
/// These must stay in sync with the values used to generate the seed CSV data.
/// </summary>
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize   = 16;   // bytes
    private const int HashSize   = 32;   // bytes

    public (string Hash, string Salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes     = Convert.FromBase64String(storedSalt);
        var expectedBytes = Convert.FromBase64String(storedHash);
        var actualBytes   = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        // Constant-time comparison prevents timing attacks.
        return CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }
}
