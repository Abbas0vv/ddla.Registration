using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Database.Models.ViewModels.Shared;

public class CompositeViewModel
{
    public Product Product { get; set; }
    public List<StockProduct> StockProducts { get; set; }
}
