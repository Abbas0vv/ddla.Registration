using ddla.ITApplication.Database.Models.DomainModels.Account;

namespace ITAsset_DDLA.Database.Models.DomainModels;

public class UserPermission
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ddlaUser User { get; set; }
    public int PermissionId { get; set; }
    public Permission Permission { get; set; }
}
