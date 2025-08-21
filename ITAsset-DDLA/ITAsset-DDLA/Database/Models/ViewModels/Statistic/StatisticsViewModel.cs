namespace ITAsset_DDLA.Database.Models.ViewModels.Statistic;

public class StatisticsViewModel
{
    // Transferlər
    public int TodayTransfers { get; set; }
    public int MonthlyTransfers { get; set; }
    public int YearlyTransfers { get; set; }
    public int TotalTransfers { get; set; }

    public int TodayReceipts { get; set; }
    public int MonthlyReceipts { get; set; }
    public int YearlyReceipts { get; set; }
    public int TotalReceipts { get; set; }


    // Məhsullar
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }    // Anbarda olan
    public int InUseProducts { get; set; }     // İstifadədə olan
}
