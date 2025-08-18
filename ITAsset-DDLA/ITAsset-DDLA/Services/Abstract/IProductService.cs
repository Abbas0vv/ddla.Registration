using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;

namespace ddla.ITApplication.Services.Abstract;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<int> GetAviableProductCount();
    Task<List<Product>> GetSomeAsync(int value);
    Task<Product> GetByIdAsync(int? id);
    Task<Product> GetByInventaryCode(string InventaryCode);
    Task<Product> GetByNameAsync(string name);
    Task InsertMultipleAsync(DoubleCreateProductTypeViewModel model);
    Task UpdateAsync(DoubleUpdateProductTypeViewModel model);
    Task RemoveAsync(int? id);
}
