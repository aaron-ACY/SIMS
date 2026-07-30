/// <summary>
/// Recreate users.csv with 3 accounts and correct password hashes.
/// Usage: dotnet run --project Tools/ReseedPasswords
/// </summary>

using System.Security.Cryptography;
using System.Globalization;

const int Iterations = 100_000;
const int SaltSize   = 16;
const int HashSize   = 32;

// ── accounts to create ──────────────────────────────────────────────────────
var accounts = new[]
{
    new { Id = 1, Username = "admin",       Email = "admin@sims.edu",    Password = "Admin123",     FirstName = "System",   LastName = "Admin",      RoleId = 1 },
    new { Id = 2, Username = "tranngoczit", Email = "user@example.com",  Password = "Ngoczit123",   FirstName = "Ngoc",     LastName = "Zit",        RoleId = 3 },
    new { Id = 3, Username = "okuzchan",    Email = "okuz@example.com",  Password = "davidokuz123", FirstName = "david",    LastName = "vu",         RoleId = 2 },
};
// ────────────────────────────────────────────────────────────────────────────

var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);

Console.WriteLine("Generating password hashes...\n");

var csvLines = new List<string>
{
    "Id, Username, Email, PasswordHash, Salt, FirstName, LastName, RoleId, IsActive, CreatedAt, UpdatedAt"
};

foreach (var account in accounts)
{
    var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
    var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
        account.Password, saltBytes, Iterations, HashAlgorithmName.SHA256, HashSize);

    var hash = Convert.ToBase64String(hashBytes);
    var salt = Convert.ToBase64String(saltBytes);

    csvLines.Add($" {account.Id}, {account.Username}, {account.Email}, {hash}, {salt}, {account.FirstName}, {account.LastName}, {account.RoleId}, True, {now}, {now}");

    Console.WriteLine($"✓ {account.Username,-15} / {account.Password,-15} (Role {account.RoleId})");
}

var csvPath = Path.GetFullPath(
    Path.Combine(Directory.GetCurrentDirectory(), "SIMS-BackEnd", "Data", "users.csv"));

await File.WriteAllLinesAsync(csvPath, csvLines);

Console.WriteLine($"\n✓ Written to: {csvPath}");
Console.WriteLine("  Restart the API and login with the new credentials.");

return 0;
