using ddla.ITApplication.Database.Models.DomainModels;

namespace ITAsset_DDLA.Services.Abstract;

public interface IExcelService
{
    byte[] ExportProductsToExcel(List<Product> products);
}