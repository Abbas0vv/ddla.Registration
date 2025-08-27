using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;

namespace ITAsset_DDLA.Database.Models.DomainModels;

public class TransferHistory
{
    public int Id { get; set; }
    public int TransferId { get; set; }
    public Transfer Transfer { get; set; }

    public TransferAction Action { get; set; }
    public string Actor { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;

    public string? FromUser { get; set; }
    public string? ToUser { get; set; }
}
