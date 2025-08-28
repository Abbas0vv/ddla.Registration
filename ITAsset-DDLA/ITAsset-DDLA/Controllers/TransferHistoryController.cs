using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Services;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers
{
    public class TransferHistoryController : Controller
    {
        private readonly ITransferHistoryService _historyService;
        private readonly ITransferService _transferService; // yeni servis

        public TransferHistoryController(
            ITransferHistoryService historyService,
            ITransferService transferService) // inject et
        {
            _historyService = historyService;
            _transferService = transferService;
        }


        public async Task<IActionResult> Index()
        {
            var histories = await _historyService.GetAllAsync();
            return View(histories);
        }

        public async Task<IActionResult> TransferDetails(int id)
        {
            // ID Transfers cədvəlindəki Transfer.Id-dir
            var transfer = await _transferService.GetByIdAsync(id);
            if (transfer == null) return NotFound();

            return PartialView("_TransferDetailsPartial", transfer);
        }
    }
}
