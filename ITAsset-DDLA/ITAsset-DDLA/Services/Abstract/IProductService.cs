using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ddla.ITApplication.Services.Abstract;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetSomeAsync(int value);
    Task<Product> GetByIdAsync(int? id);
    Task InsertAsync(CreateProductViewModel model, StockProduct stockProduct);
    Task InsertMultipleAsync(CreateProductViewModel model, List<StockProduct> stockProducts);
    Task RemoveAsync(int? id);
    Task UpdateAsync(int? id, UpdateProductViewModel model, StockProduct stockProduct);
}
