using ddla.ITApplication.Database.Models.ViewModels.Product;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.LDAP;

namespace ITAsset_DDLA.Database.Models.ViewModels.Shared;

public class CreateTransferViewModel
{
    public CreateProductViewModel CreateProductViewModel { get; set; }
    public List<LdapUserModel>? LdapUsers { get; set; }
    public List<StockProduct>? StockProducts { get; set; }
}
