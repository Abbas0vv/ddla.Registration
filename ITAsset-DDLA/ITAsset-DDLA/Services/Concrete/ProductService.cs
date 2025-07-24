using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Helpers;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
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
        return await _context.Products.FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task Remove(int? id)
    {
        if (id is null) return;
        var banner = await GetByIdAsync(id);

        FileExtention.RemoveFile(Path.Combine(_webHostEnvironment.WebRootPath, FOLDER_NAME, banner.ImageUrl));
        _context.Remove(banner);
        await _context.SaveChangesAsync();
    }

    public async Task Update(int? id, UpdateProductViewModel model)
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
