using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
namespace ddla.ITApplication.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IStockService _stockService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public HomeController(IProductService productService, IWebHostEnvironment webHostEnvironment, IStockService stockService)
    {
        _productService = productService;
        _webHostEnvironment = webHostEnvironment;
        _stockService = stockService;
    }

    public async Task<IActionResult> Index()
    {
        var models = await _productService.GetAllAsync();
        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Get all stock products for dropdown
        var stockProducts = await _stockService.GetAllAsync();
        ViewBag.StockProducts = new SelectList(stockProducts, "Id", "Name");

        return View(new CreateProductViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate dropdown if validation fails
            var stockProducts = await _stockService.GetAllAsync();
            ViewBag.StockProducts = new SelectList(stockProducts, "Id", "Name");
            return View(model);
        }

        // Get the stock product
        var stockProduct = await _stockService.GetByIdAsync(model.StockProductId);
        if (stockProduct == null)
        {
            ModelState.AddModelError("", "Selected product not found");
            return View(model);
        }

        // Check available count
        if (model.Count > stockProduct.AvailableCount)
        {
            ModelState.AddModelError("Count", $"Only {stockProduct.AvailableCount} items available");
            return View(model);
        }

        await _productService.InsertAsync(model);

        // Redirect to Index after successful creation
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (id is null || product is null) return NotFound();

        var model = new UpdateProductViewModel()
        {
            Recipient = product.Recipient,
            Name = product.Name,
            Description = product.Description,
            Count = product.InUseCount,
            DepartmentName = product.Department,
            UnitName = product.Unit,
            DateofReceipt = product.DateofReceipt,
            DocumentPath = product.FilePath,
            ImagePath = product.ImageUrl
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, UpdateProductViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _productService.UpdateAsync(id, model);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _productService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
