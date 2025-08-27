using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class TransferService : ITransferService
{
    private readonly ddlaAppDBContext _context;
    private readonly IActivityLogger _activityLogger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IStockService _stockService;
    private const string FOLDER_NAME = "assets/images/Uploads/Products";
    public TransferService(
        ddlaAppDBContext context,
        IWebHostEnvironment webHostEnvironment,
        IStockService stockService,
        IActivityLogger activityLogger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _stockService = stockService;
        _activityLogger = activityLogger;
    }
    public async Task<int> GetAviableProductCount()
    {
        return await _context.Transfers
            .Where(p => p.DateofReceipt == null)
            .CountAsync();
    }
    public async Task<List<Transfer>> GetSomeAsync(int value)
    {
        var numberOfTransfers = await _context.Transfers.CountAsync();
        if (numberOfTransfers <= value) return await GetAllAsync();
        return await _context.Transfers.OrderBy(s => s.Id).Take(value).ToListAsync();
    }
    public async Task<Transfer> GetByIdAsync(int? id)
    {
        return await _context.Transfers
            .Include(t => t.StockProduct)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<Transfer> GetByNameAsync(string name)
    {
        return await _context.Transfers
            .Include(t => t.StockProduct)
            .FirstOrDefaultAsync(s => s.Name == name);
    }
    public async Task<Transfer> GetProductByStockIdAsync(int? stockId)
    {
        return await _context.Transfers
            .Include(p => p.StockProduct)
            .FirstOrDefaultAsync(s => s.StockProductId == stockId);
    }
    public async Task<Transfer> GetByInventaryCode(string InventaryCode)
    {
        return await _context.Transfers
            .Include(p => p.StockProduct)
            .FirstOrDefaultAsync(s => s.InventarId == InventaryCode);
    }
    public async Task<List<Transfer>> GetAllAsync()
    {
        return await _context.Transfers
            .Include(p => p.StockProduct)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    #region CRUD
    public async Task InsertMultipleAsync(CreateTransferViewModel model, string userName)
    {
        // Validate input
        if (model == null || model.CreateTransferProductViewModel.StockProductIds == null || !model.CreateTransferProductViewModel.StockProductIds.Any())
        {
            throw new ArgumentException("Invalid input data - model or product IDs cannot be null/empty");
        }

        // Get the selected stock products
        var stockProducts = await _stockService.GetByIdsAsync(model.CreateTransferProductViewModel.StockProductIds);
        foreach (var stockProduct in stockProducts)
        {
            stockProduct.IsActive = false; // Deactivate stock products
        }
        // Verify we found all requested products
        if (stockProducts.Count != model.CreateTransferProductViewModel.StockProductIds.Count)
        {
            var missingIds = model.CreateTransferProductViewModel.StockProductIds.Except(stockProducts.Select(sp => sp.Id)).ToList();
            throw new KeyNotFoundException($"Could not find all requested stock products. Missing IDs: {string.Join(",", missingIds)}");
        }

        // Process documents if they exist
        string filePath = null;

        if (model.CreateTransferProductViewModel.DocumentFile != null)
            filePath = FileExtention.CreateFile(model.CreateTransferProductViewModel.DocumentFile,
                    _webHostEnvironment.WebRootPath, FOLDER_NAME);

        // Create transfers
        var transfers = stockProducts.Select(stockItem => new Transfer
        {
            Recipient = model.CreateTransferProductViewModel.Recipient,
            IsSigned = false,
            ImageUrl = stockItem.ImageUrl,
            FilePath = filePath,
            DepartmentSection = model.CreateTransferProductViewModel.DepartmentSection,
            DateofReceipt = model.CreateTransferProductViewModel.DateofReceipt,
            StockProductId = stockItem.Id,
            TransferStatus = TransferAction.Created
        }).ToList();

        foreach (var transfer in transfers)
        {
            var hist = new TransferHistory
            {
                TransferId = transfer.Id,
                Action = TransferAction.Returned,
                Actor = userName,
                ActionDate = DateTime.Now,
                FromUser = transfer.Recipient,
                ToUser = "Anbar", // və ya konkret who received
            };
            _context.TransferHistories.Add(hist);
        }

        // Add all products at once
        await _context.Transfers.AddRangeAsync(transfers);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(UpdateTransferViewModel model, string userName)
    {
        var existingTransfer = await GetProductByStockIdAsync(model.UpdateTransferProductViewModel.StockProductId);
        if (existingTransfer == null)
            throw new KeyNotFoundException($"Product not found");

        // Köhnə dəyərləri saxla
        var oldValues = new
        {
            existingTransfer.Recipient,
            existingTransfer.DepartmentSection,
            existingTransfer.DateofReceipt,
            existingTransfer.FilePath
        };

        // Fayl işlənməsi
        string filePath = existingTransfer.FilePath;
        if (model.UpdateTransferProductViewModel.DocumentFile != null)
        {
            if (!string.IsNullOrEmpty(filePath))
                FileExtention.RemoveFile(Path.Combine(filePath, _webHostEnvironment.WebRootPath, FOLDER_NAME));

            filePath = FileExtention.CreateFile(model.UpdateTransferProductViewModel.DocumentFile,
                    _webHostEnvironment.WebRootPath, FOLDER_NAME);
        }

        // Yeni dəyərlər
        var newValues = new
        {
            model.UpdateTransferProductViewModel.Recipient,
            model.UpdateTransferProductViewModel.DepartmentSection,
            model.UpdateTransferProductViewModel.DateofReceipt,
            FilePath = filePath
        };

        // Loglama (diff ilə)
        await _activityLogger.LogAsync(
            userName,
            $"Məhsul redaktə edildi (ID: {existingTransfer.Id}):\n" +
            $"--- Köhnə məlumatlar ---\n" +
            $"Recipient: {oldValues.Recipient}\n" +
            $"DepartmentSection: {oldValues.DepartmentSection}\n" +
            $"DateofReceipt: {oldValues.DateofReceipt}\n" +
            $"FilePath: {oldValues.FilePath}\n\n" +
            $"--- Yeni məlumatlar ---\n" +
            $"Recipient: {newValues.Recipient}\n" +
            $"DepartmentSection: {newValues.DepartmentSection}\n" +
            $"DateofReceipt: {newValues.DateofReceipt}\n" +
            $"FilePath: {newValues.FilePath}"
        );

        // Update product
        existingTransfer.Recipient = model.UpdateTransferProductViewModel.Recipient;
        existingTransfer.DepartmentSection = model.UpdateTransferProductViewModel.DepartmentSection;
        existingTransfer.DateofReceipt = model.UpdateTransferProductViewModel.DateofReceipt;
        existingTransfer.FilePath = filePath;
        existingTransfer.TransferStatus = TransferAction.Edited;

        var hist = new TransferHistory
        {
            TransferId = existingTransfer.Id,
            Action = TransferAction.Returned,
            Actor = userName,
            ActionDate = DateTime.Now,
            FromUser = existingTransfer.Recipient,
            ToUser = "Anbar", // və ya konkret who received
        };
        _context.TransferHistories.Add(hist);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveAsync(int? id, string actorUserName)
    {
        if (id is null) return;

        var transfer = await _context.Transfers
            .Include(p => p.StockProduct)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (transfer is null) return;

        var stockProduct = await _stockService.GetByIdAsync(transfer.StockProductId);
        stockProduct.IsActive = true; // Reactivate stock product

        // No need to manually update StockProduct because it's computed
        FileExtention.RemoveFile(Path.Combine(_webHostEnvironment.WebRootPath, FOLDER_NAME, transfer.ImageUrl));
        transfer.TransferStatus = TransferAction.Deleted;
        var hist = new TransferHistory
        {
            TransferId = transfer.Id,
            Action = TransferAction.Returned,
            Actor = actorUserName,
            ActionDate = DateTime.Now,
            FromUser = transfer.Recipient,
            ToUser = "Anbar", // və ya konkret who received
        };
        _context.TransferHistories.Add(hist);
        _context.Remove(transfer);
        await _context.SaveChangesAsync();
    }
    public async Task ReturnAsync(int transferId, string actorUserName, string notes)
    {
        // use a transaction to avoid partial updates
        using var tx = await _context.Database.BeginTransactionAsync();

        var transfer = await _context.Transfers
            .Include(t => t.StockProduct)
            .FirstOrDefaultAsync(t => t.Id == transferId);

        if (transfer == null)
            throw new InvalidOperationException("Transfer tapılmadı.");

        if (!transfer.IsSigned)
            throw new InvalidOperationException("Hələ imzalanmayıb — qaytarma qeydə alınmaz.");

        if (transfer.IsReturned)
            throw new InvalidOperationException("Bu transfer artıq qaytarılıb.");

        // mark returned
        transfer.IsReturned = true;
        transfer.DateOfReturn = DateTime.Now; // və ya DateTime.UtcNow, komandaya görə
        transfer.ReturnedBy = actorUserName;

        // add history record
        var hist = new TransferHistory
        {
            TransferId = transfer.Id,
            Action = TransferAction.Returned,
            Actor = actorUserName,
            ActionDate = DateTime.Now,
            FromUser = transfer.Recipient,
            ToUser = "Anbar", // və ya konkret who received
        };
        _context.TransferHistories.Add(hist);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }
    #endregion

}