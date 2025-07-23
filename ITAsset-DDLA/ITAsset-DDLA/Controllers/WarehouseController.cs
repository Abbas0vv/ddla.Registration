using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
namespace ddla.ITApplication.Controllers;
public class WarehouseController : Controller
{
    private readonly IStockService _stockService;

    public WarehouseController(IStockService stockService)
    {
        _stockService = stockService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _stockService.GetAllAsync();
        return View(products);
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

        await _stockService.Insert(model);
        return RedirectToAction(nameof(Index ));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var product = await _stockService.GetByIdAsync(id);
        if (id is null || product is null) return NotFound();

        var model = new UpdateStockViewModel()
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
    public async Task<IActionResult> Update(int? id, UpdateStockViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _stockService.Update(id, model);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _stockService.Remove(id);
        return RedirectToAction(nameof(Index));
    }
}
