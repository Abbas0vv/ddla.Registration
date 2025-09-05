using ITAsset_DDLA.Helpers.Enums;

namespace ITAsset_DDLA.Database.Models.ViewModels.Admin;

public class UserWithPermissionsViewModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string ProfilePictureUrl { get; set; }
    public List<PermissionType> Permissions { get; set; } = new List<PermissionType>();
    public LocalUserStatus Status { get; set; }

    public bool HasPermission(PermissionType permission)
    {
        return Permissions.Contains(permission);
    }
}