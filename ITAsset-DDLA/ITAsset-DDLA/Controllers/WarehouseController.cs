using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
namespace ddla.ITApplication.Controllers;
public class WarehouseController : Controller
{
    private readonly IStockService _stockService;
    private readonly IProductService _productService;

    public WarehouseController(IStockService stockService, IProductService productService)
    {
        _stockService = stockService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var stockProducts = await _stockService.GetAllAsync();
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

        await _stockService.InsertAsync(model);
        return RedirectToAction(nameof(Index ));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var product = await _stockService.GetByIdAsync(id);
        if (id is null || product is null) return NotFound();

        var model = new UpdateStockViewModel()
        {
            Name = product.Name,
            Description = product.Description,
            TotalCount = product.TotalCount,
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
