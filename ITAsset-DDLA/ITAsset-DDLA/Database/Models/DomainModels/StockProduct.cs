using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITAsset_DDLA.Database.Models.DomainModels;

public class StockProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string ImageUrl { get; set; }
    public string? FilePath { get; set; }
    public string InventoryCode { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public List<Transfer> Transfers { get; set; } = new List<Transfer>();
    [NotMapped]
    public bool IsCurrentlyActive => !Transfers
        .Any(t => !string.IsNullOrEmpty(t.Recipient) &&
                  t.TransferStatus != TransferAction.Returned);
}
