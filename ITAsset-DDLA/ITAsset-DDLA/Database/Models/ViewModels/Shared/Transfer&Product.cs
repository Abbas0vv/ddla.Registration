using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Database.Models.ViewModels.Shared;

public class Transfer_Product
{
    public StockProduct StockProduct { get; set; }
    public Transfer Transfer { get; set; }
}
