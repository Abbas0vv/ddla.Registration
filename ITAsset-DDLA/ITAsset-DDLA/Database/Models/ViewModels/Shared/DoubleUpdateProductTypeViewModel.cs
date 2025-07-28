using ddla.ITApplication.Database.Models.ViewModels.Product;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Database.Models.ViewModels.Shared;

public class DoubleUpdateProductTypeViewModel
{
    public UpdateProductViewModel UpdateProductViewModel { get; set; }
    public StockProduct? StockProduct { get; set; }
}
