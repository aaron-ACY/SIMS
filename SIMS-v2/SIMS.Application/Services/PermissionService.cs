using SIMS.Application.DTOs.Permissions;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

/// <summary>
/// CRUD over permissions.csv.
///
/// Important: a permission created here is data only. Authorization policies are
/// registered once at startup from Permissions.All (see Program.cs), so a brand-new
/// permission cannot be referenced by [Authorize(Policy = ...)] until it is also added
/// to the Permissions constants class and the app is restarted. Renaming an existing
/// permission has the inverse hazard — it silently detaches the row from the policy
/// that still expects the old name, so every endpoint guarded by it starts returning
/// 403 for users whose tokens carry the renamed claim.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository)
    {
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<PermissionResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var permissions = await _permissionRepository.GetAllAsync();
        return permissions.Select(MapToResponse);
    }

    public async Task<PermissionResponse> CreateAsync(CreatePermissionRequest request, CancellationToken ct = default)
    {
        // Names are compared case-insensitively everywhere, so normalise on write to
        // match the SCREAMING_SNAKE_CASE convention the seed data uses.
        var name = request.Name.Trim().ToUpperInvariant();

        if (await _permissionRepository.GetByNameAsync(name) is not null)
            throw new AppException(ErrorCode.PERMISSION_EXISTED);

        var permission = new Permission
        {
            Name        = name,
            Description = request.Description.Trim()
        };

        // AddAsync assigns Id.
        await _permissionRepository.AddAsync(permission);

        return MapToResponse(permission);
    }

    public async Task<PermissionResponse> UpdateAsync(
        int permissionId,
        UpdatePermissionRequest request,
        CancellationToken ct = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(permissionId)
                         ?? throw new AppException(ErrorCode.PERMISSION_NOT_EXISTED);

        if (request.Name is not null)
        {
            var name = request.Name.Trim().ToUpperInvariant();

            if (name.Length == 0)
                throw new AppException(ErrorCode.VALIDATION_ERROR, "Name cannot be empty.");

            // Uniqueness must hold across rows, but keeping your own name is not a clash.
            var existing = await _permissionRepository.GetByNameAsync(name);
            if (existing is not null && existing.Id != permissionId)
                throw new AppException(ErrorCode.PERMISSION_EXISTED);

            permission.Name = name;
        }

        if (request.Description is not null)
            permission.Description = request.Description.Trim();

        await _permissionRepository.UpdateAsync(permission);

        return MapToResponse(permission);
    }

    public async Task<RolePermissionsResponse> AssignToRoleAsync(
        int roleId,
        AssignPermissionRequest request,
        CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId)
                   ?? throw new AppException(ErrorCode.ROLE_NOT_EXISTED);

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId)
                         ?? throw new AppException(ErrorCode.PERMISSION_NOT_EXISTED);

        // The join table has no uniqueness constraint of its own, so a duplicate would
        // silently add a second identical row and show the permission twice in the JWT.
        if (await _rolePermissionRepository.ExistsAsync(role.Id, permission.Id))
            throw new AppException(ErrorCode.PERMISSION_ALREADY_ASSIGNED);

        await _rolePermissionRepository.AddAsync(new RolePermission
        {
            RoleId       = role.Id,
            PermissionId = permission.Id
        });

        var granted = await _permissionRepository.GetByRoleIdAsync(role.Id);

        return new RolePermissionsResponse
        {
            RoleId      = role.Id,
            RoleName    = role.Name,
            Permissions = granted.Select(MapToResponse).ToList().AsReadOnly()
        };
    }

    // ------------------------------------------------------------------ //

    private static PermissionResponse MapToResponse(Permission permission) => new()
    {
        Id          = permission.Id,
        Name        = permission.Name,
        Description = permission.Description
    };
}
