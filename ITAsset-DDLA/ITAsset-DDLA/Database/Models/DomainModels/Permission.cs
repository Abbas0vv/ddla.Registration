using ITAsset_DDLA.Helpers.Enums;
namespace ITAsset_DDLA.Database.Models.DomainModels;

public class Permission
{
    public int Id { get; set; }
    public PermissionType Type { get; set; }
    public string Description { get; set; }
}
