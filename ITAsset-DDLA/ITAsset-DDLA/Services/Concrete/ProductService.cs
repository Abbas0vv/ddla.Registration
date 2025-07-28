using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class ProductService : IProductService
{
    private readonly ddlaAppDBContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string FOLDER_NAME = "assets/images/Uploads/Products";
    public ProductService(ddlaAppDBContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    #region Methods
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.StockProduct)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }
    public async Task<List<Product>> GetSomeAsync(int value)
    {
        var numberOfProducts = await _context.Products.CountAsync();
        if (numberOfProducts <= value) return await GetAllAsync();
        return await _context.Products.OrderBy(s => s.Id).Take(value).ToListAsync();
    }
    public async Task<Product> GetByIdAsync(int? id)
    {
        return await _context.Products.FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<Product> GetByNameAsync(string name)
    {
        return await _context.Products.FirstOrDefaultAsync(s => s.Name == name);
    }
    public async Task<Product> InsertAsync(CreateProductViewModel model, StockProduct stockProduct)
    {
        if (stockProduct == null)
            throw new Exception("Selected stock product not found");

        if (model.Count > stockProduct.AvailableCount)
            throw new Exception($"Not enough items available. Available: {stockProduct.AvailableCount}, Requested: {model.Count}");
        var product = new Product
        {
            StockProductId = stockProduct.Id,
            Name = stockProduct.Name,
            Description = stockProduct.Description,
            InventarId = model.InventarId,
            Recipient = model.Recipient,
            InUseCount = model.Count,
            Department = model.DepartmentName,
            Unit = model.UnitName,
            DateofIssue = DateTime.Now,
            ImageUrl = stockProduct.ImageUrl,
            FilePath = model.DocumentFile?.CreateImageFile(_webHostEnvironment.WebRootPath, "assets/images/Uploads/Documents")
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;

    }
    public async Task RemoveAsync(int? id)
    {
        if (id is null) return;

        var product = await _context.Products
            .Include(p => p.StockProduct)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return;

        // No need to manually update StockProduct because it's computed
        FileExtention.RemoveFile(Path.Combine(_webHostEnvironment.WebRootPath, FOLDER_NAME, product.ImageUrl));
        _context.Remove(product);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(int? id, UpdateProductViewModel model, StockProduct stockProduct)
    {
        if (id is null) return;

        var product = await _context.Products
            .Include(p => p.StockProduct)
            .ThenInclude(sp => sp.Products)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return;

        // Calculate available count excluding current product's count
        var currentStockUsage = product.StockProduct.Products
            .Where(p => p.Id != id)
            .Sum(p => p.InUseCount);

        var availableCount = product.StockProduct.TotalCount - currentStockUsage;

        if (availableCount < model.Count)
            throw new Exception($"Not enough available items. Available: {availableCount}, Requested: {model.Count}");

        // Update properties
        product.InUseCount = model.Count;
        product.Department = model.DepartmentName;
        product.Unit = model.UnitName;
        product.DateofReceipt = model.DateofReceipt;

        await _context.SaveChangesAsync();
    }
    public async Task<int> GetAvailableCountAsync(int stockProductId)
    {
        var stockProduct = await _context.StockProducts
            .Include(sp => sp.Products)
            .FirstOrDefaultAsync(sp => sp.Id == stockProductId);

        if (stockProduct == null) return 0;

        return stockProduct.TotalCount - stockProduct.Products.Sum(p => p.InUseCount);
    }
    #endregion
}