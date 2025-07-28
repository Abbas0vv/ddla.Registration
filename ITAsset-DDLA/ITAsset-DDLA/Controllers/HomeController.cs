using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ddla.ITApplication.Controllers;

[Authorize]
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
        var model = new DoubleCreateProductTypeViewModel
        {
            CreateProductViewModel = new CreateProductViewModel(),
            StockProducts = await _stockService.GetAllAsync() ?? new List<StockProduct>()
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Create(DoubleCreateProductTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
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

        await _productService.InsertAsync(createModel, stockProduct);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var product = await _productService.GetByIdAsync(id);
        var stockProduct = await _stockService.GetByIdAsync(product?.StockProductId);
        if (id is null || product is null) return NotFound();

        var viewModel = new UpdateProductViewModel()
        {
            Recipient = product.Recipient,
            InventarId = product.InventarId,
            Count = product.InUseCount,
            DateofReceipt = product.DateofReceipt,
            DocumentPath = product.FilePath,
            StockProductId = stockProduct.Id
        };

        var model = new DoubleUpdateProductTypeViewModel
        {
            UpdateProductViewModel = viewModel,
            StockProduct = await _stockService.GetByIdAsync(product.Id)
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, DoubleUpdateProductTypeViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var stockProduct = await _stockService.GetByIdAsync(model.UpdateProductViewModel.StockProductId);
        await _productService.UpdateAsync(id, model.UpdateProductViewModel, stockProduct);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _productService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
