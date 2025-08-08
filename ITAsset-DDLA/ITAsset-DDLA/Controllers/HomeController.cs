using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Helpers.Extentions;
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
        if (product == null)
        {
            return NotFound();
        }

        var stockProduct = await _stockService.GetByIdAsync(product.StockProductId);

        var model = new DoubleUpdateProductTypeViewModel
        {
            UpdateProductViewModel = new UpdateProductViewModel
            {
                Recipient = product.Recipient,
                DepartmentName = product.Department,
                UnitName = product.Unit,
                DateofReceipt = product.DateofReceipt,
                StockProductId = product.StockProductId
            },
            StockProduct = stockProduct
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(DoubleUpdateProductTypeViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _productService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _productService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
