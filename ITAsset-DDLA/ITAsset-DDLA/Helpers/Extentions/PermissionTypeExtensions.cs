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
            PermissionType.InventoryRedaktəEt => "Anbar redaktə et",
            PermissionType.InventorySil => "Anbar sil",
            PermissionType.InventoryGörüntülə => "Anbarı görüntülə",

            // Equipment Permissions
            PermissionType.EquipmentRedatəEt => "Avadanlıq redaktə et",
            PermissionType.EquipmentGörüntülə => "Avadanlığı görüntülə",
            _ => permissionType.ToString()
        };
    }
}