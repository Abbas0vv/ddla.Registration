namespace ITAsset_DDLA.Database.Models.DomainModels;

public class ActivityLog
{
    public int Id { get; set; }
    public string UserFullName { get; set; } 
    public string Action { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
