using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
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
        var model = new DoubleProductTypeViewModel
        {
            CreateProductViewModel = new CreateProductViewModel(),
            StockProducts = await _stockService.GetAllAsync() ?? new List<StockProduct>()
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Create(DoubleProductTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Debug which fields are invalid
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors });

            // Log or debug these errors
            return View(model);
        }

        var createModel = model.CreateProductViewModel;
        var stockProduct = await _stockService.GetByIdAsync(createModel.StockProductId);
        if (stockProduct == null)
        {
            ModelState.AddModelError("", "Selected product not found");
            model.StockProducts = await _stockService.GetAllAsync();
            return View(model);
        }

        if (createModel.Count > stockProduct.AvailableCount)
        {
            ModelState.AddModelError("CreateProductViewModel.Count", $"Only {stockProduct.AvailableCount} items available");
            model.StockProducts = await _stockService.GetAllAsync();
            return View(model);
        }

        await _productService.InsertAsync(createModel);

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
