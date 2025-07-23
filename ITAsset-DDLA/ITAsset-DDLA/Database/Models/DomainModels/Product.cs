namespace ddla.ITApplication.Database.Models.DomainModels;

public class Product
{
    public int Id { get; set; }
    public string InventarId { get; set; }  
    public string Recipient { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int DepartmentId { get; set; }
    public string ImageUrl { get; set; }
    public int UnitId{ get; set; }
    public int AvailableCount => TotalCount - InUseCount;
    public int TotalCount { get; set; }
    public int InUseCount { get; set; } = 0;
    public string? FilePath { get; set; }

    public Department Department { get; set; }
    public Unit Unit { get; set; }
    public DateTime DateofIssue { get; set; }
    public DateTime? DateofReceipt { get; set; }

}
