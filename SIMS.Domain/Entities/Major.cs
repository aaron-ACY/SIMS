namespace SIMS.Domain.Entities;

/// <summary>A major (chuyên ngành) offered by the institution.</summary>
public class Major
{
    public int    Id           { get; set; }

    /// <summary>Human-facing unique code, e.g. IT, SE, CS.</summary>
    public string MajorCode    { get; set; } = string.Empty;

    public string Name         { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public string Department   { get; set; } = string.Empty;
    public int    TotalCredits { get; set; }

    public bool     IsActive   { get; set; }
    public DateTime CreatedAt  { get; set; }
    public DateTime UpdatedAt  { get; set; }
}
