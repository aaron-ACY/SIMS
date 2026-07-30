namespace SIMS.Application.DTOs.Users;

/// <summary>Profile of a user, with role name and permissions resolved from the store.</summary>
public class UserProfileResponse
{
    public int                  Id          { get; set; }
    public string               Username    { get; set; } = string.Empty;
    public string               Email       { get; set; } = string.Empty;
    public string               FirstName   { get; set; } = string.Empty;
    public string               LastName    { get; set; } = string.Empty;
    public string               Role        { get; set; } = string.Empty;
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
