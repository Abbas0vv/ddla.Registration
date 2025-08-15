using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;

namespace ddla.ITApplication.Database.Models.DomainModels;

public class Product
{
    public int Id { get; set; }
    public string InventarId { get; set; }
    public string Recipient { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsSigned { get; set; } = false;
    public string? ImageUrl { get; set; }
    public string? FilePath { get; set; }
    public DepartmentName Department { get; set; }
    public UnitName Unit { get; set; }
    public DateTime DateofIssue { get; set; } = DateTime.Now;
    public DateTime? DateofReceipt { get; set; }
    public int StockProductId { get; set; }
    public StockProduct StockProduct { get; set; }
}