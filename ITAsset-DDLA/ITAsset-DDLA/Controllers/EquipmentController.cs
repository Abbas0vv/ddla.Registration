using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Attributes;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ddla.ITApplication.Controllers
{
    [Authorize]
    [Permission(PermissionType.EquipmentView)]
    public class EquipmentController : Controller
    {
        private readonly IStockService _stockService;

        public EquipmentController(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _stockService.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> ToggleStatus(int? id)
        {
            await _stockService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
