using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace ddla.ITApplication.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    public HomeController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var models = await _productService.GetAllAsync();
        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _productService.Insert(model);
        return RedirectToAction(nameof(Table));
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
            DepartmentName = product.Department?.Name,
            UnitName = product.Unit?.Name,
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
        await _productService.Update(id, model);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _productService.Remove(id);
        return RedirectToAction(nameof(Index));
    }
}
