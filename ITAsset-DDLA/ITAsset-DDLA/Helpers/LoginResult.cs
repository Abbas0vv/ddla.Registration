using ddla.ITApplication.Database.Models.DomainModels.Account;

namespace ITAsset_DDLA.Helpers;

public class LoginResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public ddlaUser User { get; set; }
}
