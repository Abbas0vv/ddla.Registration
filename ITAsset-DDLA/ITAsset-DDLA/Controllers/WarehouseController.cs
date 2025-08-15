using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Helpers.Attributes;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ddla.ITApplication.Controllers;

[Authorize]
public class WarehouseController : Controller
{
    private readonly IStockService _stockService;
    private readonly IProductService _productService;
    private readonly IActivityLogger _activityLogger;

    public WarehouseController(
        IStockService stockService, 
        IProductService productService, 
        IActivityLogger activityLogger)
    {
        _stockService = stockService;
        _productService = productService;
        _activityLogger = activityLogger;
    }

    [Permission(PermissionType.InventoryView)]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var stockProducts = await _stockService.GetAllAsync();

        var grouped = stockProducts
            .GroupBy(sp => sp.Description)
            .Select(group => new GroupedProductViewModel
            {
                Name = group.First().Name,
                Description = group.Key,
                ProductCodePrefix = group.First().InventoryCode.Split('-')[0], // istəyə görə düzəlt
                TotalCount = group.Count(),
                InUseCount = group.Count(sp => !sp.IsActive),
                AvailableCount = group.Count(sp => sp.IsActive),
                ImagePath = group.First().ImageUrl
            })
            .ToList();

        return View(grouped);
    }

    [Permission(PermissionType.InventoryView)]
    [HttpGet]
    public async Task<IActionResult> Detail(string name)
    {
        var stockProducts = await _stockService.GetAllByNameAsync(name);
        var products = await _productService.GetAllAsync();

        var model = new CompositeViewModel
        {
            StockProducts = stockProducts,
            Products = products
        };

        return View(model);
    }
    
    [Permission(PermissionType.InventoryAdd)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStockViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Validate that inventory codes count matches total count
        if (model.InventoryCodes == null || model.InventoryCodes.Count != model.TotalCount)
        {
            ModelState.AddModelError("", "Please provide inventory codes for all items");
            return View(model);
        }

        await _activityLogger.LogAsync(User.Identity.Name, "anbara yeni məhsul əlavə etdi.");
        await _stockService.InsertAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [Permission(PermissionType.InventoryEdit)]
    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var product = await _stockService.GetByIdAsync(id);
        if (id is null || product is null) return RedirectToAction("NotFound", "Shared");

        var model = new UpdateStockViewModel()
        {
            Name = product.Name,
            Description = product.Description,
            DateofRegistration = product.RegistrationDate,
            DocumentPath = product.FilePath,
            ImagePath = product.ImageUrl
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, UpdateStockViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _activityLogger.LogAsync(User.Identity.Name, $"{model.Name} məhsulunu redaktə etdi.");
        await _stockService.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }

    [Permission(PermissionType.InventoryDelete)]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        var product = await _stockService.GetByIdAsync(id);
        await _activityLogger.LogAsync(User.Identity.Name, $"{product.Name} məhsulunu sildi.");
        await _stockService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
