using ddla.ITApplication.Database.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITAsset_DDLA.Database.Models.DomainModels;

public class StockProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int TotalCount { get; set; }
    public string? FilePath { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    // Navigation property
    public List<Product> Products { get; set; } = new List<Product>();

    // Computed properties
    public int InUseCount => Products?.Sum(p => p.InUseCount) ?? 0;
    public int AvailableCount => TotalCount - InUseCount;
}