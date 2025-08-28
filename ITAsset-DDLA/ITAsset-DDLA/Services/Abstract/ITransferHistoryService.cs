using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Services.Abstract;

public interface ITransferHistoryService
{
    Task<List<TransferHistory>> GetAllAsync();
    Task<TransferHistory?> GetByIdAsync(int id);
    Task AddAsync(TransferHistory history);
    Task DeleteAsync(int id);
}
