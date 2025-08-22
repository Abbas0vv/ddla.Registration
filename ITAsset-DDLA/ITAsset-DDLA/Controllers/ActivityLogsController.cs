using ddla.ITApplication.Database;
using ITAsset_DDLA.Database.Models.ViewModels.ActivityLogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITAsset_DDLA.Controllers;

public class ActivityLogsController : Controller
{
    private readonly ddlaAppDBContext _context;
    public ActivityLogsController(ddlaAppDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var query = _context.ActivityLogs.OrderByDescending(l => l.CreatedAt);

        var totalLogs = await query.CountAsync();
        var logs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new LogListViewModel
        {
            Logs = logs,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize)
        };

        return View(model);
    }

    public async Task<IActionResult> GetLogs(int page = 1, int pageSize = 10)
    {
        var query = _context.ActivityLogs.OrderByDescending(l => l.CreatedAt);

        var totalLogs = await query.CountAsync();
        var logs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new LogListViewModel
        {
            Logs = logs,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize)
        };

        return PartialView("_LogListPartial", model);
    }
}