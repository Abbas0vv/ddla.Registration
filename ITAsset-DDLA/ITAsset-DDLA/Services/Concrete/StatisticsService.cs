using ddla.ITApplication.Database;
using ITAsset_DDLA.Database.Models.ViewModels.Statistic;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using System;

namespace ITAsset_DDLA.Services.Concrete;

public class StatisticsService : IStatisticsService
{
    private readonly ddlaAppDBContext _context;

    public StatisticsService(ddlaAppDBContext context)
    {
        _context = context;
    }

    public async Task<StatisticsViewModel> GetStatisticsAsync()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);

        var model = new StatisticsViewModel
        {
            // Təhvil verilən məhsullar
            TodayTransfers = await _context.Products.CountAsync(t => t.DateofIssue.Date == today),
            MonthlyTransfers = await _context.Products.CountAsync(t => t.DateofIssue >= startOfMonth),
            YearlyTransfers = await _context.Products.CountAsync(t => t.DateofIssue >= startOfYear),
            TotalTransfers = await _context.Products.CountAsync(),

            // Təhvil alınan məhsullar
            TodayReceipts = await _context.Products.CountAsync(t => t.DateofReceipt.HasValue && t.DateofReceipt.Value.Date == today),
            MonthlyReceipts = await _context.Products.CountAsync(t => t.DateofReceipt >= startOfMonth),
            YearlyReceipts = await _context.Products.CountAsync(t => t.DateofReceipt >= startOfYear),
            TotalReceipts = await _context.Products.CountAsync(t => t.DateofReceipt != null),


            // Məhsullar
            TotalProducts = await _context.StockProducts.CountAsync(),
            ActiveProducts = await _context.StockProducts.CountAsync(p => p.IsActive == true),
            InUseProducts = await _context.StockProducts.CountAsync(p => p.IsActive == false),
        };

        return model;
    }
}
