using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
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
        return await _context.Transfers.FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<Transfer> GetByNameAsync(string name)
    {
        return await _context.Transfers.FirstOrDefaultAsync(s => s.Name == name);
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

    #region CRUD
    public async Task InsertMultipleAsync(CreateTransferViewModel model)
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
            StockProductId = stockItem.Id
        }).ToList();

        // Add all products at once
        await _context.Transfers.AddRangeAsync(transfers);
        await _context.SaveChangesAsync();
    }
    public async Task<List<Transfer>> GetAllAsync()
    {
        return await _context.Transfers
            .Include(p => p.StockProduct)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }
    public async Task UpdateAsync(UpdateTransferViewModel model, string userName)
    {
        var existingProduct = await GetProductByStockIdAsync(model.UpdateTransferProductViewModel.StockProductId);
        if (existingProduct == null)
            throw new KeyNotFoundException($"Product not found");

        // Köhnə dəyərləri saxla
        var oldValues = new
        {
            existingProduct.Recipient,
            existingProduct.DepartmentSection,
            existingProduct.DateofReceipt,
            existingProduct.FilePath
        };

        // Fayl işlənməsi
        string filePath = existingProduct.FilePath;
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
            $"Məhsul redaktə edildi (ID: {existingProduct.Id}):\n" +
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
        existingProduct.Recipient = model.UpdateTransferProductViewModel.Recipient;
        existingProduct.DepartmentSection = model.UpdateTransferProductViewModel.DepartmentSection;
        existingProduct.DateofReceipt = model.UpdateTransferProductViewModel.DateofReceipt;
        existingProduct.FilePath = filePath;

        await _context.SaveChangesAsync();
    }
    public async Task RemoveAsync(int? id)
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
        _context.Remove(transfer);
        await _context.SaveChangesAsync();
    }
    #endregion
}