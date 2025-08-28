using ddla.ITApplication.Database;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ITAsset_DDLA.Services.Concrete;

public class TransferHistoryService : ITransferHistoryService
{
    private readonly ddlaAppDBContext _context;

    public TransferHistoryService(ddlaAppDBContext context)
    {
        _context = context;
    }

    public async Task<List<TransferHistory>> GetAllAsync()
    {
        return await _context.TransferHistories
            .OrderByDescending(x => x.ActionDate)
            .ToListAsync();
    }

    public async Task<TransferHistory?> GetByIdAsync(int id)
    {
        return await _context.TransferHistories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(TransferHistory history)
    {
        _context.TransferHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var history = await _context.TransferHistories.FindAsync(id);
        if (history != null)
        {
            _context.TransferHistories.Remove(history);
            await _context.SaveChangesAsync();
        }
    }
}