using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Helpers;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class StockService : IStockService
{
    private readonly ddlaAppDBContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string FOLDER_NAME = "assets/images/Uploads/Products";
    public StockService(ddlaAppDBContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Department)
            .Include(p => p.Unit)
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
        return await _context.Products
            .Include(p => p.Department)
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<Product> GetByNameAsync(string name)
    {
        string filteredName = name.ToLower().Trim();
        return await _context.Products.FirstOrDefaultAsync(s => s.Name.ToLower() == filteredName);
    }

    public async Task Insert(CreateStockViewModel model)
    {
        var product = new Product()
        {
            Recipient = model.Recipient,
            Name = model.Name,
            Description = model.Description,
            DepartmentId = await _context.Departments
                .Where(d => d.Name == model.DepartmentName)
                .Select(d => d.Id)
                .FirstOrDefaultAsync(),
            UnitId = await _context.Units
                .Where(u => u.Name == model.UnitName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(),
            Unit = await _context.Units
                .Where(u => u.Name == model.UnitName)
                .FirstOrDefaultAsync(),
            Department = await _context.Departments
                .Where(d => d.Name == model.DepartmentName)
                .FirstOrDefaultAsync(),
            InUseCount = model.Count,
            DateofIssue = DateTime.Now,
            DateofReceipt = model.DateofReceipt,
            InventarId = IDGenerator.GenerateId(),
            ImageUrl = model.ImageFile.CreateImageFile(_webHostEnvironment.WebRootPath, FOLDER_NAME)
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task Remove(int? id)
    {
        if (id is null) return;
        var banner = await GetByIdAsync(id);

        FileExtention.RemoveFile(Path.Combine(_webHostEnvironment.WebRootPath, FOLDER_NAME, banner.ImageUrl));
        _context.Remove(banner);
        await _context.SaveChangesAsync();
    }

    public async Task Update(int? id, UpdateStockViewModel model)
    {
        if (id is null) return;
        var product = await GetByIdAsync(id);
        var modelDepartmentId = GetByNameAsync(model.DepartmentName)?.Id ?? 0;
        var modelUnitId = GetByNameAsync(model.UnitName)?.Id ?? 0;

        product.Name = model.Name;
        product.Description = model.Description;
        product.InUseCount = model.Count;
        product.DepartmentId = modelDepartmentId;
        product.UnitId = modelUnitId;
        product.DateofReceipt = model.DateofReceipt;

        if (model.ImageFile is not null)
            product.ImageUrl = model.ImageFile.UpdateFile(_webHostEnvironment.WebRootPath, FOLDER_NAME, product.ImageUrl);

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}
