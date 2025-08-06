namespace ITAsset_DDLA.Database.Models.ViewModels.Shared;

public class GroupedProductViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ProductCodePrefix { get; set; } // Common part of inventory codes
    public int TotalCount { get; set; }
    public string ImagePath { get; set; }
    public int AvailableCount { get; set; }
    public int InUseCount { get; set; }
}
