namespace SIMS.Application.Interfaces.Services;

public interface IPasswordHasher
{
    /// <summary>Returns a (hash, salt) tuple for the given plain-text password.</summary>
    (string Hash, string Salt) HashPassword(string password);

    /// <summary>Returns true when the plain-text password matches the stored hash and salt.</summary>
    bool VerifyPassword(string password, string storedHash, string storedSalt);
}
