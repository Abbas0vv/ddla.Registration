using ClosedXML.Excel;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Attributes;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ddla.ITApplication.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IActivityLogger _activityLogger;

        public EquipmentController(IStockService stockService, IActivityLogger activityLogger)
        {
            _stockService = stockService;
            _activityLogger = activityLogger;
        }

        [HttpGet]
        [Permission(PermissionType.EquipmentView)]
        public async Task<IActionResult> Index()
        {
            var products = await _stockService.GetAllAsync();
            return View(products);
        }

        [HttpPost]
        [Permission(PermissionType.EquipmentEdit)]
        public async Task<IActionResult> ToggleStatus(int? id)
        {
            var product = await _stockService.GetByIdAsync(id);
            await _activityLogger.LogAsync(
                User.Identity.Name,
                $"İstifadəçi '{User.Identity.Name}' məhsul '{product.Name}' (Inventar ID: {product.InventoryCode}) üçün statusu dəyişdi."
            );
            await _stockService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportStockProductsToExcel()
        {
            var stockProducts = await _stockService.GetAllAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Avadanlıqlar");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Status";
                worksheet.Cell(currentRow, 2).Value = "Ad";
                worksheet.Cell(currentRow, 3).Value = "Təsvir";
                worksheet.Cell(currentRow, 4).Value = "İnventar Kodu";
                worksheet.Cell(currentRow, 5).Value = "Qeydiyyat Tarixi";

                // Data
                foreach (var sp in stockProducts)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = sp.IsActive ? "Anbardadır" : "İstifadə edilir";
                    worksheet.Cell(currentRow, 2).Value = sp.Name;
                    worksheet.Cell(currentRow, 3).Value = sp.Description;
                    worksheet.Cell(currentRow, 4).Value = sp.InventoryCode;
                    worksheet.Cell(currentRow, 5).Value = sp.RegistrationDate.ToString("dd.MM.yyyy");
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "Avadanlıqlar.xlsx");
                }
            }
        }


    }
}
