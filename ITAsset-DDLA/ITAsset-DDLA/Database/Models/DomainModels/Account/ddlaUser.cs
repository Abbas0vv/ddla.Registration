using ITAsset_DDLA.Database.Models.DomainModels;
using Microsoft.AspNetCore.Identity;

namespace ddla.ITApplication.Database.Models.DomainModels.Account;

public class ddlaUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
