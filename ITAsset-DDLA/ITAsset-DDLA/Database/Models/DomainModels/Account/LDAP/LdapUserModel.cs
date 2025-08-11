namespace ITAsset_DDLA.Database.Models.DomainModels.Account.LDAP;

public class LdapUserModel
{
    public string FullName { get; set; }    // cn
    public string Vazifa { get; set; }      // title
    public string InternalPhone { get; set; }  // telephoneNumber
    public string Email { get; set; }       // mail
    public string Shobe { get; set; }       // company
}