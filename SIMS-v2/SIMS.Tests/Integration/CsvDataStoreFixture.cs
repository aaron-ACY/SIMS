using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIMS.Infrastructure;

namespace SIMS.Tests.Integration;

public sealed class CsvDataStoreFixture : IDisposable
{
    // Header lấy đúng theo file seed trong SIMS-BackEnd/Data.
    public const string UsersHeader =
        "Id,Username,Email,PasswordHash,Salt,FirstName,LastName,RoleId,IsActive,CreatedAt,UpdatedAt";

    public const string StudentsHeader =
        "Id,UserId,StudentCode,FirstName,LastName,Email,DateOfBirth,Gender,Phone," +
        "Address,Major,EnrollmentYear,Status,IsActive,CreatedAt,UpdatedAt";

    private readonly ServiceProvider _provider;

    /// <summary>Thư mục tạm chứa CSV của riêng test này.</summary>
    public string DataDirectory { get; }

    public CsvDataStoreFixture()
    {
        DataDirectory = Path.Combine(
            Path.GetTempPath(), "sims-itest-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DataDirectory);

        // Ghi đè DataStore:BasePath bằng đường dẫn tuyệt đối tới thư mục tạm.
        // DataStoreSettings.ResolvePath() sẽ dùng thẳng nó vì đã rooted.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DataStore:BasePath"] = DataDirectory
            })
            .Build();

        _provider = new ServiceCollection()
            .AddInfrastructure(configuration, contentRootPath: DataDirectory)
            .BuildServiceProvider();
    }

    /// <summary>Lấy service từ container thật của project.</summary>
    public T GetService<T>() where T : notnull => _provider.GetRequiredService<T>();

    public string PathFor(string fileName) => Path.Combine(DataDirectory, fileName);

    public bool FileExists(string fileName) => File.Exists(PathFor(fileName));

    /// <summary>Đọc thô nội dung file CSV để assert trên bytes đã nằm trên đĩa.</summary>
    public string ReadRawCsv(string fileName) => File.ReadAllText(PathFor(fileName));

    public void SeedUsers(params string[] rows) => Seed("users.csv", UsersHeader, rows);

    public void SeedStudents(params string[] rows) => Seed("students.csv", StudentsHeader, rows);

    private void Seed(string fileName, string header, IEnumerable<string> rows) =>
        File.WriteAllLines(PathFor(fileName), new[] { header }.Concat(rows));

    public void Dispose()
    {
        _provider.Dispose();

        try
        {
            if (Directory.Exists(DataDirectory))
                Directory.Delete(DataDirectory, recursive: true);
        }
        catch (IOException)
        {
            // Thư mục tạm sót lại không làm fail test; OS sẽ dọn sau.
        }
    }
}
