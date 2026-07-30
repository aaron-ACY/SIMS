using SIMS.Application.DTOs.Users;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class UserService : IUserService
{
    private const int MinUsernameLength = 6;
    private const int MinPasswordLength = 8;

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserProfileResponse> GetMyProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        var role        = await _roleRepository.GetByIdAsync(user.RoleId);
        var permissions = await _permissionRepository.GetByRoleIdAsync(user.RoleId);

        return MapToProfile(user, role?.Name ?? string.Empty, permissions.Select(p => p.Name));
    }

    public async Task<IEnumerable<UserProfileResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var roles = await _roleRepository.GetAllAsync();

        // Lookup table: roleId → roleName
        var roleMap = roles.ToDictionary(r => r.Id, r => r.Name);

        // Load permissions once per distinct role (max 3 CSV reads instead of N).
        var distinctRoleIds = users.Select(u => u.RoleId).Distinct();
        var permissionsByRole = new Dictionary<int, IReadOnlyList<string>>();

        foreach (var roleId in distinctRoleIds)
        {
            var perms = await _permissionRepository.GetByRoleIdAsync(roleId);
            permissionsByRole[roleId] = perms.Select(p => p.Name).ToList().AsReadOnly();
        }

        return users.Select(u => MapToProfile(
            u,
            roleMap.TryGetValue(u.RoleId, out var name) ? name : string.Empty,
            permissionsByRole.TryGetValue(u.RoleId, out var perms) ? perms : []));
    }

    public async Task<UserProfileResponse> CreateAsync(CreateUserRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var email    = request.Email.Trim().ToLowerInvariant();

        if (username.Length < MinUsernameLength)
            throw new AppException(ErrorCode.USERNAME_INVALID);

        if (request.Password.Length < MinPasswordLength)
            throw new AppException(ErrorCode.INVALID_PASSWORD);

        if (await _userRepository.GetByUsernameAsync(username) is not null)
            throw new AppException(ErrorCode.USER_EXISTED);

        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new AppException(ErrorCode.EMAIL_EXISTED);

        var role = await _roleRepository.GetByIdAsync(request.RoleId)
                   ?? throw new AppException(ErrorCode.ROLE_NOT_EXISTED);

        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Username     = username,
            Email        = email,
            PasswordHash = hash,
            Salt         = salt,
            FirstName    = request.FirstName.Trim(),
            LastName     = request.LastName.Trim(),
            RoleId       = role.Id,
            IsActive     = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _userRepository.AddAsync(user);

        var permissions = await _permissionRepository.GetByRoleIdAsync(role.Id);
        return MapToProfile(user, role.Name, permissions.Select(p => p.Name));
    }

    public async Task<UserProfileResponse> UpdateMyInfoAsync(int userId, UpdateMyInfoRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        var email = request.Email.Trim().ToLowerInvariant();

        // The email must stay unique across accounts — but the caller keeping
        // their own address unchanged is not a conflict.
        var existing = await _userRepository.GetByEmailAsync(email);
        if (existing is not null && existing.Id != userId)
            throw new AppException(ErrorCode.EMAIL_EXISTED);

        user.Email     = email;
        user.FirstName = request.FirstName.Trim();
        user.LastName  = request.LastName.Trim();

        // UpdateAsync refreshes UpdatedAt.
        await _userRepository.UpdateAsync(user);

        var role        = await _roleRepository.GetByIdAsync(user.RoleId);
        var permissions = await _permissionRepository.GetByRoleIdAsync(user.RoleId);

        return MapToProfile(user, role?.Name ?? string.Empty, permissions.Select(p => p.Name));
    }

    public async Task DeleteAsync(int userId, int currentUserId)
    {
        // Guard against an admin locking themselves out of the system.
        if (userId == currentUserId)
            throw new AppException(ErrorCode.CANNOT_DELETE_SELF);

        _ = await _userRepository.GetByIdAsync(userId)
            ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        await _userRepository.DeleteAsync(userId);
    }

    // ------------------------------------------------------------------ //

    private static UserProfileResponse MapToProfile(
        Domain.Entities.User user,
        string roleName,
        IEnumerable<string> permissions) => new()
    {
        Id          = user.Id,
        Username    = user.Username,
        Email       = user.Email,
        FirstName   = user.FirstName,
        LastName    = user.LastName,
        Role        = roleName,
        Permissions = permissions.ToList().AsReadOnly()
    };
}
