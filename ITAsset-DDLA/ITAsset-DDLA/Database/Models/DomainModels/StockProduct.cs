using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITAsset_DDLA.Database.Models.DomainModels;

public class StockProduct
{
    private readonly ddlaAppDBContext _context;

    public StockProduct(ddlaAppDBContext context)
    {
        _context = context;
    }

    public StockProduct() { }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string? FilePath { get; set; }
    public string InventoryCode { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public List<Product> Products { get; set; } = new List<Product>();
    public int InUseCount => Products?.Sum(p => p.InUseCount) ?? 0;
    public int AvailableCount => TotalCount() - InUseCount;
    public int TotalCount()
    {
        return _context.StockProducts?.Count(p => p.Name == this.Name) ?? 0;
    }

    public bool IsAvailable()
    {
        return AvailableCount > 0;
    }
}
