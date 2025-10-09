using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Warehouse;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Database.Models.ViewModels.Warehouse;
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
    private readonly ITransferService _transferService;
    private readonly IActivityLogger _activityLogger;
    private readonly ddlaAppDBContext _context;

    public WarehouseController(
        IStockService stockService,
        ITransferService productService,
        IActivityLogger activityLogger,
        ddlaAppDBContext context)
    {
        _stockService = stockService;
        _transferService = productService;
        _activityLogger = activityLogger;
        _context = context;
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
        var products = await _transferService.GetAllAsync();

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
    public async Task<IActionResult> Update(int id)
    {
        var product = await _stockService.GetByIdAsync(id);
        if (product == null) return RedirectToAction("NotFound", "Shared");

        var model = new UpdateStockViewModel
        {
            Id = product.Id,
            Name = product.Name,
            InventoryCode = product.InventoryCode,
            Description = product.Description,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateStockViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _stockService.UpdateAsync(model.Id, model);
        await _activityLogger.LogAsync(
            User.Identity.Name,
            $"İstifadəçi '{User.Identity.Name}' məhsulu redaktə etdi: '{model.Name}' (Inventar ID: {model.InventoryCode})"
        );
        return RedirectToAction(nameof(Index));
    }
    [Permission(PermissionType.InventoryDelete)]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        var product = await _stockService.GetByIdAsync(id);
        await _activityLogger.LogAsync(
            User.Identity.Name,
            $"İstifadəçi '{User.Identity.Name}' məhsulu sildi: '{product.Name}' (Inventar ID: {product?.InventoryCode})"
        );
        await _stockService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult AddFiles(int id)
    {
        var product = _context.StockProducts.FirstOrDefault(x => x.Id == id);
        if (product == null) return NotFound();

        return View(product);
    }

    [HttpPost]
    public IActionResult AddFiles(CreateProductFilesViewModel model, int id)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index");
        var product = _context.StockProducts.FirstOrDefault(x => x.Id == id);
        var transfer = _context.Transfers.FirstOrDefault(t => t.StockProductId == product.Id);
        if (product == null) return NotFound();

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (model.SignedFile != null && model.SignedFile.Length > 0)
        {
            var signedFolder = Path.Combine(uploadsFolder, "signed");
            Directory.CreateDirectory(signedFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.SignedFile.FileName)}";
            var filePath = Path.Combine(signedFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                model.SignedFile.CopyTo(stream);

            product.SignedFilePath = $"/uploads/signed/{fileName}";

            transfer.IsSigned = true;
        }

        if (model.ReturnedFile != null && model.ReturnedFile.Length > 0 && !string.IsNullOrEmpty(product.SignedFilePath))
        {
            var returnedFolder = Path.Combine(uploadsFolder, "returned");
            Directory.CreateDirectory(returnedFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ReturnedFile.FileName)}";
            var filePath = Path.Combine(returnedFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                model.ReturnedFile.CopyTo(stream);

            product.ReturnedFilePath = $"/uploads/returned/{fileName}";
            transfer.TransferStatus = TransferAction.Returned;
        }



        _context.SaveChanges();
        return RedirectToAction("Index");
    }


}
