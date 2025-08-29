using ClosedXML.Excel;
using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Helpers.Attributes;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using ITAsset_DDLA.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ddla.ITApplication.Controllers;

[Authorize]
public class TransferController : Controller
{
    private readonly ITransferService _transferService;
    private readonly UserManager<ddlaUser> _userManager;
    private readonly IPdfService _pdfService;
    private readonly IExcelService _excelService;
    private readonly IStockService _stockService;
    private readonly IActivityLogger _activityLogger;
    private readonly LdapService _ldapService;
    private readonly ddlaAppDBContext _context;

    public TransferController(
        ITransferService productService,
        IStockService stockService,
        ddlaAppDBContext context,
        LdapService ldapService,
        IActivityLogger activityLogger,
        IPdfService pdfService,
        IExcelService excelService,
        UserManager<ddlaUser> userManager)
    {
        _transferService = productService;
        _stockService = stockService;
        _context = context;
        _ldapService = ldapService;
        _activityLogger = activityLogger;
        _pdfService = pdfService;
        _excelService = excelService;
        _userManager = userManager;
    }

    #region CRUD

    [Permission(PermissionType.OperationView)]
    public async Task<IActionResult> Index()
    {
        var models = await _transferService.GetAllAsync();
        return View(models);
    }

    [Permission(PermissionType.OperationAdd)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var ldapUsers = _ldapService.GetLdapUsers(); // burda exception tuta bilər

            var model = new CreateTransferViewModel
            {
                CreateTransferProductViewModel = new CreateTransferProductViewModel(),
                LdapUsers = ldapUsers,
                StockProducts = await _context.StockProducts.ToListAsync()
            };

            return View(model);
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            return RedirectToAction("LdapConnectionFailed", "Error");
        }
        catch (Exception)
        {
            return RedirectToAction("HandleError", "Error");
        }
    }

    [ValidateAntiForgeryToken]
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
            .Where(s => model.CreateTransferProductViewModel.StockProductIds.Contains(s.Id))
            .ToListAsync();

        string userFullName = User.Identity.Name;
        foreach (var stockProduct in stockProducts)
        {
            await _activityLogger.LogAsync(
                userFullName,
                $"İstifadəçi '{User.Identity.Name}' yeni məhsul təhvil verdi: '{stockProduct.Name}' (Inventar ID: {stockProduct.InventoryCode}) Təhvil alan '{model.CreateTransferProductViewModel.Recipient}'"
            );
            var transfer = _transferService.GetByInventaryCode(stockProduct.InventoryCode);
        }

        await _transferService.InsertMultipleAsync(model, userFullName);
        return RedirectToAction(nameof(Index));
    }

    [Permission(PermissionType.OperationEdit)]
    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        try
        {
            if (id == null) return NotFound();

            var product = await _transferService.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            var stockProduct = await _stockService.GetByIdAsync(product.StockProductId);
            var ldapUsers = _ldapService.GetLdapUsers(); // burda COMException ata bilər

            var model = new UpdateTransferViewModel
            {
                UpdateTransferProductViewModel = new UpdateTransferProductViewModel
                {
                    Recipient = product.Recipient,
                    DepartmentSection = product.DepartmentSection,
                    DateofReceipt = product.DateofReceipt,
                    StockProductId = product.StockProductId
                },
                StockProduct = stockProduct,
                LdapUsers = ldapUsers
            };

            return View(model);
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            return RedirectToAction("LdapConnectionFailed", "Error");
        }
        catch (Exception)
        {
            ViewBag.LdapUsers = new List<string>();
            return View(new UpdateTransferViewModel());
        }
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Update(UpdateTransferViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.LdapUsers = _ldapService.GetLdapUsers();
                if (model.UpdateTransferProductViewModel?.StockProductId != null)
                {
                    model.StockProduct = await _stockService.GetByIdAsync(model.UpdateTransferProductViewModel.StockProductId);
                }
                return View(model);
            }

            var stockProduct = await _stockService.GetByIdAsync(model.UpdateTransferProductViewModel.StockProductId);

            var userName = User.Identity.Name ?? "Unknown";
            await _transferService.UpdateAsync(model, userName);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);

            // Repopulate necessary data for the view
            var ldapUsers = _ldapService.GetLdapUsers();

            if (model.UpdateTransferProductViewModel?.StockProductId != null)
            {
                model.StockProduct = await _stockService.GetByIdAsync(model.UpdateTransferProductViewModel.StockProductId);
            }

            return View(model);
        }
    }

    [Permission(PermissionType.OperationDelete)]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var transfer = await _context.Transfers
            .Include(t => t.StockProduct)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transfer == null)
            return NotFound();

        // Log yazmaq
        await _activityLogger.LogAsync(
            User.Identity?.Name ?? "Unknown",
            $"Təhvil-Təslim silindi (ID: {transfer.Id}):\n" +
            $"Məhsul: {transfer.StockProduct?.Name} (Inventar ID: {transfer.StockProduct?.InventoryCode})\n" +
            $"Alan şəxs: {transfer.Recipient}\n" +
            $"Şöbə: {transfer.DepartmentSection}\n" +
            $"Tarix: {transfer.DateofReceipt}\n" +
            $"Sənəd: {transfer.FilePath}"
        );

        await _transferService.RemoveAsync(id, User.Identity.Name);
        return RedirectToAction(nameof(Index));
    }
    #endregion

    #region Additions
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> MarkAsSigned(int id)
    {
        var transfer = await _transferService.GetByIdAsync(id);
        if (transfer == null)
            return NotFound();

        transfer.IsSigned = true;
        await _context.SaveChangesAsync();

        await _activityLogger.LogAsync(
            User.Identity.Name,
            $"Məhsul '{transfer.Name}' (Inventar ID: {transfer.InventarId}) üçün təhvil-təslim \"İmzalandı\" olaraq seçildi."
        );

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult GenerateHandoverPdf(int id)
    {
        var product = _context.Transfers
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
        try
        {
            var ldapUsers = _ldapService.GetLdapUsers();

            var model = new CreateTransferViewModel
            {
                CreateTransferProductViewModel = new CreateTransferProductViewModel(),
                StockProducts = await _context.StockProducts.ToListAsync(),
                LdapUsers = ldapUsers
            };

            return View(model);
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            return RedirectToAction("LdapConnectionFailed", "Error");
        }
        catch (Exception)
        {
            return View(new CreateTransferViewModel
            {
                CreateTransferProductViewModel = new CreateTransferProductViewModel(),
                StockProducts = await _context.StockProducts.ToListAsync(),
                LdapUsers = new List<ITAsset_DDLA.LDAP.LdapUserModel>()
            });
        }
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> CreateBlank(CreateTransferViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.LdapUsers = _ldapService.GetLdapUsers();
            model.StockProducts = await _context.StockProducts.ToListAsync();
            return View(model);
        }

        var userProducts = await _context.Transfers
           .Include(p => p.StockProduct)
           .Where(p => p.Recipient == model.CreateTransferProductViewModel.Recipient)
           .ToListAsync();

        if (!userProducts.Any())
        {
            ModelState.AddModelError("", "Seçilmiş istifadəçiyə aid məhsul tapılmadı.");
            model.LdapUsers = _ldapService.GetLdapUsers();
            model.StockProducts = await _context.StockProducts.ToListAsync();
            return View(model);
        }

        await _activityLogger.LogAsync(User.Identity.Name,
            $"İstifadəçi '{User.Identity.Name}' {model.CreateTransferProductViewModel.Recipient} üçün ümumi akt yaratdı.");

        var pdfBytes = _pdfService.GenerateBlankPdf(model.CreateTransferProductViewModel.Recipient, userProducts);
        return File(pdfBytes, "application/pdf", $"TehvilTeslim_Blank_{model.CreateTransferProductViewModel.Recipient}.pdf");
    }
    [HttpGet]
    public IActionResult ExportProductsToExcel()
    {
        var products = _context.Transfers
            .Include(p => p.StockProduct)
            .ToList();

        var content = _excelService.ExportProductsToExcel(products);
        return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Təhvil-Təslim.xlsx");
    }
    #endregion


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperAdmin")] // və ya [Permission("Transfer.Return")]
    public async Task<IActionResult> Return(int? id)
    {
        var user = await _userManager.GetUserAsync(User);
        var username = user?.UserName ?? User.Identity.Name ?? "system";
        try
        {
            await _transferService.ReturnAsync(id, username);
            TempData["Success"] = "Qaytarılma qeyd edildi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}