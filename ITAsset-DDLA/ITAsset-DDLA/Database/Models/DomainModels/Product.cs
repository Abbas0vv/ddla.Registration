using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;

namespace ddla.ITApplication.Database.Models.DomainModels;
public class Product
{
    public int Id { get; set; }

    public int StockProductId { get; set; }
    public StockProduct StockProduct { get; set; }

    public string InventarId => StockProduct?.InventoryCode ?? string.Empty;

    public string Recipient { get; set; }
    public string Name => StockProduct?.Name ?? string.Empty;
    public string Description => StockProduct?.Description ?? string.Empty;
    public bool IsSigned { get; set; } = false;
    public string? ImageUrl { get; set; }
    public string? FilePath { get; set; }
    public string DepartmentSection { get; set; }
    public DateTime DateofIssue { get; set; } = DateTime.Now;
    public DateTime? DateofReceipt { get; set; }
}
