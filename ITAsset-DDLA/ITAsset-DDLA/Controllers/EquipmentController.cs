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
            await _activityLogger.LogAsync(User.Identity.Name, $"{product.Name}({product.InventoryCode}) məhsulunun statusunu dəyişdi.");
            await _stockService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
