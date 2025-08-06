using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;

namespace ITAsset_DDLA.Database.Models.DomainModels;

public class StockProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string? FilePath { get; set; }
    public string InventoryCode { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public List<Product> Products { get; set; } = new List<Product>();
}
