using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;

namespace ddla.ITApplication.Services.Abstract;

public interface ITransferService
{
    Task<List<Transfer>> GetAllAsync();
    Task<int> GetAviableProductCount();
    Task<List<Transfer>> GetSomeAsync(int value);
    Task<Transfer> GetByIdAsync(int? id);
    Task<Transfer> GetByInventaryCode(string InventaryCode);
    Task<Transfer> GetByNameAsync(string name);
    Task InsertMultipleAsync(CreateTransferViewModel model);
    Task UpdateAsync(UpdateTransferViewModel model, string userName);
    Task RemoveAsync(int? id);
    Task ReturnAsync(int transferId, string actorUserName, string notes);
}
