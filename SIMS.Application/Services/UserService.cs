using SIMS.Application.DTOs.Users;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class UserService : IUserService
{
    private const int MinUsernameLength = 6;
    private const int MinPasswordLength = 8;

    private readonly IUserRepository       _userRepository;
    private readonly IRoleRepository       _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPasswordHasher       _passwordHasher;
    private readonly IStudentRepository    _studentRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IClassRepository      _classRepository;

    public UserService(
        IUserRepository       userRepository,
        IRoleRepository       roleRepository,
        IPermissionRepository permissionRepository,
        IPasswordHasher       passwordHasher,
        IStudentRepository    studentRepository,
        IInstructorRepository instructorRepository,
        IEnrollmentRepository enrollmentRepository,
        IClassRepository      classRepository)
    {
        _userRepository       = userRepository;
        _roleRepository       = roleRepository;
        _permissionRepository = permissionRepository;
        _passwordHasher       = passwordHasher;
        _studentRepository    = studentRepository;
        _instructorRepository = instructorRepository;
        _enrollmentRepository = enrollmentRepository;
        _classRepository      = classRepository;
    }

    public async Task<UserProfileResponse> GetMyProfileAsync(int userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        var role        = await _roleRepository.GetByIdAsync(user.RoleId);
        var permissions = await _permissionRepository.GetByRoleIdAsync(user.RoleId);
        var roleName    = role?.Name ?? string.Empty;

        string? studentCode    = null;
        string? instructorCode = null;

        if (roleName == Roles.Student)
        {
            var student = await _studentRepository.GetByUserIdAsync(userId);
            studentCode = student?.StudentCode;
        }
        else if (roleName == Roles.Instructor)
        {
            var instructor = await _instructorRepository.GetByUserIdAsync(userId);
            instructorCode = instructor?.InstructorCode;
        }

        return MapToProfile(user, roleName, permissions.Select(p => p.Name), studentCode, instructorCode);
    }

    public async Task<IEnumerable<UserProfileResponse>> GetAllAsync(CancellationToken ct = default)
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
            permissionsByRole.TryGetValue(u.RoleId, out var perms) ? perms : [],
            studentCode: null,
            instructorCode: null));
    }

    public async Task<UserProfileResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
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
        return MapToProfile(user, role.Name, permissions.Select(p => p.Name), null, null);
    }

    public async Task<UserProfileResponse> UpdateMyInfoAsync(int userId, UpdateMyInfoRequest request, CancellationToken ct = default)
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

        return MapToProfile(user, role?.Name ?? string.Empty, permissions.Select(p => p.Name), null, null);
    }

    public async Task DeleteAsync(int userId, int currentUserId, CancellationToken ct = default)
    {
        // Guard against an admin locking themselves out of the system.
        if (userId == currentUserId)
            throw new AppException(ErrorCode.CANNOT_DELETE_SELF);

        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? string.Empty;

        // ── Check dependents + cascade-delete the profile record ─────── //
        if (roleName == Roles.Student)
        {
            var student = await _studentRepository.GetByUserIdAsync(userId);
            if (student is not null)
            {
                // Block deletion when the student is still enrolled in classes.
                if (await _enrollmentRepository.ExistsActiveForStudentAsync(student.Id))
                    throw new AppException(ErrorCode.USER_HAS_ACTIVE_ENROLLMENTS);

                await _studentRepository.DeleteAsync(student.Id);
            }
        }
        else if (roleName == Roles.Instructor)
        {
            var instructor = await _instructorRepository.GetByUserIdAsync(userId);
            if (instructor is not null)
            {
                // Block deletion when the instructor is still assigned to active classes.
                var activeClasses = (await _classRepository.GetAllAsync())
                    .Any(c => c.InstructorId == instructor.Id && c.IsActive);

                if (activeClasses)
                    throw new AppException(ErrorCode.USER_HAS_ACTIVE_CLASSES);

                await _instructorRepository.DeleteAsync(instructor.Id);
            }
        }

        await _userRepository.DeleteAsync(userId);
    }

    // ------------------------------------------------------------------ //

    private static UserProfileResponse MapToProfile(
        Domain.Entities.User user,
        string roleName,
        IEnumerable<string> permissions,
        string? studentCode,
        string? instructorCode) => new()
    {
        StudentCode    = studentCode,
        InstructorCode = instructorCode,
        Username       = user.Username,
        Email          = user.Email,
        FirstName      = user.FirstName,
        LastName       = user.LastName,
        Role           = roleName,
        Permissions    = permissions.ToList().AsReadOnly()
    };
}
