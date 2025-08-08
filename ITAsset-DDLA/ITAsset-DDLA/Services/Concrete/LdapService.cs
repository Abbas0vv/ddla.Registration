using ITAsset_DDLA.Database.Models.DomainModels.Account.LDAP;
using System.DirectoryServices;

namespace ITAsset_DDLA.Services.Concrete;
public class LdapService
{
    private readonly string _ldapPath;
    private readonly string _ldapUser;
    private readonly string _ldapPassword;

    public LdapService(string ldapPath, string ldapUser, string ldapPassword)
    {
        _ldapPath = ldapPath;
        _ldapUser = ldapUser;
        _ldapPassword = ldapPassword;
    }

    public List<LdapUserModel> GetLdapUsers()
    {
        var ldapUsers = new List<LdapUserModel>();

        using (DirectoryEntry entry = new DirectoryEntry(_ldapPath, _ldapUser, _ldapPassword))
        using (DirectorySearcher searcher = new DirectorySearcher(entry))
        {
            searcher.Filter = "(objectCategory=person)";
            searcher.PropertiesToLoad.AddRange(new[] {
            "cn", "title", "telephoneNumber", "mail",
            "department", "departmentName", "ou", "company",
            "division", "organization", "physicalDeliveryOfficeName",
            "thumbnailPhoto", "jpegPhoto", "photo"
        });

            foreach (SearchResult result in searcher.FindAll())
            {
                var user = new LdapUserModel
                {
                    FullName = GetPropertyValue(result, "cn"),
                    Vazifa = GetPropertyValue(result, "title"),
                    InternalPhone = GetPropertyValue(result, "telephoneNumber"),
                    Email = GetPropertyValue(result, "mail"),
                    Shobe = GetPropertyValue(result, "company")
                };

                ldapUsers.Add(user);
            }
        }

        return ldapUsers;
    }
    private string GetPropertyValue(SearchResult result, string propertyName)
    {
        return result.Properties.Contains(propertyName) && result.Properties[propertyName].Count > 0
            ? result.Properties[propertyName][0].ToString()
            : string.Empty;
    }

    private string GetPropertyValue(SearchResult result, IEnumerable<string> propertyNames)
    {
        foreach (var property in propertyNames)
        {
            if (result.Properties.Contains(property) && result.Properties[property].Count > 0)
                return result.Properties[property][0].ToString();
        }
        return string.Empty;
    }
}