using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ddla.ITApplication.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IStockService _stockService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ddlaAppDBContext _context;

    public HomeController(
        IProductService productService,
        IWebHostEnvironment webHostEnvironment,
        IStockService stockService,
        ddlaAppDBContext context)
    {
        _productService = productService;
        _webHostEnvironment = webHostEnvironment;
        _stockService = stockService;
        _context = context;
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
            StockProducts = await _context.StockProducts.ToListAsync()
        };
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Create(DoubleCreateProductTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.StockProducts = await _context.StockProducts.ToListAsync();
            return View(model);
        }


        await _productService.InsertMultipleAsync(model);

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (id is null || product is null) return RedirectToAction("NotFound", "Shared");

        var viewModel = new UpdateProductViewModel()
        {
            Recipient = product.Recipient,
            InventarId = product.InventarId,
            DateofReceipt = product.DateofReceipt,
            DocumentPath = product.FilePath,
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
