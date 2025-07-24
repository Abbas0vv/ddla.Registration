using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Services.Abstract;

public interface IStockService
{
    Task<List<StockProduct>> GetAllAsync();
    Task<StockProduct> GetByIdAsync(int? id);
    Task<StockProduct> InsertAsync(CreateStockViewModel model);
    Task RemoveAsync(int? id);
    Task UpdateAsync(int? id, UpdateStockViewModel model);
}
