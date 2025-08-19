using ddla.ITApplication.Database.Models.DomainModels;

namespace ITAsset_DDLA.Services.Abstract;

public interface IPdfService
{
    byte[] GenerateHandoverPdf(Product product, string username);
    byte[] GenerateBlankPdf(string recipient, List<Product> products);
}
