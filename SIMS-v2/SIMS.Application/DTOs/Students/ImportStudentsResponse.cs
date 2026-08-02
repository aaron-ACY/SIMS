namespace SIMS.Application.DTOs.Students;

/// <summary>Summary returned after a bulk CSV import of student profiles.</summary>
public class ImportStudentsResponse
{
    /// <summary>Total rows read from the CSV (excluding the header).</summary>
    public int TotalRows { get; set; }

    /// <summary>Number of profiles successfully imported.</summary>
    public int Imported { get; set; }

    /// <summary>Number of rows skipped due to validation errors or duplicates.</summary>
    public int Skipped { get; set; }

    /// <summary>Row-level error messages for every skipped row.</summary>
    public IReadOnlyList<string> Errors { get; set; } = [];
}
