using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class StockService : IStockService
{
    private readonly ddlaAppDBContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly Lazy<IProductService> _productService;
    private const string FOLDER_NAME = "assets/images/Uploads/Products";

    public StockService(
        ddlaAppDBContext context,
        IWebHostEnvironment webHostEnvironment,
        Lazy<IProductService> productService)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _productService = productService;
    }

    public async Task<List<StockProduct>> GetAllAsync()
    {
        return await _context.StockProducts
            .Include(sp => sp.Products)
            .OrderBy(sp => sp.Id)
            .ToListAsync();
    }

    public async Task<int> GetTotalCount(List<StockProduct> stockProducts)
    {
        var productNames = stockProducts.Select(sp => sp.Name).Distinct().ToList();

        var totalCount = await _context.StockProducts
            .Where(sp => productNames.Contains(sp.Name))
            .CountAsync();

        return totalCount;
    }

    public async Task<List<StockProduct>> GetAllByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new List<StockProduct>();

        return await _context.StockProducts
            .Where(sp => sp.Name.ToLower() == name.ToLower())
            .ToListAsync();
    }


    public async Task<int> GetStockProductCountByNameAsync(string name)
    {
        return await _context.StockProducts
            .Where(sp => sp.Name == name)
            .CountAsync();
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

        FileExtention.RemoveFile(Path.Combine(_webHostEnvironment.WebRootPath, FOLDER_NAME, stockProduct.ImageUrl));
        _context.Remove(stockProduct);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int? id, UpdateStockViewModel model)
    {
        if (id is null) return;

        var stockProduct = await GetByIdAsync(id);
        if (stockProduct == null) return;

        stockProduct.Description = model.Description;
        stockProduct.InventoryCode = model.InventoryCode;

        await _context.SaveChangesAsync();
    }



    public async Task ToggleStatusAsync(int? id)
    {
        StockProduct product = await GetByIdAsync(id);

        if (product is null)
            return;

        product.IsActive = !product.IsActive;
        await _context.SaveChangesAsync();
    }
}