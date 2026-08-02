namespace SIMS.Application.DTOs.Majors;

/// <summary>A major (chuyên ngành) returned by the API.</summary>
public class MajorResponse
{
    public int    Id           { get; set; }
    public string MajorCode    { get; set; } = string.Empty;
    public string Name         { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public string Department   { get; set; } = string.Empty;
    public int    TotalCredits { get; set; }
    public bool   IsActive     { get; set; }
}
