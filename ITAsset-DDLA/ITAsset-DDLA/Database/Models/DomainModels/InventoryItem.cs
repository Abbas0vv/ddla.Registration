namespace ITAsset_DDLA.Database.Models.DomainModels;

public class InventoryItem
{
    public int Id { get; set; }
    public int StockProductId { get; set; }
    public StockProduct StockProduct { get; set; }
    public string InventoryCode { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedDate { get; set; }
}