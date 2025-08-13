using ITAsset_DDLA.Helpers.Enums;

namespace ITAsset_DDLA.Helpers.Extentions;

public static class PermissionTypeExtensions
{
    public static string GetDisplayName(this PermissionType permissionType)
    {
        return permissionType switch
        {
            // Operation Permissions
            PermissionType.OperationƏlavəEt => "Yeni əməliyyat yarat",
            PermissionType.OperationRedatəEt => "Əməliyyat redaktə et",
            PermissionType.OperationSil => "Əməliyyat sil",
            PermissionType.OperationGörüntülə => "Əməliyyatları görüntülə",

            // Inventory Permissions
            PermissionType.InventoryƏlavəEt => "Anbar əlavə et",
            PermissionType.InventoryRedatəEt => "Anbar redaktə et",
            PermissionType.InventorySil => "Anbar sil",
            PermissionType.InventoryGörüntülə => "Anbarı görüntülə",

            // Equipment Permissions
            PermissionType.EquipmentRedatəEt => "Avadanlıq redaktə et",
            PermissionType.EquipmentSil => "Avadanlıq sil",
            PermissionType.EquipmentGörüntülə => "Avadanlığı görüntülə",

            // Admin Permissions
            PermissionType.AdminAccess => "Admin panelinə giriş",
            PermissionType.Adminİstifadəçiİdarəetmə => "İstifadəçi idarəetmə",

            _ => permissionType.ToString()
        };
    }
}