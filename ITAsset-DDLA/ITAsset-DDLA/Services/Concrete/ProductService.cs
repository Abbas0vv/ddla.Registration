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
    public async Task InsertAsync(CreateProductViewModel model, StockProduct stockProduct)
    {
        if (stockProduct == null)
            throw new Exception("Selected stock product not found");

        var selectedInventoryItems = await _context.InventoryItems
            .Where(i => model.SelectedInventoryItemIds.Contains(i.Id))
            .ToListAsync();


        // Validate we have the correct count
        if (selectedInventoryItems.Count != model.Count)
            throw new Exception("Selected inventory items count doesn't match the specified count");

        if (model.Count > await GetAviableProductCount())
            throw new Exception($"Not enough items available. Available: {await GetAviableProductCount()}, Requested: {model.Count}");

        foreach (var stockProductId in model.StockProductIds)
        {
            var product = new Product
            {
                Name = stockProduct?.Name ?? string.Empty, // Null check with fallback
                Description = stockProduct?.Description ?? string.Empty,
                Recipient = model.Recipient.Trim(),
                Department = model.DepartmentName,
                Unit = model.UnitName,
                DateofIssue = model.DateofReceipt ?? DateTime.Now, // Use provided date or current
                ImageUrl = stockProduct?.ImageUrl ?? string.Empty,
                FilePath = model.DocumentFile != null ?
                    model.DocumentFile.CreateImageFile(_webHostEnvironment.WebRootPath, "assets/images/Uploads/Documents") :
                    null
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

        }
    }
    public async Task InsertMultipleAsync(CreateProductViewModel model, List<StockProduct> stockProducts)
    {
        if (stockProducts == null || stockProducts.Count == 0)
            throw new Exception("Heç bir stock məhsul tapılmadı");

        var selectedInventoryItems = await _context.InventoryItems
            .Where(i => model.SelectedInventoryItemIds.Contains(i.Id))
            .ToListAsync();

        // Check selected inventory item count
        if (selectedInventoryItems.Count != model.Count)
            throw new Exception("Seçilmiş inventar sayı ilə daxil edilən say uyğun deyil");


        int totalAvailable = await GetAviableProductCount();
        if (model.Count > totalAvailable)
            throw new Exception($"Kifayət qədər məhsul yoxdur. Mövcud: {totalAvailable}, Tələb olunan: {model.Count}");

        // Bölüşdürmək üçün göstəricilər
        int remainingCount = model.Count;
        int inventoryIndex = 0;

        foreach (var stockProduct in stockProducts)
        {
            if (remainingCount == 0) break;

            int useCount = Math.Min(await GetAviableProductCount(), remainingCount);
            var usedInventoryItems = selectedInventoryItems
                .Skip(inventoryIndex)
                .Take(useCount)
                .ToList();

            var product = new Product
            {
                Name = stockProduct.Name ?? string.Empty,
                Description = stockProduct.Description ?? string.Empty,
                Recipient = model.Recipient.Trim(),
                Department = model.DepartmentName,
                Unit = model.UnitName,
                DateofIssue = model.DateofReceipt ?? DateTime.Now,
                ImageUrl = stockProduct.ImageUrl ?? string.Empty,
                FilePath = model.DocumentFile != null ?
                    model.DocumentFile.CreateImageFile(_webHostEnvironment.WebRootPath, "assets/images/Uploads/Documents") :
                    null,
            };

            _context.Products.Add(product);

            remainingCount -= useCount;
            inventoryIndex += useCount;
        }

        await _context.SaveChangesAsync();
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

        // Update properties
        product.Department = model.DepartmentName;
        product.Unit = model.UnitName;
        product.DateofReceipt = model.DateofReceipt;

        await _context.SaveChangesAsync();
    }
    #endregion
}