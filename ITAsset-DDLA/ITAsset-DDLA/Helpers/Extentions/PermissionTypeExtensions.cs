using ITAsset_DDLA.Helpers.Enums;

namespace ITAsset_DDLA.Helpers.Extentions;

public static class PermissionTypeExtensions
{
    public static string GetDisplayName(this PermissionType permissionType)
    {
        return permissionType switch
        {
            // Operation Permissions
            PermissionType.OperationAdd => "Yeni əməliyyat yarat",
            PermissionType.OperationEdit => "Əməliyyat redaktə et",
            PermissionType.OperationDelete => "Əməliyyat sil",
            PermissionType.OperationView => "Əməliyyatları görüntülə",

            // Inventory Permissions
            PermissionType.InventoryAdd => "Anbara əlavə et",
            PermissionType.InventoryEdit => "Anbar redaktə et",
            PermissionType.InventoryDelete => "Anbar sil",
            PermissionType.InventoryView => "Anbarı görüntülə",

            // Equipment Permissions
            PermissionType.EquipmentEdit => "Avadanlıq redaktə et",
            PermissionType.EquipmentView => "Avadanlığı görüntülə",
            _ => permissionType.ToString()
        };
    }
}