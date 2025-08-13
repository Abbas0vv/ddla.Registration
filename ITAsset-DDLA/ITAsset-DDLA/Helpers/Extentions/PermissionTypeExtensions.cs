using ITAsset_DDLA.Helpers.Enums;

namespace ITAsset_DDLA.Helpers.Extentions;

public static class PermissionTypeExtensions
{
    public static string GetDisplayName(this PermissionType permissionType)
    {
        return permissionType switch
        {
            // Operation Permissions
            PermissionType.OperationCreate => "Yeni əməliyyat yarat",
            PermissionType.OperationEdit => "Əməliyyat redaktə et",
            PermissionType.OperationDelete => "Əməliyyat sil",
            PermissionType.OperationView => "Əməliyyatları görüntülə",

            // Inventory Permissions
            PermissionType.InventoryCreate => "Anbar əlavə et",
            PermissionType.InventoryEdit => "Anbar redaktə et",
            PermissionType.InventoryDelete => "Anbar sil",
            PermissionType.InventoryView => "Anbarı görüntülə",

            // Equipment Permissions
            PermissionType.EquipmentCreate => "Avadanlıq əlavə et",
            PermissionType.EquipmentEdit => "Avadanlıq redaktə et",
            PermissionType.EquipmentDelete => "Avadanlıq sil",
            PermissionType.EquipmentView => "Avadanlığı görüntülə",

            // Admin Permissions
            PermissionType.AdminAccess => "Admin panelinə giriş",
            PermissionType.UserManagement => "İstifadəçi idarəetmə",

            _ => permissionType.ToString()
        };
    }
}