using SIMS.Application.DTOs.Users;

namespace SIMS.Application.Interfaces.Services;

public interface IUserService
{
    /// <summary>
    /// Returns the profile of a single user by ID (fetched from DB).
    /// Used by GET /api/users/me — email is no longer in the JWT so
    /// we load it from the CSV store.
    /// </summary>
    Task<UserProfileResponse> GetMyProfileAsync(int userId, CancellationToken ct = default);

    /// <summary>Returns all users with role name and permissions resolved. Admin only.</summary>
    Task<IEnumerable<UserProfileResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Creates a new user account. Requires the CREATE_USER permission.
    /// </summary>
    Task<UserProfileResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

    /// <summary>
    /// Updates the caller's own first name, last name and email.
    /// Requires the EDIT_PROFILE permission.
    /// </summary>
    Task<UserProfileResponse> UpdateMyInfoAsync(int userId, UpdateMyInfoRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes a user account. Requires the DELETE_USER permission.
    /// </summary>
    /// <param name="userId">The account to delete.</param>
    /// <param name="currentUserId">The caller — used to block self-deletion.</param>
    Task DeleteAsync(int userId, int currentUserId, CancellationToken ct = default);
}
