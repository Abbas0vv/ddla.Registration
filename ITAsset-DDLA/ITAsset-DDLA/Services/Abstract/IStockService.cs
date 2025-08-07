using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Services.Abstract;

public interface IStockService
{
    Task<List<StockProduct>> GetAllAsync();
    Task<int> GetTotalCount(List<StockProduct> stockProducts);
    Task<int> GetStockProductCountByNameAsync(string name);
    Task<List<StockProduct>> GetAllByDescriptionAsync(string description);
    Task<List<StockProduct>> GetByIdsAsync(List<int> ids);
    Task<StockProduct> GetByIdAsync(int? id);
    Task InsertAsync(CreateStockViewModel model);
    Task RemoveAsync(int? id);
    Task UpdateAsync(int? id, UpdateStockViewModel model);
    Task ToggleStatusAsync(int? id);
}
