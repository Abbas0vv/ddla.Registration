using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.DomainModels;

namespace ITAsset_DDLA.Services.Abstract;

public interface IExcelService
{
    byte[] ExportProductsToExcel(List<Transfer> products);
    byte[] ExportLogsToExcel(List<ActivityLog> logs);
}