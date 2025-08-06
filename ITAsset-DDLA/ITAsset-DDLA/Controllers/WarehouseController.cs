using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ddla.ITApplication.Controllers;

[Authorize]
public class WarehouseController : Controller
{
    private readonly IStockService _stockService;
    private readonly IProductService _productService;

    public WarehouseController(IStockService stockService, IProductService productService)
    {
        _stockService = stockService;
        _productService = productService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        var stockProducts = await _stockService.GetAllAsync();

        var groupedProducts = new List<GroupedProductViewModel>();

        foreach (var group in stockProducts.GroupBy(p => p.Name))
        {
            var firstProduct = group.FirstOrDefault();
            var availableCount = await _productService.GetAviableProductCount();

            var viewModel = new GroupedProductViewModel
            {
                Name = group.Key,
                Description = firstProduct?.Description ?? string.Empty,
                TotalCount = group.Count(),
                ImagePath = firstProduct?.ImageUrl ?? string.Empty,
                AvailableCount = availableCount,
                InUseCount = group.Count() - availableCount
            };

            groupedProducts.Add(viewModel);
        }

        return View(groupedProducts);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string description)
    {
        var stockProducts = await _stockService.GetAllByDescriptionAsync(description);
        var products = await _productService.GetAllAsync();

        var model = new CompositeViewModel
        {
            StockProducts = stockProducts,
            Products = products
        };

        return View(model);
    }
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

        await _stockService.InsertAsync(model);
        return RedirectToAction(nameof(Index));
    }

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
        await _stockService.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _stockService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
