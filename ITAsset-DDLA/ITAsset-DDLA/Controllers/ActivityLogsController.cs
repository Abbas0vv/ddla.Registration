using ddla.ITApplication.Database;
using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;

public class ActivityLogsController : Controller
{
    private readonly ddlaAppDBContext _context;
    public ActivityLogsController(ddlaAppDBContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var logs = _context.ActivityLogs
            .OrderByDescending(l => l.CreatedAt)
            .ToList();
        return View(logs);
    }
}