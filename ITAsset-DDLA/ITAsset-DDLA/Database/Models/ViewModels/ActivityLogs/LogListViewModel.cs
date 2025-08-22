using ddla.ITApplication.Database.Models.DomainModels.Account;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;

namespace ITAsset_DDLA.Database.Models.ViewModels.ActivityLogs;
public class LogListViewModel
{
    public List<ActivityLog> Logs { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public List<ddlaUser> LocalUsers { get; set; }
    public List<PermissionType> PermissionTypes { get; set; }
}