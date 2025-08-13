using ITAsset_DDLA.Helpers.Enums;

namespace ITAsset_DDLA.Database.Models.ViewModels.Admin;

// For displaying the form (GET)
public class EditPermissionsViewModel
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string ProfilePictureUrl { get; set; }
    public List<PermissionGroup> PermissionGroups { get; set; }
}

public class PermissionGroup
{
    public string GroupName { get; set; }
    public List<PermissionItem> Permissions { get; set; }
}

public class PermissionItem
{
    public PermissionType Type { get; set; }
    public string Description { get; set; }
    public bool HasPermission { get; set; }
}

// For form submission (POST)
public class UpdatePermissionsViewModel
{
    public string UserId { get; set; }
    public List<PermissionUpdate> Permissions { get; set; } = new List<PermissionUpdate>();
}

public class PermissionUpdate
{
    public PermissionType Type { get; set; }
    public bool IsSelected { get; set; }
}