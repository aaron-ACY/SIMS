namespace SIMS.Domain.Entities;

/// <summary>Join entity mapping a Role to a Permission.</summary>
public class RolePermission
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
