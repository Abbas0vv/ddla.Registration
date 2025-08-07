using ddla.ITApplication.Database.Models.ViewModels.Product;
using ITAsset_DDLA.Database.Models.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITAsset_DDLA.Database.Models.ViewModels.Shared;

public class DoubleCreateProductTypeViewModel
{
    public CreateProductViewModel CreateProductViewModel { get; set; }
    public List<StockProduct>? StockProducts { get; set; }
}
