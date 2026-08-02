using SIMS.Application.DTOs.Auth;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

/// <summary>
/// Handles four authentication flows: login, register, logout, and token refresh.
/// Token-revocation details (block-window calculation, persistence) are
/// delegated entirely to <see cref="ITokenRevocationService"/>.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository          _userRepository;
    private readonly IStudentRepository       _studentRepository;
    private readonly IInstructorRepository    _instructorRepository;
    private readonly IRoleRepository          _roleRepository;
    private readonly IPermissionRepository    _permissionRepository;
    private readonly IPasswordHasher          _passwordHasher;
    private readonly ITokenService            _tokenService;
    private readonly ITokenRevocationService  _revocationService;

    public AuthService(
        IUserRepository         userRepository,
        IStudentRepository      studentRepository,
        IInstructorRepository   instructorRepository,
        IRoleRepository         roleRepository,
        IPermissionRepository   permissionRepository,
        IPasswordHasher         passwordHasher,
        ITokenService           tokenService,
        ITokenRevocationService revocationService)
    {
        _userRepository       = userRepository;
        _studentRepository    = studentRepository;
        _instructorRepository = instructorRepository;
        _roleRepository       = roleRepository;
        _permissionRepository = permissionRepository;
        _passwordHasher       = passwordHasher;
        _tokenService         = tokenService;
        _revocationService    = revocationService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var username = request.Username.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByUsernameAsync(username);

        // Single generic error prevents username-enumeration attacks.
        if (user is null || !user.IsActive)
            throw new AppException(ErrorCode.INVALID_CREDENTIALS);

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
            throw new AppException(ErrorCode.INVALID_CREDENTIALS);

        var role = await _roleRepository.GetByIdAsync(user.RoleId)
                   ?? throw new AppException(ErrorCode.INVALID_CREDENTIALS);

        var permissions     = await _permissionRepository.GetByRoleIdAsync(user.RoleId);
        var permissionNames = permissions.Select(p => p.Name);

        var tokenResult = _tokenService.GenerateToken(user, role.Name, permissionNames);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt   = tokenResult.ExpiresAt
        };
    }

    /// <inheritdoc/>
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var email    = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim().ToLowerInvariant();

        // Username must be unique.
        if (await _userRepository.GetByUsernameAsync(username) is not null)
            throw new AppException(ErrorCode.USER_EXISTED);

        // If this email already has a user account, the profile is already linked.
        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new AppException(ErrorCode.ACCOUNT_ALREADY_LINKED);

        // Look up the email in students first, then instructors.
        var student    = await _studentRepository.GetByEmailAsync(email);
        var instructor = student is null
            ? await _instructorRepository.GetByEmailAsync(email)
            : null;

        if (student is null && instructor is null)
            throw new AppException(ErrorCode.EMAIL_NOT_REGISTERED);

        // Guard against a profile that is already linked to an account
        // (shouldn't happen given the user-email check above, but kept as a defence).
        if (student?.UserId is not null || instructor?.UserId is not null)
            throw new AppException(ErrorCode.ACCOUNT_ALREADY_LINKED);

        // Assign role based on which profile matched.
        var roleName = student is not null ? Roles.Student : Roles.Instructor;
        var role     = await _roleRepository.GetByNameAsync(roleName)
                       ?? throw new AppException(ErrorCode.ROLE_NOT_EXISTED);

        // Pull name from the imported profile; it will be mirrored on the User.
        var firstName = student?.FirstName ?? instructor!.FirstName;
        var lastName  = student?.LastName  ?? instructor!.LastName;

        // Create the user account.
        var (hash, salt) = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Username     = username,
            Email        = email,
            PasswordHash = hash,
            Salt         = salt,
            FirstName    = firstName,
            LastName     = lastName,
            RoleId       = role.Id,
            IsActive     = true
        };
        await _userRepository.AddAsync(user);

        // Link the profile to the newly created account.
        if (student is not null)
        {
            student.UserId = user.Id;
            await _studentRepository.UpdateAsync(student);
        }
        else
        {
            instructor!.UserId = user.Id;
            await _instructorRepository.UpdateAsync(instructor);
        }

        // Issue a token so the caller is logged in immediately after registering.
        var permissions = await _permissionRepository.GetByRoleIdAsync(role.Id);
        var tokenResult = _tokenService.GenerateToken(
            user, role.Name, permissions.Select(p => p.Name));

        return new RegisterResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt   = tokenResult.ExpiresAt,
            Role        = role.Name
        };
    }

    public async Task LogoutAsync(string rawToken, CancellationToken ct = default)
    {
        // Full validation: signature, issuer and audience are all verified.
        // Lifetime is intentionally skipped — a just-expired token must still
        // be revocable so the user can log out cleanly after a brief network delay.
        var principal = _tokenService.ValidateIgnoringLifetime(rawToken)
                        ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        await _revocationService.RevokeAsync(
            principal.Jti, principal.UserId, principal.ExpiresAt, ct);
    }

    public async Task<LoginResponse> RefreshTokenAsync(
        RefreshTokenRequest request, CancellationToken ct = default)
    {
        var principal = _tokenService.ValidateIgnoringLifetime(request.AccessToken)
                        ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        if (await _revocationService.IsRevokedAsync(principal.Jti, ct))
            throw new AppException(ErrorCode.INVALID_TOKEN);

        var user = await _userRepository.GetByIdAsync(principal.UserId)
                   ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        if (!user.IsActive)
            throw new AppException(ErrorCode.INVALID_TOKEN);

        var role = await _roleRepository.GetByIdAsync(user.RoleId)
                   ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        var permissions = await _permissionRepository.GetByRoleIdAsync(user.RoleId);

        var tokenResult = _tokenService.GenerateToken(
            user, role.Name, permissions.Select(p => p.Name));

        await _revocationService.RevokeAsync(
            principal.Jti, user.Id, principal.ExpiresAt, ct);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt   = tokenResult.ExpiresAt
        };
    }
}
