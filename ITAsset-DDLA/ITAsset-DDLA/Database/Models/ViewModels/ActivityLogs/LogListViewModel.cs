using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Database.Models.ViewModels.ActivityLogs;
public class LogListViewModel
{
    public List<ActivityLog> Logs { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}