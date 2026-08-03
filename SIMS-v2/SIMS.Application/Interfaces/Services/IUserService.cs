using SIMS.Application.DTOs.Users;

namespace SIMS.Application.Interfaces.Services;

public interface IUserService
{
    /// <summary>
    /// Returns the profile of the authenticated user.
    /// Used by GET /api/users/profile. The <paramref name="roleName"/> is read
    /// from the JWT claim so no extra role DB round-trip is needed.
    /// </summary>
    Task<UserProfileResponse> GetMyProfileAsync(int userId, string roleName, CancellationToken ct = default);

    /// <summary>Returns all users with role name and permissions resolved. Admin only.</summary>
    Task<IEnumerable<UserProfileResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Creates a new user account. Requires the CREATE_USER permission.
    /// </summary>
    Task<UserProfileResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

    /// <summary>
    /// Updates the caller's own email, first name, last name, and phone.
    /// The <paramref name="roleName"/> is read from the JWT claim so the service
    /// can route the phone update to the correct profile record without a role DB lookup.
    /// Requires the EDIT_PROFILE permission.
    /// </summary>
    Task<UserProfileResponse> UpdateMyInfoAsync(int userId, string roleName, UpdateMyInfoRequest request, CancellationToken ct = default);

    /// <summary>
    /// Creates a user account (role = Student) together with the student profile
    /// in a single operation. Requires the CREATE_USER permission.
    /// </summary>
    Task<UserProfileResponse> CreateStudentUserAsync(CreateStudentUserRequest request, CancellationToken ct = default);

    /// <summary>
    /// Creates a user account (role = Instructor) together with the instructor profile
    /// in a single operation. Requires the CREATE_USER permission.
    /// </summary>
    Task<UserProfileResponse> CreateInstructorUserAsync(CreateInstructorUserRequest request, CancellationToken ct = default);

    /// <summary>
    /// Changes the password of the authenticated caller.
    /// Verifies the current password before applying the new one.
    /// Requires the CHANGE_PASSWORD permission (Instructor and Student roles).
    /// </summary>
    /// <param name="userId">The caller's own account ID, read from the JWT.</param>
    /// <param name="request">Contains the current and new password.</param>
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes a user account. Requires the DELETE_USER permission.
    /// </summary>
    /// <param name="userId">The account to delete.</param>
    /// <param name="currentUserId">The caller — used to block self-deletion.</param>
    Task DeleteAsync(int userId, int currentUserId, CancellationToken ct = default);
}
