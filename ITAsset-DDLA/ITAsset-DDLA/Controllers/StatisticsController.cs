using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
namespace ITAsset_DDLA.Controllers;

public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _statisticsService.GetStatisticsAsync();
        return View(model);
    }
}

