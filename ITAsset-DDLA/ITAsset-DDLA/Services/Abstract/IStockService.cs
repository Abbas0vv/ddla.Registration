using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Services.Abstract;

public interface IStockService
{
    Task<List<StockProduct>> GetAllAsync();
    Task<List<StockProduct>> GetByIdsAsync(List<int> ids);
    Task<StockProduct> GetByIdAsync(int? id);
    Task InsertAsync(CreateStockViewModel model);
    Task RemoveAsync(int? id);
    Task UpdateAsync(int? id, UpdateStockViewModel model);
}
