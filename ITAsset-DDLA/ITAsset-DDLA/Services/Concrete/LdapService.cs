using ITAsset_DDLA.LDAP;
using System.DirectoryServices;

namespace ITAsset_DDLA.Services.Concrete;
using ITAsset_DDLA.LDAP;
using System.DirectoryServices;

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
        var titles = LdapTranslations.Titles;
        var companies = LdapTranslations.Companies;

        using (var entry = new DirectoryEntry(_ldapPath, _ldapUser, _ldapPassword))
        using (var searcher = new DirectorySearcher(entry))
        {
            searcher.Filter = "(objectCategory=person)";
            searcher.PropertiesToLoad.AddRange(new[] { "cn", "title", "telephoneNumber", "mail", "company" });

            foreach (SearchResult result in searcher.FindAll())
            {
                var title = GetPropertyValue(result, "title");
                var company = GetPropertyValue(result, "company");

                ldapUsers.Add(new LdapUserModel
                {
                    FullName = GetPropertyValue(result, "cn"),
                    Vazifa = titles.ContainsKey(title) ? titles[title] : title,
                    InternalPhone = GetPropertyValue(result, "telephoneNumber"),
                    Email = GetPropertyValue(result, "mail"),
                    Shobe = companies.ContainsKey(company) ? companies[company] : company
                });
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
}
