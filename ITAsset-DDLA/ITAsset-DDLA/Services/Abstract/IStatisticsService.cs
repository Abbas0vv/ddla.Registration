using ITAsset_DDLA.Database.Models.ViewModels.Statistic;

namespace ITAsset_DDLA.Services.Abstract;

public interface IStatisticsService
{
    Task<StatisticsViewModel> GetStatisticsAsync();
}
