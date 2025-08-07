using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ddla.ITApplication.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private readonly IStockService _stockService;
        private readonly ddlaAppDBContext _context;

        public EquipmentController(IStockService stockService, ddlaAppDBContext context)
        {
            _stockService = stockService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _stockService.GetAllAsync();
            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(int? id)
        {
            await _stockService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
