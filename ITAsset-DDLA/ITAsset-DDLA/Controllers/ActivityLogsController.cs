using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using ITAsset_DDLA.Database.Models.ViewModels.ActivityLogs;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITAsset_DDLA.Controllers;

public class ActivityLogsController : Controller
{
    private readonly ddlaAppDBContext _context;
    private readonly UserManager<ddlaUser> _userManager;
    private readonly IExcelService _excelService;
    public ActivityLogsController(
        ddlaAppDBContext context,
        UserManager<ddlaUser> userManager,
        IExcelService excelService)
    {
        _context = context;
        _userManager = userManager;
        _excelService = excelService;
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
            TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize),
            LocalUsers = await _userManager.Users.ToListAsync(),
            PermissionTypes = Enum.GetValues(typeof(PermissionType)).Cast<PermissionType>().ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> GetLogs(
        int page = 1,
        int pageSize = 10,
        string userName = null,
        PermissionType? permissionType = null,
        DateTime? date = null)
    {
        var query = _context.ActivityLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userName))
            query = query.Where(l => l.UserFullName.Contains(userName));

        if (permissionType.HasValue)
        {
            var permissionString = permissionType.Value.ToString();
            query = query.Where(l => l.Action == permissionString);
        }

        if (date.HasValue)
            query = query.Where(l => l.CreatedAt.Date == date.Value.Date);

        query = query.OrderByDescending(l => l.CreatedAt);

        var totalLogs = await query.CountAsync();
        var logs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var model = new LogListViewModel
        {
            Logs = logs,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize),
            LocalUsers = await _userManager.Users.ToListAsync(),
            PermissionTypes = Enum.GetValues(typeof(PermissionType)).Cast<PermissionType>().ToList()
        };


        return PartialView("_LogListPartial", model);
    }

    [HttpGet]
    public IActionResult ExportLogsToExcel()
    {
        var logs = _context.ActivityLogs
            .OrderByDescending(l => l.CreatedAt)
            .ToList();

        var content = _excelService.ExportLogsToExcel(logs);

        return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "ActivityLogs.xlsx");
    }

}