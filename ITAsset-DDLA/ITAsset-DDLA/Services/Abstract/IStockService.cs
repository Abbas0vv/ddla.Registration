using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Warehouse;

namespace ddla.ITApplication.Services.Abstract;

public interface IStockService
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetSomeAsync(int value);
    Task<Product> GetByIdAsync(int? id);
    Task Insert(CreateStockViewModel model);
    Task Remove(int? id);
    Task Update(int? id, UpdateStockViewModel model);
}
