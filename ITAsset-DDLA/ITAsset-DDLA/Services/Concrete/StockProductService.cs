using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Helpers.Extentions;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class StockProductService : IStockService
{
    private readonly ddlaAppDBContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string FOLDER_NAME = "assets/images/Uploads/Products";

    public StockProductService(ddlaAppDBContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<List<StockProduct>> GetAllAsync()
    {
        return await _context.StockProducts
            .Include(sp => sp.Products)
            .OrderBy(sp => sp.Id)
            .ToListAsync();
    }

    public async Task<List<StockProduct>> GetByIdsAsync(List<int> ids)
    {
        return await _context.StockProducts
            .Where(sp => ids.Contains(sp.Id))
            .Include(sp => sp.Products)
            .ToListAsync();
    }

    public async Task<StockProduct> GetByIdAsync(int? id)
    {
        return await _context.StockProducts
          .Include(sp => sp.Products)
          .FirstOrDefaultAsync(sp => sp.Id == id);
    }

    public async Task InsertAsync(CreateStockViewModel model)
    {
        // Create the main product record
        foreach (var code in model.InventoryCodes)
        {
            var stockProduct = new StockProduct
            {
                Name = model.Name,
                Description = model.Description,
                RegistrationDate = model.DateofRegistration ?? DateTime.Now,
                ImageUrl = model.ImageFile?.CreateImageFile(_webHostEnvironment.WebRootPath, FOLDER_NAME),
                FilePath = model.DocumentFile?.CreateImageFile(_webHostEnvironment.WebRootPath, FOLDER_NAME),
                InventoryCode = code.Trim()
            };
            _context.StockProducts.Add(stockProduct);
        }

        await _context.SaveChangesAsync();
    }
    public async Task RemoveAsync(int? id)
    {
        if (id is null) return;

        var stockProduct = await _context.StockProducts
            .Include(sp => sp.Products)
            .FirstOrDefaultAsync(sp => sp.Id == id);

        if (stockProduct == null) return;

        // Check if any products are still using this stock
        if (stockProduct.Products.Any())
        {
            throw new Exception("Cannot delete StockProduct that has registered Products");
        }

        FileExtention.RemoveFile(Path.Combine(_webHostEnvironment.WebRootPath, FOLDER_NAME, stockProduct.ImageUrl));
        _context.Remove(stockProduct);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int? id, UpdateStockViewModel model)
    {
        if (id is null) return;

        var stockProduct = await _context.StockProducts.FindAsync(id);
        if (stockProduct == null) return;

        // Check if we're reducing the total count below what's already in use
        if (model.TotalCount < stockProduct.InUseCount)
        {
            throw new Exception($"Cannot reduce total count below currently in-use count ({stockProduct.InUseCount})");
        }

        stockProduct.Name = model.Name;
        stockProduct.Description = model.Description;
        stockProduct.RegistrationDate = model.DateofRegistration ?? stockProduct.RegistrationDate;

        if (model.ImageFile is not null)
        {
            stockProduct.ImageUrl = model.ImageFile.UpdateFile(_webHostEnvironment.WebRootPath, FOLDER_NAME, stockProduct.ImageUrl);
        }

        await _context.SaveChangesAsync();
    }
}