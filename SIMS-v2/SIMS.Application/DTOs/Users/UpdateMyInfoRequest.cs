using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Users;

/// <summary>
/// Payload for PUT /api/users/profile. Requires the EDIT_PROFILE permission.
/// All fields are optional — only the fields present in the request body are updated;
/// omitted (null) fields are left unchanged.
/// Password changes use a dedicated endpoint.
/// </summary>
public class UpdateMyInfoRequest
{
    /// <summary>Leave null to keep the current email.</summary>
    [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
    public string? Email { get; set; }

    /// <summary>Leave null to keep the current first name.</summary>
    public string? FirstName { get; set; }

    /// <summary>Leave null to keep the current last name.</summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Leave null to keep the current phone.
    /// Applies to Student and Instructor profiles only.
    /// </summary>
    public string? Phone { get; set; }
}
