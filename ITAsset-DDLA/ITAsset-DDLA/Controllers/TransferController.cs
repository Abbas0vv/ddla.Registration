using ClosedXML.Excel;
using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Helpers.Attributes;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using ITAsset_DDLA.Services.Concrete;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ddla.ITApplication.Controllers;

[Authorize]
public class TransferController : Controller
{
    private readonly IProductService _productService;
    private readonly IPdfService _pdfService;
    private readonly IExcelService _excelService;
    private readonly IStockService _stockService;
    private readonly IActivityLogger _activityLogger;
    private readonly LdapService _ldapService;
    private readonly ddlaAppDBContext _context;

    public TransferController(
        IProductService productService,
        IStockService stockService,
        ddlaAppDBContext context,
        LdapService ldapService,
        IActivityLogger activityLogger,
        IPdfService pdfService,
        IExcelService excelService)
    {
        _productService = productService;
        _stockService = stockService;
        _context = context;
        _ldapService = ldapService;
        _activityLogger = activityLogger;
        _pdfService = pdfService;
        _excelService = excelService;
    }

    [Permission(PermissionType.OperationView)]
    public async Task<IActionResult> Index()
    {
        var models = await _productService.GetAllAsync();
        return View(models);
    }

    [Permission(PermissionType.OperationAdd)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var ldapUsers = _ldapService.GetLdapUsers(); // AZ dilində title və company

        var model = new CreateTransferViewModel
        {
            CreateProductViewModel = new CreateProductViewModel(),
            LdapUsers = ldapUsers,
            StockProducts = await _context.StockProducts.ToListAsync()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransferViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.LdapUsers = _ldapService.GetLdapUsers(); // AZ dilində
            model.StockProducts = await _context.StockProducts.ToListAsync();
            return View(model);
        }

        var stockProducts = await _context.StockProducts
            .Where(s => model.CreateProductViewModel.StockProductIds.Contains(s.Id))
            .ToListAsync();

        foreach (var stockProduct in stockProducts)
        {
            await _activityLogger.LogAsync(
                User.Identity.Name,
                $"İstifadəçi '{User.Identity.Name}' yeni məhsul əlavə etdi: '{stockProduct.Name}' (Inventar ID: {stockProduct.InventoryCode})"
            );
        }

        await _productService.InsertMultipleAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [Permission(PermissionType.OperationEdit)]
    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        try
        {
            // Safely get LDAP users with null check
            var ldapUsers = _ldapService.GetLdapUsers();
            ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).Where(name => !string.IsNullOrEmpty(name)).ToList();

            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var stockProduct = await _stockService.GetByIdAsync(product.StockProductId);

            var model = new UpdateTransferViewModel
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
        catch (Exception ex)
        {
            // Provide empty list to prevent null reference in view
            ViewBag.LdapUsers = new List<string>();
            return View(new UpdateTransferViewModel());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateTransferViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Repopulate ViewBag.LdapUsers if validation fails
                var ldapUsers = _ldapService.GetLdapUsers();
                ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).Where(name => !string.IsNullOrEmpty(name)).ToList();
                return View(model);
            }

            var stockProduct = await _stockService.GetByIdAsync(model.UpdateProductViewModel.StockProductId);

            await _activityLogger.LogAsync(
                User.Identity.Name,
                $"İstifadəçi '{User.Identity.Name}' məhsulu redaktə etdi: '{stockProduct?.Name}' (Inventar ID: {stockProduct?.InventoryCode})"
            );

            await _productService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);

            // Repopulate necessary data for the view
            var ldapUsers = _ldapService.GetLdapUsers();
            ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).Where(name => !string.IsNullOrEmpty(name)).ToList();

            if (model.UpdateProductViewModel?.StockProductId != null)
            {
                model.StockProduct = await _stockService.GetByIdAsync(model.UpdateProductViewModel.StockProductId);
            }

            return View(model);
        }
    }

    [Permission(PermissionType.OperationDelete)]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _productService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsSigned(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        product.IsSigned = true;
        await _context.SaveChangesAsync();

        await _activityLogger.LogAsync(
            User.Identity.Name,
            $"Məhsul '{product.Name}' (Inventar ID: {product.InventarId}) üçün təhvil-təslim \"İmzalandı\" olaraq seçildi."
        );

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult GenerateHandoverPdf(int id)
    {
        var product = _context.Products
            .Include(p => p.StockProduct)
            .FirstOrDefault(p => p.Id == id);

        if (product == null) return NotFound();

        var pdfBytes = _pdfService.GenerateHandoverPdf(product, User.Identity.Name);
        return File(pdfBytes, "application/pdf", $"TehvilTeslim_{product.InventarId}.pdf");
    }



    [Permission(PermissionType.OperationAdd)]
    [HttpGet]
    public async Task<IActionResult> CreateBlank()
    {
        var ldapUsers = _ldapService.GetLdapUsers();
        ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).ToList();

        var model = new CreateTransferViewModel
        {
            CreateProductViewModel = new CreateProductViewModel(),
            StockProducts = await _context.StockProducts.ToListAsync()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlank(CreateTransferViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.LdapUsers = _ldapService.GetLdapUsers().Select(u => u.FullName).ToList();
            return View(model);
        }

        var userProducts = await _context.Products
           .Include(p => p.StockProduct)
           .Where(p => p.Recipient == model.CreateProductViewModel.Recipient)
           .ToListAsync();

        if (!userProducts.Any())
        {
            ModelState.AddModelError("", "Seçilmiş istifadəçiyə aid məhsul tapılmadı.");
            ViewBag.LdapUsers = _ldapService.GetLdapUsers().Select(u => u.FullName).ToList();
            return View(model);
        }

        await _activityLogger.LogAsync(User.Identity.Name,
            $"İstifadəçi '{User.Identity.Name}' {model.CreateProductViewModel.Recipient} üçün ümumi akt yaratdı.");

        var pdfBytes = _pdfService.GenerateBlankPdf(model.CreateProductViewModel.Recipient, userProducts);
        return File(pdfBytes, "application/pdf", $"TehvilTeslim_Blank_{model.CreateProductViewModel.Recipient}.pdf");
    }

    [HttpGet]
    public IActionResult ExportProductsToExcel()
    {
        var products = _context.Products
            .Include(p => p.StockProduct)
            .ToList();

        var content = _excelService.ExportProductsToExcel(products);
        return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Təhvil-Təslim.xlsx");
    }

}