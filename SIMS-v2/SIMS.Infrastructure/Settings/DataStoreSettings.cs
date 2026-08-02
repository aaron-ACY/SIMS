namespace SIMS.Infrastructure.Settings;

public class DataStoreSettings
{
    public const string SectionName = "DataStore";

    /// <summary>
    /// Path to the folder containing CSV files.
    /// Relative paths are resolved against AppContext.BaseDirectory at runtime.
    /// </summary>
    public string BasePath { get; set; } = "Data";

    public string ResolvePath(string fileName)
    {
        var basePath = Path.IsPathRooted(BasePath)
            ? BasePath
            : Path.Combine(AppContext.BaseDirectory, BasePath);

        return Path.Combine(basePath, fileName);
    }
}
