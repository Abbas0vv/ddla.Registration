using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class ProductService : IProductService
{
    private readonly ddlaAppDBContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IStockService _stockService;
    private const string FOLDER_NAME = "assets/images/Uploads/Products";
    public ProductService(
        ddlaAppDBContext context, 
        IWebHostEnvironment webHostEnvironment, 
        IStockService stockService)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _stockService = stockService;
    }

    #region Methods
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.StockProduct)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }
    public async Task<int> GetAviableProductCount()
    {
        return await _context.Products
            .Where(p => p.DateofReceipt == null)
            .CountAsync();
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
    public async Task<Product> GetProductByStockIdAsync(int? stockId)
    {
        return await _context.Products
            .Include(p => p.StockProduct)
            .FirstOrDefaultAsync(s => s.StockProductId == stockId);
    }
    public async Task InsertMultipleAsync(DoubleCreateProductTypeViewModel model)
    {
        // Validate input
        if (model == null || model.CreateProductViewModel.StockProductIds == null || !model.CreateProductViewModel.StockProductIds.Any())
        {
            throw new ArgumentException("Invalid input data - model or product IDs cannot be null/empty");
        }

        // Get the selected stock products
        var stockProducts = await _stockService.GetByIdsAsync(model.CreateProductViewModel.StockProductIds);
        foreach (var stockProduct in stockProducts)
        {
            stockProduct.IsActive = false; // Deactivate stock products
        }
        // Verify we found all requested products
        if (stockProducts.Count != model.CreateProductViewModel.StockProductIds.Count)
        {
            var missingIds = model.CreateProductViewModel.StockProductIds.Except(stockProducts.Select(sp => sp.Id)).ToList();
            throw new KeyNotFoundException($"Could not find all requested stock products. Missing IDs: {string.Join(",", missingIds)}");
        }

        // Process documents if they exist
        string filePath = null;

        if (model.CreateProductViewModel.DocumentFile != null)
            filePath = FileExtention.CreateFile(model.CreateProductViewModel.DocumentFile,
                    _webHostEnvironment.WebRootPath, FOLDER_NAME);

        // Create products
        var products = stockProducts.Select(stockItem => new Product
        {
            InventarId = stockItem.InventoryCode,
            Recipient = model.CreateProductViewModel.Recipient,
            Name = stockItem.Name,
            Description = stockItem.Description,
            ImageUrl = stockItem.ImageUrl,
            FilePath = filePath,
            Department = model.CreateProductViewModel.DepartmentName,
            Unit = model.CreateProductViewModel.UnitName,
            DateofReceipt = model.CreateProductViewModel.DateofReceipt,
            StockProductId = stockItem.Id
        }).ToList();

        // Add all products at once
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveAsync(int? id)
    {
        if (id is null) return;

        var product = await _context.Products
            .Include(p => p.StockProduct)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return;

        var stockProduct = await _stockService.GetByIdAsync(product.StockProductId);
        stockProduct.IsActive = true; // Reactivate stock product

        // No need to manually update StockProduct because it's computed
        FileExtention.RemoveFile(Path.Combine(_webHostEnvironment.WebRootPath, FOLDER_NAME, product.ImageUrl));
        _context.Remove(product);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(DoubleUpdateProductTypeViewModel model)
    {
        var existingProduct = await GetProductByStockIdAsync(model.UpdateProductViewModel.StockProductId);
        if (existingProduct == null)
            throw new KeyNotFoundException($"Product not found");

        // Process documents if they exist
        string filePath = existingProduct.FilePath;
        if (model.UpdateProductViewModel.DocumentFile != null)
        {
            // Delete old file if exists
            if (!string.IsNullOrEmpty(filePath))
                FileExtention.RemoveFile(Path.Combine(filePath, _webHostEnvironment.WebRootPath, FOLDER_NAME));
            filePath = FileExtention.CreateFile(model.UpdateProductViewModel.DocumentFile,
                    _webHostEnvironment.WebRootPath, FOLDER_NAME);
        }

        // Update product
        existingProduct.Recipient = model.UpdateProductViewModel.Recipient;
        existingProduct.Department = model.UpdateProductViewModel.DepartmentName;
        existingProduct.Unit = model.UpdateProductViewModel.UnitName;
        existingProduct.DateofReceipt = model.UpdateProductViewModel.DateofReceipt;
        existingProduct.FilePath = filePath;

        await _context.SaveChangesAsync();
    }
    #endregion
}