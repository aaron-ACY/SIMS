using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;

namespace SIMS.Infrastructure.Persistence.Base;

/// <summary>
/// Thread-safe base class for CSV-backed repositories.
/// All reads and writes are guarded by a per-file semaphore so concurrent
/// HTTP requests cannot corrupt the file.
/// </summary>
public abstract class CsvRepositoryBase<T> where T : class
{
    /// <summary>
    /// UTC round-trip formats. The first is used when writing; both are accepted
    /// when reading so the hand-written seed files ("2026-01-01T00:00:00Z", no
    /// fractional seconds) parse alongside files this class has rewritten.
    /// </summary>
    private static readonly string[] DateTimeFormats =
        ["yyyy-MM-ddTHH:mm:ss.fffffffZ", "yyyy-MM-ddTHH:mm:ssZ"];

    private readonly string _filePath;

    // One semaphore per concrete instance (i.e., per CSV file).
    private readonly SemaphoreSlim _lock = new(1, 1);

    private static readonly CsvConfiguration CsvConfig = new(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        MissingFieldFound = null,   // tolerate columns added in future migrations

        // The seed CSV files are hand-aligned with padding spaces for readability
        // ("Id, Username  , Email  ..."). CsvHelper does not trim by default, so
        // without these two settings header matching fails outright and every
        // string field keeps its padding.
        TrimOptions = TrimOptions.Trim,
        PrepareHeaderForMatch = args => args.Header.Trim()
    };

    /// <summary>
    /// Built once per T. Excludes computed get-only properties (e.g. User.FullName)
    /// which CsvHelper would otherwise emit as a phantom column on write, and pins
    /// DateTime handling to UTC so timestamps survive a write/read round-trip
    /// instead of drifting by the host's UTC offset.
    /// </summary>
    private static readonly ClassMap<T> Map = BuildMap();

    protected CsvRepositoryBase(string filePath)
    {
        _filePath = filePath;
    }

    private static ClassMap<T> BuildMap()
    {
        var map = new DefaultClassMap<T>();

        foreach (var property in typeof(T).GetProperties(
                     BindingFlags.Public | BindingFlags.Instance))
        {
            // Read-only members are derived values, not persisted columns.
            if (property.GetSetMethod() is null)
                continue;

            var memberMap = map.Map(typeof(T), property);

            if (property.PropertyType == typeof(DateTime) ||
                property.PropertyType == typeof(DateTime?))
            {
                memberMap.TypeConverterOption.Format(DateTimeFormats);
                memberMap.TypeConverterOption.DateTimeStyles(
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
            }
        }

        return map;
    }

    protected async Task<List<T>> ReadAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
                return [];

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, CsvConfig);
            csv.Context.RegisterClassMap(Map);
            return csv.GetRecords<T>().ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    protected async Task WriteAllAsync(IEnumerable<T> records)
    {
        await _lock.WaitAsync();
        try
        {
            await using var writer = new StreamWriter(_filePath, append: false);
            await using var csv = new CsvWriter(writer, CsvConfig);
            csv.Context.RegisterClassMap(Map);
            await csv.WriteRecordsAsync(records);
        }
        finally
        {
            _lock.Release();
        }
    }
}
